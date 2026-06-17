using UnityEngine;
using TMPro;

public class GraphicSettings : MonoBehaviour
{
    [Header("Dropdown")]
    [SerializeField] TMP_Dropdown fpsDropdown;
    [SerializeField] TMP_Dropdown qualityDropdown;

    private const string FPS_KEY = "FPS_INDEX";
    private const string QUALITY_KEY = "QUALITY_INDEX";

    private void Start()
    {
        QualitySettings.vSyncCount = 0;

        // FPS
        int fpsIndex = PlayerPrefs.GetInt(FPS_KEY, 1);
        fpsDropdown.SetValueWithoutNotify(fpsIndex);
        ApplyFPS(fpsIndex);

        // Quality
        int qualityIndex = PlayerPrefs.GetInt(QUALITY_KEY, 2);
        qualityDropdown.SetValueWithoutNotify(qualityIndex);
        ApplyQuality(qualityIndex);
    }

    public void OnFpsChanged(int index)
    {
        PlayerPrefs.SetInt(FPS_KEY, index);
        PlayerPrefs.Save();

        ApplyFPS(index);
    }

    public void OnQualityChanged(int index)
    {
        PlayerPrefs.SetInt(QUALITY_KEY, index);
        PlayerPrefs.Save();

        ApplyQuality(index);
    }

    private void ApplyFPS(int index)
    {
        switch (index)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;

            case 1:
                Application.targetFrameRate = 60;
                break;

            case 2:
                Application.targetFrameRate = 120;
                break;
        }
    }

    private void ApplyQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}