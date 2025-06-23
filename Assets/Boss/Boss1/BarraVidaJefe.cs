using UnityEngine;
using UnityEngine.UI;

public class BarraVidaJefe : MonoBehaviour
{
    public Image rellenoBarraVida;
    public JefeFantasma jefeFantasma;
    private float vidaMaxima;

    void Start()
    {
        jefeFantasma = GameObject.FindObjectOfType<JefeFantasma>();
        vidaMaxima = jefeFantasma.vida;
    }

    void Update()
    {
        rellenoBarraVida.fillAmount = jefeFantasma.vida / vidaMaxima;
    }
}
