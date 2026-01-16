using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class XRSetupFixer : EditorWindow
{
    [MenuItem("Tools/Antigravity/Fix XR and Recreate UI")]
    public static void ShowWindow()
    {
        GetWindow<XRSetupFixer>("XR Setup Fixer");
    }

    private void OnGUI()
    {
        GUILayout.Label("XR & UI Auto-Fixer", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("1. Fix XR Interactors (Force Input Readers)"))
        {
            FixInteractors();
        }

        if (GUILayout.Button("2. Recreate World Space Canvas"))
        {
            RecreateUI();
        }

        if (GUILayout.Button("3. Fix Event System"))
        {
            FixEventSystem();
        }
        
        GUILayout.Space(20);
        GUILayout.Label("Instructions:", EditorStyles.boldLabel);
        GUILayout.Label("1. Click 'Fix XR Interactors' to solve the console error.");
        GUILayout.Label("2. Click 'Recreate World Space Canvas' to build a clean UI.");
        GUILayout.Label("3. Click 'Fix Event System' to ensure clicks work.");
    }

    static void FixInteractors()
    {
        // Find both Ray Interactors (Left and Right)
        var interactors = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        
        if (interactors.Length == 0)
        {
            // Fallback for older versions or different namespaces
            interactors = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>(); 
        }

        foreach (var interactor in interactors)
        {
            SerializedObject so = new SerializedObject(interactor);
            
            // 1. Set Input Compatibility Mode to Force Input Readers (Value = 1)
            // Property name depends on version, checking common names
            SerializedProperty compatProp = so.FindProperty("m_InputCompatibilityMode");
            if (compatProp != null)
            {
                compatProp.intValue = 1; // 1 = ForceInputReaders
                Debug.Log($"[XR Fix] Set InputCompatibilityMode to ForceInputReaders on {interactor.name}");
            }
            else
            {
                Debug.LogWarning($"[XR Fix] Could not find 'm_InputCompatibilityMode' property on {interactor.name}. You might be on an older version.");
            }

            // 2. Ensure Actions are assigned (Basic assignments if missing)
            // We can't easily guess the Input Action Assets guid without searching, 
            // but we can try to set valid references if we find common ones.
            // For now, we rely on the mode change resolving the conflict.
            
            // 3. Ensure Raycast Mask includes UI
            SerializedProperty raycastMask = so.FindProperty("m_RaycastMask");
            if (raycastMask != null)
            {
                // Set to Everything just to be safe
                raycastMask.intValue = -1; 
                Debug.Log($"[XR Fix] Set RaycastMask to Everything on {interactor.name}");
            }

            so.ApplyModifiedProperties();
        }
        
        Debug.Log("[XR Fix] Interactor Fix Complete.");
    }

    static void FixEventSystem()
    {
        EventSystem es = FindObjectOfType<EventSystem>();
        if (es == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            es = esGO.AddComponent<EventSystem>();
        }

        // Remove Standalone Input Module if it exists
        var standalone = es.GetComponent("StandaloneInputModule"); // flexible check
        if (standalone != null) DestroyImmediate(standalone);

        // Ensure XR UI Input Module exists
        var xrInput = es.GetComponent("XRUIInputModule"); // flexible check
        if (xrInput == null)
        {
            // Try adding via reflection or standard add if class is available
            // In XRI 2.0+ it's UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule
            // using string based add to avoid compilation issues if types vary
             UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(es.gameObject, "Assets/Scripts/XRSetupFixer.cs", "XRUIInputModule");
        }
        
        Debug.Log("[XR Fix] Event System Checked. Plase verify 'XR UI Input Module' is present.");
    }

    static void RecreateUI()
    {
        // 1. Delete old problematic Canvas if exists (User can undo)
        GameObject oldCanvas = GameObject.Find("Canvas");
        if (oldCanvas != null)
        {
            Undo.DestroyObjectImmediate(oldCanvas);
            Debug.Log("[XR Fix] Removed old Canvas.");
        }

        // 2. Create New Canvas
        GameObject canvasGO = new GameObject("VR_World_Canvas");
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create VR Canvas");
        
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main; // Assigning Main Camera is usually safe for event camera logic in mixed setups
        canvasGO.layer = LayerMask.NameToLayer("UI"); // Layer 5

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        
        // 3. Add Tracked Device Graphic Raycaster
        // Namespace varies, trying standard. If this fails to compile, user needs to add manually, but usually it's in standard namespace.
        // We use Reflection to avoid hard dependency if the user hasn't imported namespace in this script context
        // But we added 'using UnityEngine.XR.Interaction.Toolkit;' so it should work if it's there.
        // Actually, it's often in UnityEngine.XR.Interaction.Toolkit.UI
        
        var raycasterType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster, Unity.XR.Interaction.Toolkit");
        if(raycasterType == null) raycasterType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.TrackedDeviceGraphicRaycaster, Unity.XR.Interaction.Toolkit");

        if (raycasterType != null)
        {
            var raycaster = canvasGO.AddComponent(raycasterType);
            // Set Ignore Reversed Graphics to false via serialized object to be generic
            SerializedObject soRay = new SerializedObject(raycaster);
            SerializedProperty ignoreRev = soRay.FindProperty("m_IgnoreReversedGraphics");
            if (ignoreRev != null) ignoreRev.boolValue = false;
            soRay.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogError("Could not find Tracked Device Graphic Raycaster type.");
        }

        // Position it nicely in front of the rig
        canvasGO.transform.position = new Vector3(0, 1.5f, 2.0f); // 2 meters forward, eye level
        canvasGO.transform.localScale = Vector3.one * 0.005f; // Small scale for world space
        canvasGO.transform.rotation = Quaternion.identity;

        // 4. Create Background Panel
        GameObject panel = new GameObject("Background");
        panel.transform.SetParent(canvasGO.transform, false);
        panel.AddComponent<CanvasRenderer>();
        Image bgImg = panel.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.8f);
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(400, 300);

        // 5. Create Play Button
        GameObject buttonGO = new GameObject("PlayButton");
        buttonGO.transform.SetParent(panel.transform, false);
        buttonGO.AddComponent<CanvasRenderer>();
        Image btnImg = buttonGO.AddComponent<Image>();
        btnImg.color = Color.white;
        Button btn = buttonGO.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        RectTransform btnRT = buttonGO.GetComponent<RectTransform>();
        btnRT.sizeDelta = new Vector2(160, 60);
        btnRT.anchoredPosition = new Vector2(0, 50);

        // Button Text (TMP)
        GameObject btnTextGO = new GameObject("Text");
        btnTextGO.transform.SetParent(buttonGO.transform, false);
        TextMeshProUGUI btnTmp = btnTextGO.AddComponent<TextMeshProUGUI>();
        btnTmp.text = "PLAY";
        btnTmp.alignment = TextAlignmentOptions.Center;
        btnTmp.color = Color.black;
        btnTmp.fontSize = 36;
        RectTransform btnTextRT = btnTextGO.GetComponent<RectTransform>();
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;

        // 6. Create Score Text
        GameObject scoreGO = new GameObject("ScoreText");
        scoreGO.transform.SetParent(panel.transform, false);
        TextMeshProUGUI scoreTmp = scoreGO.AddComponent<TextMeshProUGUI>();
        scoreTmp.text = "Score: 0";
        scoreTmp.alignment = TextAlignmentOptions.Center;
        scoreTmp.color = Color.white;
        scoreTmp.fontSize = 48;
        RectTransform scoreRT = scoreGO.GetComponent<RectTransform>();
        scoreRT.anchoredPosition = new Vector2(0, -50);
        scoreRT.sizeDelta = new Vector2(300, 60);

        Debug.Log("[XR Fix] Recreated VR Canvas successfully.");
    }
}
#endif
