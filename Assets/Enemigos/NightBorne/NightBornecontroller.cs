using UnityEngine;
using System.Collections;

public class NightBornecontroller : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float speed = 4.5f;
    public float fuerzaRebote = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    private bool recibiendoDanio;
    private bool playerVivo = true;
    private Animator animator;
    public int vida = 3;
    private bool muerto = false;

    public GameObject monedaPrefab; // Prefab de la moneda
    public int minMonedas = 1;
    public int maxMonedas = 5;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (muerto) return; // Detén toda la lógica si está muerto

        if (playerVivo)
        {
            Movimiento();
        }
        if (animator != null)
            animator.SetBool("isMoving", enMovimiento);
    }

    private void Movimiento()
    {
        if (muerto) return; // No moverse si está muerto

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);

            movement = new Vector2(direction.x, 0);
            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }
        if (!recibiendoDanio)
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (muerto) return; // No colisionar si está muerto

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();

            playerScript.RecibirDanio(direccionDanio, 1); // <-- Usa el nombre correcto del método
            playerVivo = !playerScript.muerto;
            if (!playerVivo)
            {
                enMovimiento = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (muerto) return; // No recibir daño si está muerto

        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio && !muerto)
        {
            recibiendoDanio = true;
            vida -= cantDanio;
            if (vida <= 0)
            {
                muerto = true;
                // Instanciar monedas al morir
                int cantidad = Random.Range(minMonedas, maxMonedas + 1);
                for (int i = 0; i < cantidad; i++)
                {
                    Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.2f, 0.8f), 0);
                    Instantiate(monedaPrefab, transform.position + offset, Quaternion.identity);
                }
                if (animator != null)
                {
                    animator.SetBool("die", true);
                }
                GameManager.Instance.SumarPuntaje(1); // Suma 1 punto al morir el enemigo
            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);

                if (animator != null)
                {
                    animator.SetBool("isHurt", true); // Activa la animación de daño
                }
                StartCoroutine(DesactivaDanio());
            }
        }
    }

    public IEnumerator DesactivaDanio()
    {
        rb.linearVelocity = Vector2.zero; // Detén cualquier movimiento físico
        rb.linearVelocity = Vector2.zero; // Si usas linearVelocity (por compatibilidad)
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        if (animator != null)
        {
            animator.SetBool("isHurt", false);
        }
    }

    public void DestruirDespuesDeMorir()
    {
        Destroy(gameObject);
    }
}
