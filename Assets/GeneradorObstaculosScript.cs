using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorObstaculosScript : MonoBehaviour
{
    //lo asigno desde el inspector, es mas eficiente(en ocasiones no se puede por que el objeto se crea en tiempo de ejecución)
    public LogicScript logic;
    public GameObject obstaculo;
    public GameObject avisoObstaculo;
    public float ratioGeneracion;


    private float limiteHorizontal;
    private float contador = 0;
    private Vector3 posicionNueva;
    private GameObject avisoGenerado;
    private SpriteRenderer spriteAviso;
    private float ratioOpacidad;


    void Start()
    {
        limiteHorizontal = logic.obtenerLimiteHorizontalObjetos();
        //generarObstaculo();
    }

    void Update()
    {
        if (contador == 0)
        {
            //Creamos la posicion del nuevo obstaculo
            posicionNueva = generarPosicion();
            //generamos el aviso
            avisoGenerado = generarAviso();
            spriteAviso = avisoGenerado.GetComponent<SpriteRenderer>();

            ratioOpacidad = (1.0f - spriteAviso.color.a) / ratioGeneracion;
            //ratioOpacidad = (spriteAviso.color.a) / ratioGeneracion;
        }
        //contamos hasta el ratio de generacion
        if (contador < ratioGeneracion)
        {
            contador += Time.deltaTime;

            //Creo un nuevo color mas opaco
            Color nuevoColor = spriteAviso.color;

            nuevoColor.a += ratioOpacidad * Time.deltaTime;
            spriteAviso.color = nuevoColor;
        }
        //cuando llegue el momento:
        else
        {
            //generamos el obstaculo
            generarObstaculo();
            contador = 0;
            //eliminamos el aviso
            Destroy(avisoGenerado);
        }
    }

    void generarObstaculo()
    {
        Instantiate(obstaculo, posicionNueva, transform.rotation);
    }

    private GameObject generarAviso()
    {
        GameObject aviso = Instantiate(avisoObstaculo, new Vector3(posicionNueva[0], posicionNueva[1], transform.position.z), transform.rotation);
        return aviso;
    }

    private Vector3 generarPosicion()
    {
        Collider2D collider;
        Vector2 posicionNueva;
        //Así evitamos que se generen objetos donde ya hay otros
        do
        {
            posicionNueva = new Vector2(Random.Range(-limiteHorizontal, limiteHorizontal), Random.Range(-3.7f, 3.7f));
            collider = Physics2D.OverlapCircle(posicionNueva, 0.3f);
            if (collider != null) Debug.Log("Se han detectado " + collider + "colisiones objeto");
        }
        while (collider != null);

        return new Vector3(posicionNueva[0], posicionNueva[1], transform.position.z);
    }

    //Funcion par amentar la dificultad
    public void aumentarObstaculos()
    {
        ratioGeneracion -= 0.2f;
        Debug.Log("Nuevo ratio de generación" + ratioGeneracion);
    }
}
