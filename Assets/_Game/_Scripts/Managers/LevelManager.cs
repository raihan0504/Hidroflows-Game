using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] int currentLevelIndex;

    private const string LEVEL_REACHED = "LevelReached";

    public int CurrentLevelIndex => currentLevelIndex;

    public void CompleteLevel()
    {
        int levelReached = PlayerPrefs.GetInt(LEVEL_REACHED, 1);

        if (currentLevelIndex >= levelReached)
        {
            PlayerPrefs.SetInt(LEVEL_REACHED, currentLevelIndex + 1);
            PlayerPrefs.Save();
        }
    }

    public int GetUnlockedLevels()
    {
        return PlayerPrefs.GetInt(LEVEL_REACHED, 1);
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex <= GetUnlockedLevels();
    }

    [ContextMenu("Reset Level Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LEVEL_REACHED);
        PlayerPrefs.Save();

        Debug.Log("Berhasil Reset Level");
    }

    public void RestartLevel()
    {
        GlobalManager.Instance.LoadLevel(currentLevelIndex);
    }

    public void NextLevel()
    {
        int nextLevel = currentLevelIndex + 1;
        if (nextLevel > SceneManager.sceneCountInBuildSettings - 1)
        {
            LoadMainMenu();
            return;
        }

        GlobalManager.Instance.LoadLevel(currentLevelIndex + 1);
    }

    public void LoadMainMenu()
    {
        GlobalManager.Instance.BackToMenu();
    }
}