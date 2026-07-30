using UnityEngine;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private LevelManager levelManager;

    private void Start()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable =
                levelManager.IsLevelUnlocked(i + 1);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (!levelManager.IsLevelUnlocked(levelIndex))
            return;

        GlobalManager.Instance.LoadLevel(levelIndex);
    }
}