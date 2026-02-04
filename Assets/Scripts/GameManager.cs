using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class GameManager : MonoBehaviour
{
    public static int Puntuacion = 0;
    public TMP_Text t;

    void Start()
    {
        // Asegurar que el EventSystem tenga el XRUIInputModule correcto para VR
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem != null)
        {
            // Intentar obtener XRUIInputModule
            XRUIInputModule xrInputModule = eventSystem.GetComponent<XRUIInputModule>();
            
            // Si no existe, lo configuramos
            if (xrInputModule == null)
            {
                // Si existe el modulo standard de InputSystem, lo quitamos porque entra en conflicto o no sirve para VR Laser
                InputSystemUIInputModule inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
                if (inputSystemModule != null)
                {
                    Destroy(inputSystemModule);
                }

                // Añadimos el modulo de VR
                eventSystem.gameObject.AddComponent<XRUIInputModule>();
                Debug.Log("GameManager: XRUIInputModule añadido para corregir interacción VR.");
            }
        }
        leftRay = leftHandController.GetComponentInChildren<Behaviour>();
        rightRay = rightHandController.GetComponentInChildren<Behaviour>();
    }
    // Menu y mandos
    public GameObject leftHandController;
    public GameObject rightHandController;

    Behaviour leftRay;
    Behaviour rightRay;
    
    public GameObject sabaer1;
    public GameObject saber2;

    
    public void InGame()
    {
        rightRay.enabled = false;
        leftRay.enabled = false;

        sabaer1.SetActive(true);
        saber2.SetActive(true);
    }

    
    public void InMenu()
    {
        rightRay.enabled = true;
        leftRay.enabled = true;

        sabaer1.SetActive(false);
        saber2.SetActive(false);
    }



    void Update()
    {
        t.text = Puntuacion.ToString();
    }
}

