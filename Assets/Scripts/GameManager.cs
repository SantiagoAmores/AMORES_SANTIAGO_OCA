using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    int turnoActual = 1;
    int ronda = 1;
    int posicionJugador = 0;
    int posicionIA = 0;
    public GameObject[] casillas; // Array de todas las casillas
    public List<int> tipoCasillas; // Lista que almacena los tipos de las casillas

    private void Awake()
    {
        vectorCasillas = new int[21];
        infoCasillas = new int[21];

        // RELLENAMOS EL VECTOR DE CASILLAS
        for (int i = 0; i < vectorCasillas.Length; i++)
        {
            vectorCasillas[i] = 0;
        }

        // RELLENAMOS EL VECTOR DE INFO CASILLAS SEGÚN EL PDF
        for (int i = 0; i < infoCasillas.Length; i++)
        {
            infoCasillas[i] = 0;
        }

        // Asignamos las reglas específicas para ciertas casillas
        AsignarTiposCasillas();
        // Cambiamos el color de las casillas según el tipo
        CambiarColoresCasillas();
    }

    // Asignar el tipo de casilla según su número
    void AsignarTiposCasillas()
    {
        tipoCasillas = new List<int>();

        for (int i = 0; i < casillas.Length; i++)
        {
            // Asignación de tipos según el número de la casilla (índice en la lista)
            if (i == 1 || i == 6) // Casillas de teleport
            {
                tipoCasillas.Add(1);
            }
            else if (i == 12 || i == 18) // Casillas de "volver a tirar"
            {
                tipoCasillas.Add(2);
            }
            else if (i == 5 || i == 10 || i == 14 || i == 19) // Casillas de retroceder
            {
                tipoCasillas.Add(-1);
            }
            else if (i == 20) // Casilla de victoria
            {
                tipoCasillas.Add(99);
            }
            else // Casillas normales
            {
                tipoCasillas.Add(0);
            }
        }
    }

    // Cambiar el color de las casillas según su tipo
    void CambiarColoresCasillas()
    {
        for (int i = 0; i < casillas.Length; i++)
        {
            Renderer casillaRenderer = casillas[i].GetComponent<Renderer>();

            if (casillaRenderer == null)
            {
                Debug.LogError("No se encontró un Renderer en la casilla: " + casillas[i].name);
                continue; // Si no tiene Renderer, se pasa a la siguiente
            }

            // Cambiar el color de la casilla según el tipo
            if (tipoCasillas[i] == 1) // Teleport
            {
                casillaRenderer.material.color = Color.blue;
            }
            else if (tipoCasillas[i] == 2) // Volver a tirar
            {
                casillaRenderer.material.color = Color.green;
            }
            else if (tipoCasillas[i] == -1) // Retroceder
            {
                casillaRenderer.material.color = Color.red;
            }
            else if (tipoCasillas[i] == 99) // Victoria
            {
                casillaRenderer.material.color = Color.yellow;
            }
            else // Normal
            {
                casillaRenderer.material.color = Color.white;
            }
        }
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
        MoverFicha(resultadoDado);
    }

    void MoverFicha(int pasos)
    {
        if (turnoActual == 1)
        {
            posicionJugador += pasos;
            if (posicionJugador > 20)
            {
                posicionJugador = 20;
                Debug.Log("¡El jugador ha ganado!");
            }
            AplicarReglas(ref posicionJugador);
            fichaJugador.transform.position = vectorObjetos[posicionJugador].transform.position;
        }
        else
        {
            posicionIA += pasos;
            if (posicionIA > 20)
            {
                posicionIA = 20;
                Debug.Log("¡La IA ha ganado!");
            }
            AplicarReglas(ref posicionIA);
            fichaIA.transform.position = vectorObjetos[posicionIA].transform.position;
        }
        CambiarTurno();
    }

    void AplicarReglas(ref int posicion)
    {
        if (infoCasillas[posicion] == 1)
        {
            Debug.Log("Teleport activado");
            posicion += 6;
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
    }

    void CambiarTurno()
    {
        if (turnoActual == 1)
        {
            turnoActual = 2;
        }
        else
        {
            turnoActual = 1;
            ronda++;
        }
        ActualizarCanvas();
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
}
