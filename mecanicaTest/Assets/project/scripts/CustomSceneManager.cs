using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    public void GoToNextScene()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings ? SceneManager.GetActiveScene().buildIndex + 1 : 0;
        SceneManager.LoadScene(nextScene);
    }
    public void GoToScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
