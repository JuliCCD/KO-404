using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Velocidad del jugador
    public float velocidad = 5f;

    public Animator animator;

    public float fuerzaSalto = 0.5f;
    public float longitudRaycast = 0.1f; // Fixed typo in variable name
    public LayerMask CapaSuelo;

    private bool enSuelo;
    private Rigidbody2D rb; // Fixed typo in class name

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MoverJugador();
        VerificarSuelo();
        Saltar();
        ActualizarAnimaciones();
    }

    private void MoverJugador()
    {
        // Obtener la entrada horizontal 
        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;

        if (velocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (velocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // Actualizar la posición del jugador
        transform.position += new Vector3(velocidadX, 0, 0);
    }

    private void VerificarSuelo()
    {
        // Fixed RaycastHit2D and Physics2D.Raycast usage
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, CapaSuelo);
        enSuelo = hit.collider != null;
    }

    private void Saltar()
    {
        if (enSuelo && Input.GetKeyDown(KeyCode.Space)) // Fixed KeyCode from Escape to Space for jumping
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }
    }

    private void ActualizarAnimaciones()
    {
        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;
        animator.SetFloat("movement", Mathf.Abs(velocidadX * velocidad)); // Use Mathf.Abs for positive movement values
        animator.SetBool("ensuelo", enSuelo);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}