using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int puntaje = 0;
    public int monedas = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
        }
    }

    public void SumarPuntaje(int cantidad)
    {
        puntaje += cantidad;
        Debug.Log("Puntaje: " + puntaje);
    }

    public void SumarMoneda(int cantidad)
    {
        monedas += cantidad;
        // Aquí puedes actualizar el UI si tienes uno
    }
}
