using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public int[] vectorCasillas;
    public int[] infoCasillas;

    GameObject[] vectorObjetos;
    public GameObject fichaJugador;
    public GameObject fichaIA;

    public TextMeshProUGUI rondaText;
    public TextMeshProUGUI jugadorActivoText;
    public TextMeshProUGUI resultadoDadoText;

    public GameObject resultadoCanvas;
    public TextMeshProUGUI textoVictoria;
    public GameObject botonReinicio;

    int turnoActual = 1;
    int ronda = 1;
    int posicionJugador = -1;
    int posicionIA = -1;

    private void Awake()
    {
        vectorCasillas = new int[21];
        infoCasillas = new int[21];

        // RELLENAMOS EL VECTOR DE CASILLAS
        for (int i = 0; i < vectorCasillas.Length; i++)
        {
            vectorCasillas[i] = 0;
        }

        // RELLENAMOS EL VECTOR DE INFO CASILLAS
        for (int i = 0; i < infoCasillas.Length; i++)
        {
            infoCasillas[i] = 0;
        }

        infoCasillas[1] = 1; // Teleport a 7
        infoCasillas[6] = 1; // Teleport a 12
        infoCasillas[13] = 2; // Volver a tirar
        infoCasillas[18] = 2; // Volver a tirar
        infoCasillas[5] = -1; // Retrocede 3
        infoCasillas[10] = -1; // Retrocede 3
        infoCasillas[14] = -1; // Retrocede 3
        infoCasillas[19] = -1; // Retrocede 3
        infoCasillas[20] = 99; // Victoria

        // RELLENAMOS EL VECTOR DE GAMEOBJECTS
        vectorObjetos = new GameObject[21];
        for (int i = 0; i < vectorObjetos.Length; i++)
        {
            vectorObjetos[i] = GameObject.Find("casilla" + i);
        }

        CambiarColorCasilla(); // Actualizar los colores de las casillas

        resultadoCanvas.SetActive(false);
    }

    public void TirarDado()
    {
        StartCoroutine(AnimacionDado());
    }

    private IEnumerator AnimacionDado()
    {
        float duracion = 1f;
        float tiempo = 0;
        while (tiempo < duracion)
        {
            resultadoDadoText.text = Random.Range(1, 7).ToString();
            tiempo += Time.deltaTime;
            yield return null;
        }
        int resultadoDado = 1;// Random.Range(1, 7);
        resultadoDadoText.text = resultadoDado.ToString();
        Debug.Log("Dado: " + resultadoDado);
        MoverFicha(resultadoDado);
    }

    void MoverFicha(int pasos)
    {
        int nuevaPosicion;

        if (turnoActual == 1) // Turno del jugador
        {
            nuevaPosicion = posicionJugador + pasos;
            nuevaPosicion = Mathf.Clamp(nuevaPosicion, 0, 20); // Limita la posición entre 0 y 20

            // Si la IA ya está en esa posición, recalculamos una nueva posición
            if (nuevaPosicion == posicionIA)
            {
                nuevaPosicion = AjustarPosicion(nuevaPosicion);
            }

            posicionJugador = nuevaPosicion;
            AplicarReglas(ref posicionJugador);
            fichaJugador.transform.position = vectorObjetos[posicionJugador].transform.position;
        }
        else // Turno de la IA
        {
            nuevaPosicion = posicionIA + pasos;
            nuevaPosicion = Mathf.Clamp(nuevaPosicion, 0, 20); // Limita la posición entre 0 y 20

            // Si el jugador ya está en esa posición, recalculamos una nueva posición
            if (nuevaPosicion == posicionJugador)
            {
                nuevaPosicion = AjustarPosicion(nuevaPosicion);
            }

            posicionIA = nuevaPosicion;
            AplicarReglas(ref posicionIA);
            fichaIA.transform.position = vectorObjetos[posicionIA].transform.position;
        }

        CambiarTurno();
    }

    int AjustarPosicion(int posicionActual)
    {
        int nuevaPosicion;
        if (posicionActual == 20) return 19; // Si ya está en la meta, no hay opción de moverse

        if (posicionActual == 0) return 1; // Si está en la primera casilla, solo puede avanzar

        // Decidimos aleatoriamente si avanzamos o retrocedemos
        if (Random.value < 0.5f)
        {
            nuevaPosicion = posicionActual + 1; // Avanza
        }
        else
        {
            nuevaPosicion = posicionActual - 1; // Retrocede
        }

        nuevaPosicion = Mathf.Clamp(nuevaPosicion, 0, 20); // Limita la posición entre 0 y 20
        Debug.Log($"Casilla ocupada. Moviendo a {nuevaPosicion}");
        return nuevaPosicion;
    }

    void AplicarReglas(ref int posicion)
    {
        if (posicion >= 0 && posicion < infoCasillas.Length) // Asegura que la posición esté dentro del rango válido
        {
            if (infoCasillas[posicion] == 1)
            {
                Debug.Log("Teleport activado");
                posicion += 6;
                posicion = Mathf.Clamp(posicion, 0, 20); // Limita la posición entre 0 y 20
            }
            else if (infoCasillas[posicion] == -1)
            {
                Debug.Log("Retroceso activado");
                posicion -= 3;
                if (posicion < 0) posicion = 0;
            }
            else if (infoCasillas[posicion] == 2)
            {
                Debug.Log("Vuelves a tirar");
                TirarDado();
            }

            else if (infoCasillas[posicion] == 99)
            {
                if (turnoActual == 1) // Si es el turno del jugador
                {
                    MostrarVictoria("¡Jugador ha ganado!");
                }
                else // Si es el turno de la IA
                {
                    MostrarVictoria("¡IA ha ganado!");
                }
            }
        }
        else
        {
            Debug.LogError("Posición fuera de los límites del array infoCasillas: " + posicion);
        }
    }

    void CambiarTurno()
    {
        if (turnoActual == 1)
        {
            turnoActual = 2;
            StartCoroutine(TirarDadoIA()); // La IA tira automáticamente después de 1 segundo
        }
        else
        {
            turnoActual = 1;
            ronda++;
        }
        ActualizarCanvas();
    }

    private IEnumerator TirarDadoIA()
    {
        yield return new WaitForSeconds(1f); // Espera 1seg antes de tirar
        TirarDado();
    }

    void ActualizarCanvas()
    {
        rondaText.text = "Ronda: " + ronda;
        if (turnoActual == 1)
        {
            jugadorActivoText.text = "Turno de: Jugador";
        }
        else
        {
            jugadorActivoText.text = "Turno de: IA";
        }
    }

    // RELLENAMOS EL VECTOR DE CASILLAS
    public void CambiarColorCasilla()
    {
        for (int i = 0; i < vectorCasillas.Length; i++)
        {
            if (infoCasillas[i] == 1)
            {
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.green;
            }
            else if (infoCasillas[i] == 3)
            {
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.blue;
            }
            else if (infoCasillas[i] == 2)
            {
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.white;
            }
            else if (infoCasillas[i] == -1)
            {
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.red;
            }
            else if (infoCasillas[i] == 99)
            {
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
    }

    public void MostrarVictoria(string mensaje)
    {
        resultadoCanvas.SetActive(true); // Activamos el canvas de victoria
        textoVictoria.text = mensaje; // Mostramos el mensaje de victoria

        // Deshabilitar el botón de reinicio hasta que el jugador haga clic en él
        botonReinicio.SetActive(true);
    }

    public void ReiniciarJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga la escena actual
    }
}
