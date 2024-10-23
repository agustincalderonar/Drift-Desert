using UnityEngine;
using UnityEngine.EventSystems;

public class BotonIzquierda : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public CocheScript coche;
    private bool girandoIzquierda = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        girandoIzquierda = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        girandoIzquierda = false;
    }

    private void FixedUpdate()
    {
        // Gira el objeto si el botón izquierdo está siendo presionado
        if (girandoIzquierda)
        {
            coche.girarIzquierda();
        }
    }
}
