using UnityEngine;
using UnityEngine.EventSystems;

public class BotonDerecha : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public CocheScript coche;
    private bool girandoDerecha = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        girandoDerecha = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        girandoDerecha = false;
    }

    private void FixedUpdate()
    {
        // Gira el objeto si el botón derecho está siendo presionado
        if (girandoDerecha)
        {
            coche.girarDerecha();
        }
    }
}
