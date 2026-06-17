using System.Collections;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] Slider progresBar;
    [SerializeField] float fakeLoadingTime = 2f;

    IEnumerator LoadingMainAsync()
    {
        yield return new WaitForSeconds(fakeLoadingTime);
        AsyncOperation operation = SceneManager.LoadSceneAsync("");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progresBar.value = progress;
            loadingText.text = $"Loading... {progress * 100:F0}%";
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        } 
    }
}
