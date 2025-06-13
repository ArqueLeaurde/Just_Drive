using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton pattern: allows easy access to the GameManager instance
    public static GameManager instance;

    public enum GameState { MainMenu, Playing, GameOver }
    public GameState currentState;

    [Header("Player & UI")]
    public CarHandler playerCar;
    public UIManager uiManager;

    [Header("Score")]
    public float distanceTraveled = 0f;
    public int carsOvertaken = 0;

    private float initialPlayerZ;

    [Header("Game Over")]
    [SerializeField] private float gameOverDelay = 3.5f;

    private void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // pause the game at the start and set the initial state
        currentState = GameState.MainMenu;
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        if (currentState == GameState.MainMenu)
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f; 
            uiManager.ShowGameUI(); // display the game UI

            if (playerCar != null)
            {
                initialPlayerZ = playerCar.transform.position.z;
            }
        }
    }

    private void Update()
    {
        if (currentState == GameState.Playing && playerCar != null)
        {
            // Check distance traveled
            distanceTraveled = playerCar.transform.position.z - initialPlayerZ;
        }
    }

    public void IncrementOvertakeCount()
    {
        if (currentState == GameState.Playing)
        {
            carsOvertaken++;
        }
    }

    public void PlayerDied()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.GameOver;
            
            StartCoroutine(GameOverSequence());
        }
    }

    // Game Over sequence coroutine
    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);
        Time.timeScale = 0f;

        int finalScore = Mathf.FloorToInt(distanceTraveled) + (carsOvertaken * 100);
       
        uiManager.ShowGameOverScreen(finalScore);
    }

    public void RestartGame()
    {
        // Time scale reset
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        RestartGame(); // Just restart the game scene
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game..."); 
        Application.Quit();

        //To make it work in unity editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}