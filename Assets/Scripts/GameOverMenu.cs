using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject panelGameOver;
    private bool gameOverActivo = false;

    public void MostrarGameOver()
    {
        Debug.Log("Mostrando Game Over");
        panelGameOver.SetActive(true);
        gameOverActivo = true;
    }

    void Update()
    {
        if (gameOverActivo && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f; // Reanuda el juego
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
