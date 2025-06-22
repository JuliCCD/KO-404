using UnityEngine;

public class NightBornecontroller : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float speed = 2f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator; // 1. Referencia al Animator

    private bool recibiendoDanio;
    public float fuerzaRebote = 5f; // Fuerza del rebote al recibir daño

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // 2. Inicializar el Animator
    }

    void Update()
    {
        if(!recibiendoDanio)
        mover();

    }
    private void mover()
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

            Vector2 direccionDanio = new Vector2(transform.position.x,0);
            collision.gameObject.GetComponent<PlayerController>().RecibirDanio(direccionDanio, 1);

            // Daño y stun
            //PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            //if (player != null)
            //{
            //    player.RecibirDanio(1); // Llama a un método para recibir daño
            //    player.Stunear(0.5f);  
            //}
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {

            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x,0);
            RecibirDanio(direccionDanio, 1);

        }
    }

    public void RecibirDanio(Vector2 direccion, int cantDanio)
    {
        recibiendoDanio = true;
        if (animator != null)
        {
            animator.SetBool("isHurt", true); // Activa la animación de daño
        }
        Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
        rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocityX = 0;
        rb.linearVelocity = Vector2.zero;
        if (animator != null)
        {
            animator.SetBool("isHurt", false); // Desactiva la animación de daño
        }
    }
}
