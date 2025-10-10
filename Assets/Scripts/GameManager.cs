using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameOver = false;
    private int enemiesKilled = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    public void PlayerDied()
    {
        isGameOver = true;
        Debug.Log("GAME OVER!");
        UIManager.instance.ShowGameOver();
        Cursor.visible = true;
        Time.timeScale = 0.1f; // Optional: Pause the game
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        UIManager.instance.UpdateEnemyKillUI(enemiesKilled);
    }
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); 
    }
}
