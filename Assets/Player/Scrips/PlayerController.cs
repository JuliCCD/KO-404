using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f; // Velocidad de movimiento horizontal
    public float fuerzaSalto = 0.5f; // Fuerza del salto
    public float longitudRaycast = 0.1f; // Longitud del raycast para detectar el suelo
    public float dashVelocidad = 10f; // Velocidad del dash
    public float dashDuracion = 0.2f; // Duración del dash
    public LayerMask CapaSuelo; // Capa que define qué es considerado suelo

    private bool enSuelo; // Indica si el jugador está en el suelo
    private bool isDashing = false; // Indica si el jugador está haciendo dash
    private bool dashUsado = false; // Indica si el dash ya fue usado en el aire
    private float dashTiempoRestante = 0f; // Tiempo restante del dash
    private Rigidbody2D rb; // Referencia al Rigidbody2D del jugador
    public Animator animator; // Referencia al Animator para manejar animaciones

    void Start()
    {
        // Inicializar el Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Si no está haciendo dash, permitir otros movimientos
        if (!isDashing)
        {
            MoverJugador();
            VerificarSuelo();
            Saltar();
        }

        // Manejar el dash
        Dash();

        // Actualizar las animaciones
        ActualizarAnimaciones();
    }

    private void MoverJugador()
    {
        // Manejar el movimiento horizontal del jugador
        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;

        // Cambiar la dirección del sprite según el movimiento
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
        // Detectar si el jugador está tocando el suelo usando dos raycasts (uno a cada lado)
        Vector2 raycastOriginIzquierda = (Vector2)transform.position + new Vector2(-0.1f, 0); // Desplazar el raycast hacia la izquierda
        Vector2 raycastOriginDerecha = (Vector2)transform.position + new Vector2(0.1f, 0); // Desplazar el raycast hacia la derecha

        RaycastHit2D hitIzquierda = Physics2D.Raycast(raycastOriginIzquierda, Vector2.down, longitudRaycast, CapaSuelo);
        RaycastHit2D hitDerecha = Physics2D.Raycast(raycastOriginDerecha, Vector2.down, longitudRaycast, CapaSuelo);

        // El jugador está en el suelo si cualquiera de los dos raycasts detecta una colisión
        enSuelo = hitIzquierda.collider != null || hitDerecha.collider != null;

        // Resetear el dash cuando el jugador toca el suelo
        if (enSuelo)
        {
            dashUsado = false;
        }
    }

    private void Saltar()
    {
        // Manejar el salto del jugador
        if (enSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }
    }

    private void Dash()
    {
        // Manejar el dash del jugador
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && (enSuelo || !dashUsado))
        {
            isDashing = true;

            // Si está en el aire, marcar el dash como usado
            if (!enSuelo)
            {
                dashUsado = true;
            }

            dashTiempoRestante = dashDuracion;

            // Aplicar velocidad de dash en la dirección actual
            float dashDirection = transform.localScale.x; // Dirección basada en la escala del jugador
            rb.linearVelocity = new Vector2(dashDirection * dashVelocidad, rb.linearVelocity.y);
        }

        if (isDashing)
        {
            // Reducir el tiempo restante del dash
            dashTiempoRestante -= Time.deltaTime;
            if (dashTiempoRestante <= 0)
            {
                // Terminar el dash
                isDashing = false;
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Detener el movimiento horizontal
            }
        }
    }

    private void ActualizarAnimaciones()
    {
        // Actualizar las animaciones del jugador
        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;
        animator.SetFloat("movement", Mathf.Abs(velocidadX * velocidad)); // Animación de movimiento
        animator.SetBool("ensuelo", enSuelo); // Animación de estar en el suelo

        // Animación de caída
        if (!enSuelo && rb.linearVelocity.y < 0)
        {
            animator.SetBool("fall", true);
        }
        else
        {
            animator.SetBool("fall", false);
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar los raycasts en la escena para depuración
        Gizmos.color = Color.red;

        Vector2 raycastOriginIzquierda = (Vector2)transform.position + new Vector2(-0.1f, 0); // Desplazar el raycast hacia la izquierda
        Vector2 raycastOriginDerecha = (Vector2)transform.position + new Vector2(0.1f, 0); // Desplazar el raycast hacia la derecha

        Gizmos.DrawLine(raycastOriginIzquierda, raycastOriginIzquierda + Vector2.down * longitudRaycast);
        Gizmos.DrawLine(raycastOriginDerecha, raycastOriginDerecha + Vector2.down * longitudRaycast);
    }
}