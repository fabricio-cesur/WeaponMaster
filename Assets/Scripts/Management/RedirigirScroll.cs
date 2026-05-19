using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Añadimos IScrollHandler para obligar a este objeto a escuchar la rueda del ratón
public class RedirigirScroll : MonoBehaviour, IScrollHandler
{
    private ScrollRect scrollRect;

    void Start()
    {
        // El script busca automáticamente hacia arriba en la jerarquía 
        // hasta encontrar el componente ScrollRect (tu Scroll View)
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    // Esta función nativa se dispara automáticamente cuando giras la rueda 
    // del ratón mientras estás encima de este objeto
    public void OnScroll(PointerEventData eventData)
    {
        if (scrollRect != null)
        {
            // Le pasamos los datos del ratón directamente al Scroll View
            scrollRect.OnScroll(eventData);
        }
    }
}