using UnityEngine;

public class CorredorEnemigo : MonoBehaviour
{
    public float xMin = -27.84f;
    public float xMax = 27.84f;
    public float velocidad = 5f;
    public float velocidadExtra = 3f;
    public JefeFantasma jefe; // Arrastra el jefe desde el inspector
    private int direccion = 1; // 1 = derecha, -1 = izquierda

    private int velocidadAumentadaVeces = 0;
    private float vidaMaximaJefe;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Opcional: buscar jefe automáticamente si no está asignado
        if (jefe == null)
            jefe = GameObject.FindObjectOfType<JefeFantasma>();
        if (jefe != null)
            vidaMaximaJefe = jefe.vida;
    }

    // Update is called once per frame
    void Update()
    {
        // Mover enemigo
        transform.Translate(Vector2.right * direccion * velocidad * Time.deltaTime);

        // Cambiar dirección al llegar a los extremos
        if (transform.position.x >= xMax)
        {
            direccion = -1;
            Vector3 escala = transform.localScale;
            escala.x = -Mathf.Abs(escala.x); // Voltea a la izquierda
            transform.localScale = escala;
        }
        else if (transform.position.x <= xMin)
        {
            direccion = 1;
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Abs(escala.x); // Voltea a la derecha
            transform.localScale = escala;
        }

        // Aumentar velocidad cada vez que el jefe pierde 25% de vida
        if (jefe != null && vidaMaximaJefe > 0)
        {
            float porcentajeVida = jefe.vida / vidaMaximaJefe;
            int aumentosNecesarios = 3 - Mathf.FloorToInt(porcentajeVida / 0.25f);
            while (velocidadAumentadaVeces < aumentosNecesarios)
            {
                velocidad += velocidadExtra;
                velocidadAumentadaVeces++;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Daño al jugador: dirección desde el enemigo hacia el jugador y cantidad de daño
            Vector2 direccion = (collision.transform.position - transform.position).normalized;
            collision.GetComponent<PlayerController>().RecibirDanio(direccion, 1);
        }
    }
}
