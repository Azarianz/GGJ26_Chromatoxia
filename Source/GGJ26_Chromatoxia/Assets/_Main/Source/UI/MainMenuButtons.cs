using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public string bootstrapScene = "RunBootstrap";

    public void OnStartClicked()
    {
        LevelManager.startMode = BootstrapStartMode.Run;
        SceneManager.LoadScene(bootstrapScene, LoadSceneMode.Single);
    }

    public void OnTutorialClicked()
    {
        LevelManager.startMode = BootstrapStartMode.Tutorial;
        SceneManager.LoadScene(bootstrapScene, LoadSceneMode.Single);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}