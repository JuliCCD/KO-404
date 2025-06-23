using UnityEngine;

public class OjoVolador : MonoBehaviour
{
    public float xMin = -27.84f;
    public float xMax = 27.84f;
    public float velocidad = 4f;
    public float velocidadExtra = 2f;
    public JefeFantasma jefe; // Arrastra el jefe desde el inspector
    private int direccion = 1; // 1 = derecha, -1 = izquierda

    private int velocidadAumentadaVeces = 0;
    private float vidaMaximaJefe;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (jefe == null)
            jefe = GameObject.FindObjectOfType<JefeFantasma>();
        if (jefe != null)
            vidaMaximaJefe = jefe.vida;
    }

    // Update is called once per frame
    void Update()
    {
        // Mover ojo volador en X
        transform.Translate(Vector2.right * direccion * velocidad * Time.deltaTime);

        // Cambiar dirección al llegar a los extremos
        if (transform.position.x >= xMax)
            direccion = -1;
        else if (transform.position.x <= xMin)
            direccion = 1;

        // Voltear sprite en X (opcional)
        Vector3 escala = transform.localScale;
        escala.x = direccion > 0 ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;

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
            Vector2 direccionDanio = (collision.transform.position - transform.position).normalized;
            collision.GetComponent<PlayerController>().RecibirDanio(direccionDanio, 1);
        }
    }
}
