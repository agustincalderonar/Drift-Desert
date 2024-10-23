using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuOpcionesScript : MonoBehaviour
{
    //Para los ajustar para bajar o subir el volumen accedemos a los mixers
    public AudioMixer audioMixer;

    //inicializo los slider para darle el valor del volumen
    public Slider sliderMusica;

    public Slider sliderSFX;


    private void Start()
    {
        //Obtenemos el volumen guardado por el jugador, lo asignamos al slider y lo aplicamos al audioMixer
        sliderMusica.value = PlayerPrefs.GetFloat("SliderMusica");
        CambiarVolumenMusica(sliderMusica.value);

        sliderSFX.value = PlayerPrefs.GetFloat("SliderSFX");
        CambiarVolumenSFX(sliderSFX.value);
    }

    public void CambiarVolumenMusica(float volumen)
    {
        //Lo hago logaritmico porque asi funciona el sonido
        audioMixer.SetFloat("VolumenMusica", Mathf.Log10(volumen)*20);
        PlayerPrefs.SetFloat("SliderMusica", volumen);
    }

    public void CambiarVolumenSFX(float volumen)
    {
        audioMixer.SetFloat("VolumenSFX", Mathf.Log10(volumen) * 20);
        PlayerPrefs.SetFloat("SliderSFX", volumen);
    }
}
