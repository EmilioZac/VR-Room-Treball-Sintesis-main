using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonDebug : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("DEBUG: Puntero Entró en el botón");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("DEBUG: Puntero Salió del botón");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("DEBUG: Puntero Presionado sobre el botón");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("DEBUG: Click completado en el botón");
    }
}
