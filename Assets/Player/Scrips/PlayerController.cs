using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Velocidad del jugador
    public float velocidad = 6f;

    public Animator animator;

    public float fuerzaSalto = 10f;
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
        // Obtener la entrada horizontal 
        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;

        animator.SetFloat("movement", Mathf.Abs(velocidadX * velocidad)); // Use Mathf.Abs for positive movement values

        if (velocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (velocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // Obtener la posición actual del jugador
        Vector3 posicion = transform.position;

        // Actualizar la posición del jugador
        transform.position = new Vector3(posicion.x + velocidadX, posicion.y, posicion.z);

        // Fixed RaycastHit2D and Physics2D.Raycast usage
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, CapaSuelo);
        enSuelo = hit.collider != null;

        if (enSuelo && Input.GetKeyDown(KeyCode.Space)) // Fixed KeyCode from Escape to Space for jumping
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }

        animator.SetBool("ensuelo", enSuelo); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}