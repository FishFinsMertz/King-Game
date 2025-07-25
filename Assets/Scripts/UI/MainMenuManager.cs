using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        MySceneManager.Instance.LoadScene("OpeningScene");
    }
}
