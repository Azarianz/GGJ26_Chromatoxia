using UnityEngine;

public class BootstrapAutoStart : MonoBehaviour
{
    void Start()
    {
        if (LevelManager.startMode == BootstrapStartMode.Tutorial)
            LevelManager.I.StartTutorial();
        else
            LevelManager.I.StartRun();
    }
}