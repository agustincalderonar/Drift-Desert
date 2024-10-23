using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicialScript : MonoBehaviour
{
    public void Jugar()
    {
        //En file/Build Settings podemos reordenar las escenas y asignarles diferentes numeros
        //SceneManager.LoadScene(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
