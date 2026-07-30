using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;

    [Header("Win Panel")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button restartButton;
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [Header("HUD")]
    [SerializeField] private TMP_Text countdownText;

    private GameState prevState = GameState.Loading;

    private bool isPause = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel();
        }

        // Check for win state transition and show win panel once
        if (GameManager.Instance != null)
        {
            GameState s = GameManager.Instance.CurrentState;
            if (s != prevState)
            {
                if (s == GameState.Win)
                {
                    ShowWinPanel();
                }

                if (s == GameState.Lose)
                {
                    ShowGameOverPanel();
                }

                prevState = s;
            }
        }

        // Update countdown text (TextMeshPro) if assigned
        if (countdownText != null && GameManager.Instance != null)
        {
            float t = GameManager.Instance.CurrentTime;
            if (t < 0f) t = 0f;
            int minutes = (int)(t / 60f);
            int seconds = (int)(t % 60f);
            countdownText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    public void PausePanel()
    {
        isPause = !isPause;
        pausePanel.SetActive(isPause);
        Time.timeScale = isPause ? 0f : 1f;
    }

    private void Start()
    {

        // Ensure win panel is hidden at start
        if (winPanel != null)
            winPanel.SetActive(false);

        if (restartButton != null)
            restartButton.interactable = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void ShowWinPanel()
    {
        if (winPanel == null)
            return;

        // Activate panel
        Time.timeScale = 0f; // Pause the game
        winPanel.SetActive(true);

        // Enable restart button
        if (restartButton != null)
            restartButton.interactable = true;

    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game before restarting
        GlobalManager.Instance.RestartCurrentLevel();
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanel == null)
            return;

        Time.timeScale = 0f; // pause
        gameOverPanel.SetActive(true);

        if (restartButton != null)
            restartButton.interactable = true;
    }
}
