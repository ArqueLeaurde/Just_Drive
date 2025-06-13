using TMPro; // TextMeshPro is used for better text rendering
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;
    public GameObject gameOverPanel;

    [Header("Game UI Elements")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI carsOvertakenText;
    public Slider jumpCooldownSlider;

    [Header("Game Over UI Elements")]
    public TextMeshProUGUI finalScoreText;

    // References
    private CarHandler playerCar;
    private Rigidbody playerRb;

    void Start()
    {
        // find layer car and its Rigidbody
        playerCar = GameManager.instance.playerCar;
        if (playerCar != null)
        {
            playerRb = playerCar.GetComponent<Rigidbody>();
        }

        // hide all panels at the start except the main menu
        mainMenuPanel.SetActive(true);
        gameUIPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (GameManager.instance.currentState == GameManager.GameState.Playing)
        {
            UpdateGameUI();
        }
    }

    private void UpdateGameUI()
    {
        if (playerCar == null || playerRb == null) return;

        // speed in km/h
        float speedKmh = playerRb.linearVelocity.z * 3.6f;
        speedText.text = $"{Mathf.FloorToInt(speedKmh)} km/h";

        // Distance
        distanceText.text = $"{Mathf.FloorToInt(GameManager.instance.distanceTraveled)} m";

        // cars overtaken
        carsOvertakenText.text = $"Cars overtaken : {GameManager.instance.carsOvertaken}";

        // jump cooldown slider
        if (playerCar.isJumpInCooldown)
        {
            jumpCooldownSlider.gameObject.SetActive(true);
            jumpCooldownSlider.value = playerCar.currentCooldownTimer / playerCar.jumpCooldown;
        }
        else
        {
            jumpCooldownSlider.gameObject.SetActive(false);
        }
    }

    public void ShowGameUI()
    {
        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameUIPanel.SetActive(true);
    }

    public void ShowGameOverScreen(int finalScore)
    {
        gameUIPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        finalScoreText.text = $"Final Score\n<size=120>{finalScore}</size>";
    }
}