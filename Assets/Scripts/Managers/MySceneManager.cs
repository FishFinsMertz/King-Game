using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance { get; private set; }

    [Header("Fade Settings")]
    public GameObject fadePanelPrefab; 
    public float fadeDuration;
    private CanvasGroup fadeCanvasGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Instantiate the fade panel prefab and get its CanvasGroup
        if (fadePanelPrefab != null)
        {
            GameObject fadePanel = Instantiate(fadePanelPrefab);
            DontDestroyOnLoad(fadePanel);
            fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();

            if (fadeCanvasGroup == null)
                Debug.LogWarning("Fade panel prefab is missing CanvasGroup component!");
        }
        else
        {
            Debug.LogWarning("Fade panel prefab not assigned!");
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(Fade(1f));

        // Load scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        yield return null;

        // Fade back in
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("No fade canvas group assigned.");
            yield break;
        }

        fadeCanvasGroup.blocksRaycasts = true;

        float startAlpha = fadeCanvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCanvasGroup.blocksRaycasts = targetAlpha != 0;
    }
}
