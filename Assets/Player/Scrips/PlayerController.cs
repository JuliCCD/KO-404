using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f; // Velocidad de movimiento horizontal
    public float fuerzaSalto = 0.5f; // Fuerza del salto
    public float fuerzaRebote= 10f;
    public float longitudRaycast = 0.1f; // Longitud del raycast para detectar el suelo
    public float dashVelocidad = 10f; // Velocidad del dash
    public float dashDuracion = 0.2f; // Duración del dash
    public LayerMask CapaSuelo; // Capa que define qué es considerado suelo

    private bool enSuelo; // Indica si el jugador está en el suelo
    private bool recibiendoDanio;
    private bool isDashing = false; // Indica si el jugador está haciendo dash
    private bool dashUsado = false; // Indica si el dash ya fue usado en el aire
    private float dashTiempoRestante = 0f; // Tiempo restante del dash
    private Rigidbody2D rb; // Referencia al Rigidbody2D del jugador
    public Animator animator; // Referencia al Animator para manejar animaciones
    private bool isAttacking = false;
    private float attackCooldown = 0.5f; // Tiempo de cooldown en segundos
    private float lastAttackTime = -Mathf.Infinity; // Último tiempo de ataque
    private float attackDuration = 0.6f; // Duración de la animación de ataque

    private float attackTimer = 0f;
    private bool attackKeyReleased = true;
    private bool isStunned = false;
    private float stunTimer = 0f;

    public int vida  = 3; // Vida del jugador
    public bool muerto;
    public AudioClip sonidoSalto;
    public AudioClip sonidoAtaque;
    public AudioClip sonidoDash;
    public AudioClip sonidoDanio;
    public AudioClip sonidoMuerte; 
    public AudioClip sonidoCaminar;
    private AudioSource audioSource;
    private AudioSource audioSourceCaminar; // Canal exclusivo para caminar
    private AudioSource audioSourceDie;     // Canal exclusivo para muerte

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        audioSourceCaminar = gameObject.AddComponent<AudioSource>();
        audioSourceCaminar.playOnAwake = false;
        audioSourceCaminar.loop = true;
        audioSourceCaminar.volume = 1f;

        audioSourceDie = gameObject.AddComponent<AudioSource>();
        audioSourceDie.playOnAwake = false;
        audioSourceDie.loop = false;

        // PRUEBA: fuerza el sonido de caminar al iniciar
        if (sonidoCaminar != null)
        {
            audioSourceCaminar.clip = sonidoCaminar;
            audioSourceCaminar.Play();
        }
    }

    void Update()
    {
        // Si el jugador está muerto, detener el sonido de caminar si está sonando
        if (muerto && audioSourceCaminar.isPlaying)
        {
            audioSourceCaminar.Stop();
        }

        // Verificar si el jugador está muerto
        if (!muerto)
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

            // Detectar si la tecla C fue soltada
            if (Input.GetKeyUp(KeyCode.C))
            {
                attackKeyReleased = true;
            }

            // Solo atacar si la tecla fue soltada antes
            if (Input.GetKeyDown(KeyCode.C) && attackKeyReleased)
            {
                Atacar();
                attackKeyReleased = false;
            }

            // Si está atacando, cuenta el tiempo y termina el ataque cuando pase la duración
            if (isAttacking)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackDuration)
                {
                    FinalizarAtaque();
                }
            }

            // Controlar el stun
            if (isStunned)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0)
                {
                    isStunned = false;
                }
                return; // Si está stuneado, no puede moverse ni atacar
            }

            ActualizarAnimaciones();
        }

        
    }

    private void MoverJugador()
    {
        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;

        if (velocidadX < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (velocidadX > 0)
            transform.localScale = new Vector3(1, 1, 1);

        transform.position += new Vector3(velocidadX, 0, 0);

        // Sonido de caminar SOLO en su canal
        if (Mathf.Abs(velocidadX) > 0.01f && enSuelo && !isDashing && !isAttacking)
        {
            if (!audioSourceCaminar.isPlaying)
            {
                audioSourceCaminar.clip = sonidoCaminar;
                audioSourceCaminar.Play();
            }
        }
        else
        {
            if (audioSourceCaminar.isPlaying)
            {
                audioSourceCaminar.Stop();
            }
        }
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
        if (enSuelo && Input.GetKeyDown(KeyCode.Space)/*&& !recibiendoDanio*/)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            audioSource.PlayOneShot(sonidoSalto); // Reproducir sonido de salto
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
            audioSource.PlayOneShot(sonidoDash); // Reproducir sonido de dash
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

    private void Atacar()
    {
        // Solo atacar si ha pasado el cooldown
        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            attackTimer = 0f; 
            lastAttackTime = Time.time;
            audioSource.PlayOneShot(sonidoAtaque); // Reproducir sonido de ataque
        }
    }
    
    public void FinalizarAtaque()
    {
        isAttacking = false;
        attackTimer = 0f;
    }

    private void ActualizarAnimaciones()
    {
        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;
        animator.SetFloat("movement", Mathf.Abs(velocidadX * velocidad));
        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("isHurt", recibiendoDanio);

        // Animación de caída
        animator.SetBool("fall", !enSuelo && rb.linearVelocity.y < 0);

        // Animación de dash
        animator.SetBool("dash", isDashing && Mathf.Abs(velocidadX) < 0.01f);

        // Animación de ataque (opcional, si quieres un bool en vez de trigger)
        animator.SetBool("isAttacking", isAttacking);
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

    public void RecibirDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            vida -= cantDanio;
            if (vida <= 0)
            {
                muerto = true;
                animator.SetBool("die", true); 
                if (audioSourceCaminar.isPlaying)
                {
                    audioSourceCaminar.Stop(); // Detener sonido de caminar al morir
                }
            
                if (audioSourceDie != null && sonidoMuerte != null)
                {
                    audioSourceDie.clip = sonidoMuerte;
                    audioSourceDie.Play(); // Reproducir sonido de muerte en su canal
                    LlamarGameOverMenu();
                }
                
            }
            if (!muerto)
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                audioSource.PlayOneShot(sonidoDanio); // Reproducir sonido de daño
            }
        }
    }

    public void Stunear(float duracion)
    {
        isStunned = true;
        stunTimer = duracion;
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void LlamarGameOverMenu()
    {
        var gameOverMenu = FindFirstObjectByType<GameOverMenu>();
        if (gameOverMenu != null)
        {
            gameOverMenu.MostrarGameOver();
        }
    }
}