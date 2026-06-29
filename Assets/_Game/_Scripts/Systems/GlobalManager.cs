using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Loading Settings")]
    [SerializeField] private float progressSpeed = 0.5f;

    private int currentScene = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.LoadSceneAsync((int)SceneIndex.MAINMENU, LoadSceneMode.Additive);
        currentScene = (int)SceneIndex.MAINMENU;
    }

    public void LoadGame()
    {
        StartCoroutine(LoadSceneAsync((int)SceneIndex.LEVEL1));
    }

    public void LoadLevel(int levelIndex)
    {
        StartCoroutine(LoadSceneAsync(levelIndex));
    }

    public void RestartCurrentLevel()
    {
        if (currentScene != -1)
        {
            StartCoroutine(LoadSceneAsync(currentScene));
        }
    }

    public void BackToMenu()
    {
        StartCoroutine(LoadSceneAsync((int)SceneIndex.MAINMENU));
    }

    private IEnumerator LoadSceneAsync(int targetScene)
    {
        loadingScreen.SetActive(true);

        progressBar.value = 0;
        loadingText.text = "Loading... 0%";

        // Unload scene lama
        if (currentScene != -1 &&
            SceneManager.GetSceneByBuildIndex(currentScene).isLoaded)
        {
            AsyncOperation unloadOp =
                SceneManager.UnloadSceneAsync(currentScene);

            while (!unloadOp.isDone)
            {
                yield return null;
            }
        }

        // Load scene baru (Additive)
        AsyncOperation loadOp =
            SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        loadOp.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (!loadOp.isDone)
        {
            // Progress asli Unity (0 - 0.9)
            float targetProgress =
                Mathf.Clamp01(loadOp.progress / 0.9f);

            // Batasi visual sampai 90%
            targetProgress *= 0.9f;

            displayedProgress = Mathf.MoveTowards(
                displayedProgress,
                targetProgress,
                progressSpeed * Time.deltaTime
            );

            progressBar.value = displayedProgress;
            loadingText.text =
                $"Loading... {(displayedProgress * 100f):F0}%";

            // Ketika scene selesai load di background
            if (loadOp.progress >= 0.9f)
            {
                // Animasi 90 -> 100
                while (displayedProgress < 1f)
                {
                    displayedProgress = Mathf.MoveTowards(
                        displayedProgress,
                        1f,
                        progressSpeed * Time.deltaTime
                    );

                    progressBar.value = displayedProgress;
                    loadingText.text =
                        $"Loading... {(displayedProgress * 100f):F0}%";

                    yield return null;
                }

                yield return new WaitForSeconds(0.2f);

                loadOp.allowSceneActivation = true;
            }

            yield return null;
        }

        Scene loadedScene =
            SceneManager.GetSceneByBuildIndex(targetScene);

        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
        }

        currentScene = targetScene;

        loadingScreen.SetActive(false);
    }
}