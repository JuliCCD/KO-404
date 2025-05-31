using UnityEngine;

public class NightBornecontroller : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float speed = 2f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator; // 1. Referencia al Animator

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // 2. Inicializar el Animator
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            movement = new Vector2(direction.x, 0f);
        }
        else
        {
            movement = Vector2.zero;
        }

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        // Voltear el sprite según la dirección del movimiento
        if (movement.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Mirar a la derecha
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Mirar a la izquierda
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", Mathf.Abs(movement.x) > 0.01f);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja un círculo de detección en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Rebote
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 reboundDirection = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(reboundDirection * 5f, ForceMode2D.Impulse); // Ajusta la fuerza a tu gusto
            }

            // Daño y stun
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.RecibirDanio(1); // Llama a un método para recibir daño
                player.Stunear(0.5f);  
            }
        }
    }
}
