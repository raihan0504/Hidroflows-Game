using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject selectLevel;
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject aboutUsPanel;
    [SerializeField] GameObject mainMenuPanel;


    private void Start()
    {
        selectLevel.SetActive(false);
        settingPanel.SetActive(false);
        aboutUsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Button Start Game
    #region Start Game
    public void OpenPanelLevel()
    {
        selectLevel.SetActive(true);
        mainMenuPanel.SetActive(false); 
    }

    public void StartGame()
    {
        GlobalManager.Instance.LoadGame();
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
