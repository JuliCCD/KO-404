using UnityEngine;
using TMPro; 

public class MostrarPuntaje : MonoBehaviour
{
    public TMP_Text textoPuntaje;

    void Update()
    {
        textoPuntaje.text = "Puntaje: " + GameManager.Instance.puntaje;
    }
}
