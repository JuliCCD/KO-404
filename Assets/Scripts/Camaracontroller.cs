using UnityEngine;

public class Camaracontroller : MonoBehaviour
{
    public Transform Objetivo;
    public float velocidadcamara = 0.025f;
    public Vector3 desplazamiento;

    private void LateUpdate() 
    {
        Vector3 posicionDeseada = Objetivo.position + desplazamiento;
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, velocidadcamara);

        transform.position = posicionSuavizada;
    }
}
