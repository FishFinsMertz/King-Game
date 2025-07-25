using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TeleporterController : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
     public TextMeshProUGUI promptText;
     private bool isPlayerNear = false;
    private bool isLoading = false; // Flag to prevent multiple loads

    void Start()
    {
        if (promptText != null) {
            promptText.gameObject.SetActive(false); // Turn off text
        }
        else {
            Debug.Log("prompt text doesn't exist");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerNear && !isLoading && Input.GetKeyDown(KeyCode.X))
        {
            isLoading = true;
            MySceneManager.Instance.LoadScene(sceneToLoad);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        isPlayerNear = true;
        promptText.gameObject.SetActive(true);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerNear = false;
        promptText.gameObject.SetActive(false);
    }
}
