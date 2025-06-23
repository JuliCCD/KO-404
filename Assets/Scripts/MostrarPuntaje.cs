using UnityEngine;
using TMPro; 

public class MostrarPuntaje : MonoBehaviour
{
    public TMP_Text textoPuntaje;
    public TMP_Text textoMonedas; // Nuevo campo para mostrar monedas

    void Update()
    {
        textoPuntaje.text = "Puntaje: " + GameManager.Instance.puntaje;
        textoMonedas.text = "Monedas: " + GameManager.Instance.monedas; // Actualiza monedas
    }
}
