using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla : MonoBehaviour
{
    public int numeroCasilla;
    public int tipoCasilla; // 0 = normal, 1 = teleport, 2 = volver a tirar, -1 = retroceder, 99 = victoria

    private void Awake()
    {
        AsignarTipoCasilla();
        CambiarColorCasilla();
    }

    // Asigna el tipo de casilla según el número de casilla
    public void AsignarTipoCasilla()
    {
        if (numeroCasilla == 1 || numeroCasilla == 6)
        {
            tipoCasilla = 1; // Teleport
        }
        else if (numeroCasilla == 12 || numeroCasilla == 18)
        {
            tipoCasilla = 2; // Volver a tirar
        }
        else if (numeroCasilla == 5 || numeroCasilla == 10 || numeroCasilla == 14 || numeroCasilla == 19)
        {
            tipoCasilla = -1; // Retroceder
        }
        else if (numeroCasilla == 20)
        {
            tipoCasilla = 99; // Victoria
        }
    }

    // Cambia el color de las casillas hijas según su tipo
    public void CambiarColorCasilla()
    {
        // Recorremos todos los hijos del objeto actual (contendor de casillas)
        foreach (Transform hijo in transform) // 'transform' es el objeto contenedor que tiene los hijos
        {
            // Obtenemos el Renderer del hijo (casilla individual)
            Renderer casillaRenderer = hijo.GetComponent<Renderer>();


            // Cambiar el color de la casilla según su tipo
            if (tipoCasilla == 1) // Teleport
            {
                casillaRenderer.material.color = Color.blue;
            }
            else if (tipoCasilla == 2) // Volver a tirar
            {
                casillaRenderer.material.color = Color.green;
            }
            else if (tipoCasilla == -1) // Retroceder
            {
                casillaRenderer.material.color = Color.red;
            }
            else if (tipoCasilla == 99) // Victoria
            {
                casillaRenderer.material.color = Color.yellow;
            }
            else // Normal
            {
                casillaRenderer.material.color = Color.white;
            }
        }
    }
}
