using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonedaScript : MonoBehaviour
{

    public LogicScript logic;
    private float limiteHorizontal;
    private AudioSource monedaAudioSource;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        limiteHorizontal = logic.obtenerLimiteHorizontalObjetos();
        transform.position = new Vector3(Random.Range(-limiteHorizontal, limiteHorizontal), Random.Range(-2.5f, 3.7f), transform.position.z);

        monedaAudioSource = GetComponent<AudioSource>();
    }

    private void CambiarPosicion()
    {
        Collider2D collider;
        Vector2 posicionNueva;

        //Así evitamos que se generen objetos donde ya hay otros
        do
        {
            posicionNueva = new Vector2(Random.Range(-limiteHorizontal, limiteHorizontal), Random.Range(-3.7f, 3.7f));
            collider = Physics2D.OverlapCircle(posicionNueva, 0.4f);
            if(collider != null) Debug.Log("Se han detectado " + collider + "colisiones moneda");
        }
        while (collider != null);

        transform.position = new Vector3(posicionNueva[0], posicionNueva[1], transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si el jugador choca con la moneda, se incrementa la puntuacion
        //collision.gameObject.layer == 3
        if (collision.gameObject.CompareTag("Player"))
        {
            monedaAudioSource.Play();
            logic.sumarPuntuacion();
            CambiarPosicion();
        }
    }
}
