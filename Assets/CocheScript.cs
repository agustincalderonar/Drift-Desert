using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CocheScript : MonoBehaviour
{
    public float factorAceleracion;
    public float factorRotacion;
    public float factorDerrape;
    public float velocidadMaxima;
    public bool cocheFunciona = true;
    public float tamañoCoche;

    //Efectos de sonido
    public AudioSource derrapeAudioSource;
    public AudioSource motorAudioSource;
    public AudioSource choqueAudioSource;

    //Los limites del mapa
    public float margenLimiteMapa;

    float limiteIzquierda;
    float limiteDerecha;
    float limiteArriba;
    float limiteAbajo;

    //Variables locales
    float inputDireccion;
    //El input de aceleracion sera siempre 1 ya que el coche acelerara por defecto
    float inputAceleracion = 1;
    float anguloRotacion;
    bool supensionMarcas = false;
    bool cocheEstaDerrapando = false;

    
    //Componentes
    Rigidbody2D cocheRigidBody;
    LogicScript logic;


    //Awake is called when the script instance is being loaded.
    private void Awake()
    {
        cocheRigidBody = GetComponent<Rigidbody2D>();

        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    private void Start()
    {
        limiteArriba = 4 + tamañoCoche;
        limiteAbajo = -limiteArriba;
        limiteDerecha = logic.obtenerLimiteHorizontalCoche(tamañoCoche); 
        limiteIzquierda = -limiteDerecha;
    }

    //Cada frame comprueba la posicion
    private void Update()
    {
        cocheEstaDerrapando = ComprobarDerrape();
        ComprobarPosicion();
        ComprobarMarcas();
        GestionarSonidos();
    }


    //Frame-rate independent for physics calculations.
    private void FixedUpdate()
    {
        //definimos el input para ordenador, el de movil es con las funciones girarIzquierda() y girarDerecha() asignadas a los botones
        inputDireccion = Input.GetAxis("Horizontal");

        Acelerar();

        EliminarVelocidadLateral();

        Girar(inputDireccion);
    }

    private bool ComprobarDerrape()
    {
        if(Mathf.Abs(obtenerVelocidadLateral()) > 0.75f)
        {
            return true;
        }

        return false;
    }

    private void ComprobarPosicion()
    {
        //Si el coche se encuntra cerca del limite del mapa, se desactivan las marcas, que se mantienen así hasta que salga de esta zona
        if(transform.position.x < limiteIzquierda + margenLimiteMapa || transform.position.x > limiteDerecha - margenLimiteMapa || transform.position.y < limiteAbajo + margenLimiteMapa || transform.position.y > limiteArriba - margenLimiteMapa)
        {
            marcasEmitiendo(false);
            supensionMarcas = true;
        }
        else supensionMarcas = false;

        // Comprueba si el coche se sale del mapa por algun lado para llevarlo al lado opuesto
        if (transform.position.x < limiteIzquierda)
        {
            transform.position = new Vector3(limiteDerecha, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > limiteDerecha)
        {
            transform.position = new Vector3(limiteIzquierda, transform.position.y, transform.position.z);
        }

        if (transform.position.y < limiteAbajo)
        {
            transform.position = new Vector3(transform.position.x, limiteArriba, transform.position.z);
        }
        else if (transform.position.y > limiteArriba)
        {
            transform.position = new Vector3(transform.position.x, limiteAbajo, transform.position.z);
        }
    }

    //Activa o desactiva las marcas segun sea necesario
    private void ComprobarMarcas()
    {
        //Si el coche esta en zona critica se ponen las marcas en suspension (desactivadas sin opcion a activarse por esta funcion)
        if(supensionMarcas)
        {
            return;
        }

        //Si el coche derrapa se activan las marcas
        if (cocheEstaDerrapando)
        {
            marcasEmitiendo(true);
        }
        else
        {
            marcasEmitiendo(false);
        }
    }

    //Alterna la acctivacion de las marcas segun se desee
    private void marcasEmitiendo(bool eleccion)
    {
        TrailRenderer[] trailRenderer = transform.Find("Marcas").GetComponentsInChildren<TrailRenderer>();
        if (trailRenderer != null)
        {
            foreach (TrailRenderer trail in trailRenderer)
            {
                trail.emitting = eleccion;
                //Debug.Log("Marcas = " + eleccion);
            }
        }
    }

    private void GestionarSonidos()
    {
        
        if (cocheFunciona)
        {
            //Cuanto mas rapido vaya el coche mas sonará el motor y mas grave será

            motorAudioSource.volume = obtenerVelocidadVertical() * 0.25f;

            //Variamos tambien el tono
            float tonoMotor = obtenerVelocidadVertical() * 0.2f;
            //Le ponemos un minimo y maximo
            tonoMotor = Mathf.Clamp(tonoMotor, 0.5f, 2f);
            //Irá pasando del tono actual al deseado(futuro) a un ritmo de 10 por fraccion de tiempo
            motorAudioSource.pitch = Mathf.Lerp(motorAudioSource.pitch, tonoMotor, Time.deltaTime * 1.5f);

            if (cocheEstaDerrapando)
            {
                derrapeAudioSource.volume = Mathf.Abs(obtenerVelocidadLateral()) * 0.25f;
            }
            else derrapeAudioSource.volume = Mathf.Lerp(derrapeAudioSource.volume, 0, Time.deltaTime * 10);
        }
        else
        {
            //Cuando el coche choca, ambos se mutean
            motorAudioSource.volume = Mathf.Lerp(motorAudioSource.volume, 0, Time.deltaTime * 10);
            derrapeAudioSource.volume = Mathf.Lerp(derrapeAudioSource.volume, 0, Time.deltaTime * 10);
        }
    }

    //Calculamos la velocidad vertical(hacia delante) y la lateral
    private float obtenerVelocidadVertical()
    {
         return Vector2.Dot(cocheRigidBody.velocity, transform.up);
    }
    private float obtenerVelocidadLateral()
    {
        return Vector2.Dot(cocheRigidBody.velocity, transform.right);
    }


    private void Acelerar()
    {
        //el vector aceleracion es nuestra componente que apunta hacia arriba, morro del coche
        Vector2 vectorAceleracion = transform.up * factorAceleracion * inputAceleracion;

        //Si pasamos la velocidad maxima vertical, dejamos de acelerar
        if (obtenerVelocidadVertical() > velocidadMaxima)
            return;
        //Si pasamos la velocidad maxima, dejamos de acelerar
        if (cocheRigidBody.velocity.magnitude > velocidadMaxima)
            return;


        //si el coche funciona, le aplicamos el vectorAceleracion como una fuerza constante, si no, le aplicamos drag par aque se vaya frenando
        if (cocheFunciona)
        {
            cocheRigidBody.AddForce(vectorAceleracion, ForceMode2D.Force);
        }
        else
        {
            cocheRigidBody.drag = Mathf.Lerp(cocheRigidBody.drag, 3.0f, Time.fixedDeltaTime * 1);
            cocheRigidBody.angularDrag = Mathf.Lerp(cocheRigidBody.angularDrag, 3.0f, Time.fixedDeltaTime * 1);
        }
    }


    private void Girar(float inputDireccion)
    {
        //Segun el input tendremos un angulo de rotacion
        anguloRotacion -= inputDireccion * factorRotacion;

        //Aplicamos la rotacion al rb para que sea mas organico(en vez de al gameObject en si o cambiar cocheRigidBody.rotation)
        if (cocheFunciona)
            cocheRigidBody.MoveRotation(anguloRotacion);
    }

    //Funciones llamadas por los botones de direccion
    public void girarIzquierda()
    {
        Girar(-1);
    }
    public void girarDerecha()
    {
        Girar(1);
    }


    private void EliminarVelocidadLateral()
    {
        //Obtenemos la velocidad horizontal y vertical en forma de vectores
        Vector2 vectorVelocidadVertical = transform.up * obtenerVelocidadVertical();
        Vector2 vectorVelocidadLateral = transform.right * obtenerVelocidadLateral();

        //Eliminamos la velocidad lateral segun el factor de derrape 
        cocheRigidBody.velocity = vectorVelocidadVertical + vectorVelocidadLateral * factorDerrape;
    }


    //Al chocar con un obstaculo pierde
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //asi solo tendra en cuenta los obstaculos que son las que estan en la capa 7
        if (collision.gameObject.layer == 7)
        {
            float velocidadChoque = collision.relativeVelocity.magnitude;
            choqueAudioSource.volume = velocidadChoque * 0.1f;
            choqueAudioSource.pitch = Random.Range(0.97f, 1.03f);

            if (!choqueAudioSource.isPlaying) choqueAudioSource.Play();

            logic.gameOver();
            cocheFunciona = false;
        }
    }
}