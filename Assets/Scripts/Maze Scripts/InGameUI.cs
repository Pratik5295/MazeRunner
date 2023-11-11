using TMPro;
using UnityEngine;

[DefaultExecutionOrder(3)]
public class InGameUI : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI gameWonText;

    public GameObject gameOverPanel;

    public TextMeshProUGUI healthText;


    public void Start()
    {
        if(PlayerHealthSystem.instance != null)
        {
           PlayerHealthSystem.instance.OnHealthUpdatedEvent += UpdateScoreText;

            UpdateScoreText(PlayerHealthSystem.instance.GetHealth());
        }
    }

    private void OnDestroy()
    {
        if (PlayerHealthSystem.instance != null)
        {
            PlayerHealthSystem.instance.OnHealthUpdatedEvent -= UpdateScoreText;
        }
    }
    public void OnRestartButtonClicked()
    {
        GameSceneManager.instance.GoToGameScene();
    }

    public void OnMenuButtonClicked()
    {
        GameSceneManager.instance.GoToGameMenu();
    }

    public void ShowGameOverPanel(bool hasWon)
    {
        if (hasWon)
        {
            gameWonText.gameObject.SetActive(true);
            gameOverText.gameObject.SetActive(false);
        }
        else
        {
            gameWonText.gameObject.SetActive(false);
            gameOverText.gameObject.SetActive(true);
        }

        gameOverPanel.SetActive(true);
    }

    public void UpdateScoreText(int value)
    {
        healthText.text = value.ToString();
    }
}
