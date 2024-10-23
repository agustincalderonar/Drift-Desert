using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class LogicScript : MonoBehaviour
{
    public int puntuacion;
    public int puntuacionRecord;

    public TMP_Text textoPuntuacion;
    public TMP_Text textoPuntuacionRecord;

    public GameObject pantallaGameOver;
    public GeneradorObstaculosScript generadorObstaculosScript;

    private int monedasRecogidas = 0;
    private int valorMoneda = 1;
    private Camera camara;
    private AudioSource musicaFondoAudioSource;
    private bool juegoPausado = false;

    //se ejecuta antes, asi no da problemas cuando llaman a la funcion obtenerLimiteHorizontal()
    void Awake()
    {
        puntuacionRecord = PlayerPrefs.GetInt("PuntuacionRecord", 0);
        actualizarTextoRecord();

        camara = Camera.main;
        Debug.Log("Area segura: " + Screen.safeArea);
        musicaFondoAudioSource = camara.GetComponent<AudioSource>();
        musicaFondoAudioSource.Play();
    }


    public void sumarPuntuacion()
    {
        monedasRecogidas++;
        puntuacion += valorMoneda;
        textoPuntuacion.text = puntuacion.ToString();

        if(puntuacion > puntuacionRecord)
        {
            actualizarRecord();
            actualizarTextoRecord();
        }

        //Se aumenta la dificultad cada 10 monedas
        if (monedasRecogidas % 10 == 0) aumentarDificultad();
    }

    private void actualizarRecord()
    {
        puntuacionRecord = puntuacion;
        PlayerPrefs.SetInt("PuntuacionRecord", puntuacionRecord);
        PlayerPrefs.Save();
    }

    private void actualizarTextoRecord()
    {
        textoPuntuacionRecord.text = "HS:" + puntuacionRecord;
    }

    private void aumentarDificultad()
    {
        generadorObstaculosScript.aumentarObstaculos();
        valorMoneda++;
        Debug.Log("Dificultad aumentada");
    }

    public void pausarJuego()
    {
        //Se detiene el tiempo
        Time.timeScale = 0f;
        juegoPausado = true;
    }
    public void reanudarJuego()
    {
        //Reanuda el tiempo
        Time.timeScale = 1f;
        juegoPausado = false;
    }

    public void reiniciarJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        musicaFondoAudioSource.Play();
        //cocheScript.cocheFunciona = true;
        Debug.Log(SceneManager.GetActiveScene().name);
    }

    public void volverMenu()
    {
        if (juegoPausado)
            reanudarJuego();

        //En file/Build Settings podemos reordenar las escenas y asignarles diferentes numeros
        //SceneManager.LoadScene(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void gameOver()
    {
        musicaFondoAudioSource.Stop();
        pantallaGameOver.SetActive(true);
    }

    //Para obtener el ancho, debo adaptar el area de acción del juego a los limites de la camara,
    //tengo que escalarlo con la proporción de la camara, camera.aspect
    public float obtenerLimiteHorizontalObjetos()
    {
        float limiteH = camara.aspect * 3.7f;
        Debug.Log("Proporción de la camara: " + limiteH);
        return limiteH;
    }
    

    public float obtenerLimiteHorizontalCoche(float tamañoCoche)
    {
        float limiteH = (camara.aspect * 4) + tamañoCoche;
        Debug.Log("Proporción de la camara: " + limiteH);
        return limiteH;
    }
}
