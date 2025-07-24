using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TeleporterController : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
     public TextMeshProUGUI promptText;
     private bool isPlayerNear = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        if (isPlayerNear && Input.GetKeyDown(KeyCode.X)) {
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
