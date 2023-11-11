using UnityEngine;

public class MainMenuUI : MonoBehaviour
{

    public void OnStartGameButton()
    {
        GameSceneManager.instance.GoToGameScene();
    }

    public void OnEndGameButton()
    {
        Application.Quit();
    }
}
