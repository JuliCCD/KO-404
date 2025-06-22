using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    // --- Variables públicas ---
    public float velocidad = 5f;
    public float fuerzaSalto = 0.5f;
    public float longitudRaycast = 0.1f;
    public float dashVelocidad = 10f;
    public float dashDuracion = 0.2f;
    public LayerMask CapaSuelo;

    // --- Variables privadas ---
    private bool enSuelo;
    private bool isDashing = false;
    private bool dashUsado = false;
    private float dashTiempoRestante = 0f;
    private Rigidbody2D rb;
    public Animator animator;
    private bool isAttacking = false;
    private float attackCooldown = 0.5f;
    private float lastAttackTime = -Mathf.Infinity;
    private float attackDuration = 0.6f;
    private float attackTimer = 0f;
    private bool attackKeyReleased = true;

    // --- Variables de aturdimiento ---
    private bool isHurt = false;
    private bool isStunned = false;
    private float stunTimer = 0f;


    // --- Inicialización ---
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // --- Lógica principal ---
    void Update()
    {
        // --- Movimiento, salto y suelo ---
        if (!isDashing)
        {
            MoverJugador();
            VerificarSuelo();
            Saltar();
        }

        // --- Dash ---
        Dash();

        // --- Ataque ---
        if (Input.GetKeyUp(KeyCode.C))
        {
            attackKeyReleased = true;
        }

        if (Input.GetKeyDown(KeyCode.C) && attackKeyReleased)
        {
            Atacar();
            attackKeyReleased = false;
        }

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                FinalizarAtaque();
            }
        }

        // --- Stun (aturdimiento) ---
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                isStunned = false;
            }
            return;
        }

        // --- Animaciones ---
        ActualizarAnimaciones();
    }

    // --- Movimiento horizontal ---
    private void MoverJugador()
    {
        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;

        if (velocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (velocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        transform.position += new Vector3(velocidadX, 0, 0);
    }

    // --- Verificar si está en el suelo ---
    private void VerificarSuelo()
    {
        Vector2 raycastOriginIzquierda = (Vector2)transform.position + new Vector2(-0.1f, 0);
        Vector2 raycastOriginDerecha = (Vector2)transform.position + new Vector2(0.1f, 0);

        RaycastHit2D hitIzquierda = Physics2D.Raycast(raycastOriginIzquierda, Vector2.down, longitudRaycast, CapaSuelo);
        RaycastHit2D hitDerecha = Physics2D.Raycast(raycastOriginDerecha, Vector2.down, longitudRaycast, CapaSuelo);

        enSuelo = hitIzquierda.collider != null || hitDerecha.collider != null;

        if (enSuelo)
        {
            dashUsado = false;
        }
    }

    // --- Salto ---
    private void Saltar()
    {
        if (enSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }
    }

    // --- Dash ---
    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && (enSuelo || !dashUsado))
        {
            isDashing = true;

            if (!enSuelo)
            {
                dashUsado = true;
            }

            dashTiempoRestante = dashDuracion;

            float dashDirection = transform.localScale.x;
            rb.linearVelocity = new Vector2(dashDirection * dashVelocidad, rb.linearVelocity.y);
        }

        if (isDashing)
        {
            dashTiempoRestante -= Time.deltaTime;
            if (dashTiempoRestante <= 0)
            {
                isDashing = false;
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    // --- Ataque ---
    private void Atacar()
    {
        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            attackTimer = 0f;
            lastAttackTime = Time.time;
        }
    }
    
    public void FinalizarAtaque()
    {
        isAttacking = false;
        attackTimer = 0f;
    }

    // --- Actualizar animaciones ---
    private void ActualizarAnimaciones()
    {
        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;
        animator.SetFloat("movement", Mathf.Abs(velocidadX * velocidad));
        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("fall", !enSuelo && rb.linearVelocity.y < 0);
        animator.SetBool("dash", isDashing && Mathf.Abs(velocidadX) < 0.01f);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isHurt", isHurt);
    }

    // --- Gizmos para depuración ---
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 raycastOriginIzquierda = (Vector2)transform.position + new Vector2(-0.1f, 0);
        Vector2 raycastOriginDerecha = (Vector2)transform.position + new Vector2(0.1f, 0);

        Gizmos.DrawLine(raycastOriginIzquierda, raycastOriginIzquierda + Vector2.down * longitudRaycast);
        Gizmos.DrawLine(raycastOriginDerecha, raycastOriginDerecha + Vector2.down * longitudRaycast);
    }

    // --- Daño ---
    public void RecibirDanio(int cantidad)
    {
        animator.SetBool("isHurt", true);
        Debug.Log("El jugador recibió " + cantidad + " de daño.");
    }

    private void ResetHurt()
    {
        animator.SetBool("isHurt", false);
    }

    // --- Stun (aturdimiento) ---
    public void Stunear(float duracion)
    {
        isStunned = true;
        stunTimer = duracion;
    }

    // --- Desactivar animación de daño ---
    public void DesactivaDano()
    {
        animator.SetBool("isHurt", false);
    }
}