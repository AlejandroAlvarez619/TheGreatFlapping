using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior: MonoBehaviour 
{
    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Quit()
    {
        Application.Quit();
        // UnityEditor.EditorApplication.isPlaying = false;
    }

    public void Reset()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
