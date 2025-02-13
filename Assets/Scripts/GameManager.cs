using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    int[] vectorCasillas;
    int[] infoCasillas;
    GameObject[] vectorObjetos;

    private void Awake()
    {

        vectorCasillas = new int[21];
        infoCasillas   = new int[21];

        // RELLENAMOS EL VECTOR DE CASILLAS
        for (int i = 0; i < vectorCasillas.Length; i++)
            vectorCasillas[i] = 0;

        // RELLENAMOS EL VECTOR DE INFO CASILLAS
        for (int i = 0; i < infoCasillas.Length; i++)
            infoCasillas[i] = 0;

        // teleports, destino +6
        infoCasillas[0] = 1;
        infoCasillas[5] = 1;

        // volver a tirar
        infoCasillas[13] = 2;
        infoCasillas[17] = 2;

        // retroceder 3 casillas
        infoCasillas[4] = -1;
        infoCasillas[9] = -1;
        infoCasillas[13] = -1;
        infoCasillas[18] = -1;
        infoCasillas[19] = -1;

        // victoria
        infoCasillas[20] = 99;

        //No pintar casillas, tenemos que infoCasillas[i] y que lea el numero

        // RELLENAMOS EL VECTOR DE GAMEOBJECTS
        //vectorObjetos = GameObject.FindGameObjectsWithTag("casilla");

        // METODO 1: OBTENER LOS HIJOS DE UN PARENT VACÍO

        // METODO 2: RELLENAR CON UN FOR Y UN FIND
        vectorObjetos = new GameObject[21];

        for (int i = 0; i < vectorObjetos.Length; i++)
            vectorObjetos[i] = GameObject.Find("casilla" + i);


        // METODO 3: ORDENAR LA LISTA A PARTIR DE LA LISTA DE TAGS
        // LA MAS COMPLICADA PERO LA MAS EFICIENTE

        // 21 CASILLAS DESORDENADAS
        GameObject[] vectorGOCasillas = GameObject.FindGameObjectsWithTag("casilla");

        for (int i = 0; i < vectorGOCasillas.Length; i++)
        {
            GameObject casilla = vectorGOCasillas[i];
            // falta terminar ..

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
