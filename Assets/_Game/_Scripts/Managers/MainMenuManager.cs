using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject aboutUsPanel;

    // Button Start Game
    #region Start Game
    public void OpenStartPanel()
    {
        startPanel.SetActive(true);
    }

    public void StartGame()
    {
        GameManager.Instance.LoadGame();
    }
    #endregion

    // Button Settings
    #region Settings Panel
    public void OpenSetting()
    {
        settingPanel.SetActive(true);
    }

    public void CloseSetting()
    {
        settingPanel.SetActive(false);
    }
    #endregion 

    // Button About Us
    #region About Us
    public void OpenAboutUs()
    {
        aboutUsPanel.SetActive(true);
    }

    public void CloseAboutUs()
    {
        aboutUsPanel.SetActive(false);
    }
    #endregion
}
