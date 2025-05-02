using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    public float velocidad = 1f;
    public float fuerzaSalto = 1f;
    bool enSuelo = true; // Necesitarías detectar si está en el suelo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        MoverseHorizontal();
        Saltar();
    }

    void MoverseHorizontal()
    {
        float movimiento = 0f;
        animator.SetInteger("Estado", 0); // Idle por defecto

        if (Input.GetKey(KeyCode.RightArrow))
        {
            movimiento = velocidad;
            sr.flipX = false;
            animator.SetInteger("Estado", 1); // Caminando
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            movimiento = -velocidad;
            sr.flipX = true;
            animator.SetInteger("Estado", 1); // Caminando
        }

        rb.linearVelocity = new Vector2(movimiento, rb.linearVelocity.y);
    }

    void Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            enSuelo = false; // Necesitas lógica para restablecerlo al tocar el suelo
            animator.SetInteger("Estado", 2);
        }
    }

    // Puedes usar OnCollisionEnter2D para detectar cuando toca el suelo
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true;
        }
    }
}
