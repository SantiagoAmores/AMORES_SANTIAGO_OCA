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

    // Array para almacenar los eventos (máximo 5 eventos)
    string[] eventos = new string[5];
    public TextMeshProUGUI textoColaEventos;

    private void Awake()
    {
        vectorCasillas = new int[21];
        infoCasillas = new int[21];

        // Rellenamos el vector de casillas
        for (int i = 0; i < vectorCasillas.Length; i++)
        {
            vectorCasillas[i] = 0;
        }

        // Rellenamos el vector de infoCasillas
        for (int i = 0; i < infoCasillas.Length; i++)
        {
            infoCasillas[i] = 0;
        }

        infoCasillas[1] = 1;  // Teleport: avanza 6 posiciones
        infoCasillas[6] = 1;  // Teleport: avanza 6 posiciones
        infoCasillas[13] = 2; // Vuelve a tirar
        infoCasillas[18] = 2; // Vuelve a tirar
        infoCasillas[5] = -1; // Retrocede 3 posiciones
        infoCasillas[10] = -1; // Retrocede 3 posiciones
        infoCasillas[14] = -1; // Retrocede 3 posiciones
        infoCasillas[19] = -1; // Retrocede 3 posiciones
        infoCasillas[20] = 99; // Victoria

        // Rellenamos el vector de GameObjects (se asume que existen objetos llamados "casilla0", "casilla1", etc.)
        vectorObjetos = new GameObject[21];
        for (int i = 0; i < vectorObjetos.Length; i++)
        {
            vectorObjetos[i] = GameObject.Find("casilla" + i);
        }

        CambiarColorCasilla(); // Actualizar colores según el tipo de casilla

        resultadoCanvas.SetActive(false);

        // Inicializamos el array de eventos (vacío)
        for (int i = 0; i < eventos.Length; i++)
        {
            eventos[i] = "";
        }
        // Añadimos los eventos iniciales
        AnyadirEvento("Inicio del juego");
        AnyadirEvento("Turno de Jugador");

        // Obtenemos la referencia al TextMeshPro del canvas de eventos
        textoColaEventos = GameObject.Find("ColaEventos").GetComponent<TextMeshProUGUI>();
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
        int resultadoDado = Random.Range(1, 7);
        resultadoDadoText.text = resultadoDado.ToString();
        Debug.Log("Dado: " + resultadoDado);

        // Añadimos el evento de tirada
        if (turnoActual == 1)
            AnyadirEvento("Jugador tira: " + resultadoDado);
        else
            AnyadirEvento("IA tira: " + resultadoDado);

        MoverFicha(resultadoDado);
    }

    void MoverFicha(int pasos)
    {
        int nuevaPosicion;

        if (turnoActual == 1) // Turno del jugador
        {
            nuevaPosicion = posicionJugador + pasos;
            nuevaPosicion = Mathf.Clamp(nuevaPosicion, 0, 20);

            // Si la IA ya está en esa casilla, se ajusta la posición
            if (nuevaPosicion == posicionIA)
            {
                nuevaPosicion = AjustarPosicion(nuevaPosicion);
            }

            AnyadirEvento("Jugador se mueve a la casilla " + nuevaPosicion);
            posicionJugador = nuevaPosicion;
            AplicarReglas(ref posicionJugador);
            fichaJugador.transform.position = vectorObjetos[posicionJugador].transform.position;
        }
        else // Turno de la IA
        {
            nuevaPosicion = posicionIA + pasos;
            nuevaPosicion = Mathf.Clamp(nuevaPosicion, 0, 20);

            if (nuevaPosicion == posicionJugador)
            {
                nuevaPosicion = AjustarPosicion(nuevaPosicion);
            }

            AnyadirEvento("IA se mueve a la casilla " + nuevaPosicion);
            posicionIA = nuevaPosicion;
            AplicarReglas(ref posicionIA);
            fichaIA.transform.position = vectorObjetos[posicionIA].transform.position;
        }

        CambiarTurno();
    }

    int AjustarPosicion(int posicionActual)
    {
        int nuevaPosicion;
        if (posicionActual == 20) return 19;
        if (posicionActual == 0) return 1;

        // Se decide aleatoriamente si se avanza o retrocede
        if (Random.value < 0.5f)
            nuevaPosicion = posicionActual + 1;
        else
            nuevaPosicion = posicionActual - 1;

        nuevaPosicion = Mathf.Clamp(nuevaPosicion, 0, 20);
        Debug.Log($"Casilla ocupada. Moviendo a {nuevaPosicion}");
        AnyadirEvento("Casilla ocupada. Ajuste a " + nuevaPosicion);
        return nuevaPosicion;
    }

    void AplicarReglas(ref int posicion)
    {
        if (posicion >= 0 && posicion < infoCasillas.Length)
        {
            int valorCasilla = infoCasillas[posicion];

            if (valorCasilla == 1)
            {
                AnyadirEvento("Casilla especial (Teleport) en la casilla " + posicion + ". Avanza 6 posiciones.");
                Debug.Log("Teleport activado");
                posicion += 6;
                posicion = Mathf.Clamp(posicion, 0, 20);
            }
            else if (valorCasilla == -1)
            {
                AnyadirEvento("Casilla especial (Retroceso) en la casilla " + posicion + ". Retrocede 3 posiciones.");
                Debug.Log("Retroceso activado");
                posicion -= 3;
                if (posicion < 0) posicion = 0;
            }
            else if (valorCasilla == 2)
            {
                int resultadoDadoExtra = Random.Range(1, 7);
                resultadoDadoText.text = resultadoDadoExtra.ToString();
                AnyadirEvento("Casilla especial (Vuelve a tirar) en la casilla " + posicion + ". Dado extra: " + resultadoDadoExtra);
                Debug.Log("Vuelve a tirar");
                MoverFicha(resultadoDadoExtra);
            }
            else if (valorCasilla == 99)
            {
                if (turnoActual == 1)
                {
                    AnyadirEvento("Casilla especial (Meta) en la casilla " + posicion + ". ¡Jugador ha ganado!");
                    MostrarVictoria("¡Jugador ha ganado!");
                    Debug.Log("Jugador ha ganado");
                }
                else
                {
                    AnyadirEvento("Casilla especial (Meta) en la casilla " + posicion + ". ¡IA ha ganado!");
                    MostrarVictoria("¡IA ha ganado!");
                    Debug.Log("IA ha ganado");
                }
            }
        }
        else
        {
            Debug.LogError("Posición fuera de los límites: " + posicion);
        }
    }

    void CambiarTurno()
    {
        if (turnoActual == 1)
        {
            turnoActual = 2;
            AnyadirEvento("Cambio de turno: Ahora es turno de IA.");
            StartCoroutine(TirarDadoIA());
        }
        else
        {
            turnoActual = 1;
            ronda++;
            AnyadirEvento("Cambio de turno: Ahora es turno de Jugador.");
        }
        ActualizarCanvas();
    }

    private IEnumerator TirarDadoIA()
    {
        yield return new WaitForSeconds(1f);
        TirarDado();
    }

    void ActualizarCanvas()
    {
        rondaText.text = "Ronda: " + ronda;
        jugadorActivoText.text = turnoActual == 1 ? "Turno de: Jugador" : "Turno de: IA";
    }

    public void CambiarColorCasilla()
    {
        for (int i = 0; i < vectorCasillas.Length; i++)
        {
            if (infoCasillas[i] == 1)
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.green;
            else if (infoCasillas[i] == 3)
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.blue;
            else if (infoCasillas[i] == 2)
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.white;
            else if (infoCasillas[i] == -1)
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.red;
            else if (infoCasillas[i] == 99)
                vectorObjetos[i].GetComponent<Renderer>().material.color = Color.yellow;
        }
    }

    public void MostrarVictoria(string mensaje)
    {
        resultadoCanvas.SetActive(true);
        textoVictoria.text = mensaje;
        botonReinicio.SetActive(true);
    }

    public void ReiniciarJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Método para añadir un evento al array desplazando los elementos,
    // de forma que siempre se muestren solo los 5 últimos eventos
    void AnyadirEvento(string nuevoEvento)
    {
        // Desplazar los elementos una posición a la izquierda
        for (int i = 0; i < eventos.Length - 1; i++)
        {
            eventos[i] = eventos[i + 1];
        }
        // Insertar el nuevo evento en la última posición
        eventos[eventos.Length - 1] = nuevoEvento;
    }

    // Método para mostrar en el canvas los eventos actuales
    void MostrarColaEventos()
    {
        string textoCola = "";
        foreach (string ev in eventos)
        {
            if (!string.IsNullOrEmpty(ev))
                textoCola += ev + "\n";
        }
        textoColaEventos.text = textoCola;
    }

    void Update()
    {
        MostrarColaEventos();
    }
}
