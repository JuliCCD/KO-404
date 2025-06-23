using UnityEngine;

public class Moneda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.SumarMoneda(1); // Suma 1 moneda
            Destroy(gameObject); // Desaparece la moneda
        }
    }
}