using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Asegúrate de incluir esto para usar UI

public class JefeFantasma : MonoBehaviour
{
    public float rangoXMin = -10f;
    public float rangoXMax = 10f;
    public float posY = 2f;
    public float velocidad = 3f;
    public float tiempoVisible = 5f; // Ahora el jefe permanece más tiempo visible
    public float tiempoInvisible = 1.5f;
    public int vida = 10;

    private bool visible = true;
    private Vector3 destino;

    void Start()
    {
        AparecerEnRango();
        StartCoroutine(CicloAparicion());
    }

    void Update()
    {
        if (visible)
        {
            // Movimiento de izquierda a derecha
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

            // Si llegó al destino, elige el otro extremo y voltea el sprite
            if (Vector3.Distance(transform.position, destino) < 0.1f)
            {
                float nuevoX = destino.x == rangoXMin ? rangoXMax : rangoXMin;
                destino = new Vector3(nuevoX, posY, 0);

                // Voltear sprite en X según dirección
                Vector3 escala = transform.localScale;
                if (nuevoX < transform.position.x)
                    escala.x = -Mathf.Abs(escala.x); // Voltea a la izquierda
                else
                    escala.x = Mathf.Abs(escala.x);  // Voltea a la derecha
                transform.localScale = escala;
            }
        }
    }

    IEnumerator CicloAparicion()
    {
        while (vida > 0)
        {
            // Visible
            visible = true;
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
            yield return new WaitForSeconds(tiempoVisible);

            // Desaparece
            visible = false;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            yield return new WaitForSeconds(tiempoInvisible);

            // Aparece en nueva posición
            AparecerEnRango();
        }
    }

    void AparecerEnRango()
    {
        float x = Random.Range(rangoXMin, rangoXMax);
        transform.position = new Vector3(x, posY, 0);
        destino = new Vector3(x > (rangoXMin + rangoXMax) / 2 ? rangoXMin : rangoXMax, posY, 0);
    }

    // Recibe daño solo del jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (visible && collision.CompareTag("Espada"))
        {
            vida--;
            if (vida <= 0)
            {
                // Aquí puedes poner animación de muerte, sumar puntaje, etc.
                Destroy(gameObject);
            }
        }
    }
}

public class BarraVida : MonoBehaviour
{
    public JefeFantasma jefeFantasma;
    public Image rellenoBarraVida;
    public float vidaMaxima = 10f;

    void Update()
    {
        if (jefeFantasma != null)
            rellenoBarraVida.fillAmount = jefeFantasma.vida / vidaMaxima;
    }
}
