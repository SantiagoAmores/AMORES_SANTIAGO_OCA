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

        infoCasillas[1] = 1; // Teleport a 7
        infoCasillas[6] = 1; // Teleport a 13
        infoCasillas[12] = 2; // Volver a tirar
        infoCasillas[18] = 2; // Volver a tirar
        infoCasillas[5] = -1; // Retrocede 3
        infoCasillas[10] = -1;
        infoCasillas[14] = -1;
        infoCasillas[19] = -1;
        infoCasillas[20] = 99; // Victoria

        // RELLENAMOS EL VECTOR DE GAMEOBJECTS
        vectorObjetos = new GameObject[21];
        for (int i = 0; i < vectorObjetos.Length; i++)
        {
            vectorObjetos[i] = GameObject.Find("casilla" + i);
        }

        // ORDENAMOS EL VECTOR DE GAMEOBJECTS POR NUMERO DE CASILLA
        GameObject[] vectorGOCasillas = GameObject.FindGameObjectsWithTag("casilla");
        for (int i = 0; i < vectorGOCasillas.Length; i++)
        {
            GameObject casilla = vectorGOCasillas[i];
            Casilla casillaScript = casilla.GetComponent<Casilla>();
            if (casillaScript != null)
            {
                vectorObjetos[casillaScript.numeroCasilla] = casilla;
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
            resultadoDadoText.text = "Dado: " + Random.Range(1, 7);
            tiempo += Time.deltaTime;
            yield return null;
        }
        int resultadoDado = Random.Range(1, 7);
        resultadoDadoText.text = "Dado: " + resultadoDado;
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
