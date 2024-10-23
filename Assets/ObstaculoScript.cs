using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoScript : MonoBehaviour
{
    public float ratioDesaparicion = 6;

    private float contador = 0;

    //Para controlar las animaciones
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (contador < ratioDesaparicion)
        {
            contador += Time.deltaTime;

            //Le pasamos el tiempo del contador al parametro tiempo que controla las transiciones entre animaciones
            animator.SetFloat("Tiempo", contador);
        }
        else
        {
            Debug.Log("Obstaculo eliminado");
            Destroy(gameObject);
        }
    }

}
