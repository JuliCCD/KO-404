using UnityEngine;

public class ManoDaño : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 direccion = (collision.transform.position - transform.position).normalized;
            collision.GetComponent<PlayerController>().RecibirDanio(direccion, 1);
        }
    }
}
