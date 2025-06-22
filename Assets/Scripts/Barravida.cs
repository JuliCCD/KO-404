using UnityEngine;
using UnityEngine.UI; 

public class Barravida : MonoBehaviour
{
    public Image rellenoBarraVida; 
    private PlayerController playerController; 
    private float vidaMaxima;
    
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        vidaMaxima = playerController.vida; 
    }

    // Update is called once per frame
    void Update()
    {
        rellenoBarraVida.fillAmount = playerController.vida / vidaMaxima;
    }
}
