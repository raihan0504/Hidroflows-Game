using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;

    private bool isPause = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel();
        }
    }

    public void PausePanel()
    {
        isPause = !isPause;
        pausePanel.SetActive(isPause);
        Time.timeScale = isPause ? 0f : 1f;
    }
}
