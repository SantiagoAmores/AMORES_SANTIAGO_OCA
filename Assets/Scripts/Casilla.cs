using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla : MonoBehaviour
{
    public int numeroCasilla;
    public int tipoCasilla; // 0 = normal, 1 = teleport, 2 = volver a tirar, -1 = retroceder, 99 = victoria

    private void Awake()
    {
        string nombre = gameObject.name;
        if (nombre.StartsWith("casilla"))
        {
            string numeroStr = nombre.Substring(7);
            int numero;
            if (int.TryParse(numeroStr, out numero))
            {
                numeroCasilla = numero;
            }
        }
        AsignarTipoCasilla();
    }

    private void AsignarTipoCasilla()
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
        else
        {
            tipoCasilla = 0; // Normal
        }
    }
}


