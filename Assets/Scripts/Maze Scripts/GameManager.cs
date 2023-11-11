using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CameraShake cameraShake;

    public InGameUI gameUI;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EndGame(bool hasWon)
    {
        Debug.Log("Game has ended");

        //Time.timeScale = 0.0f;
        Cursor.visible = true;
        gameUI.ShowGameOverPanel(hasWon);
    }

    public void CameraShake()
    {
        Debug.Log("Shaking camera");
        cameraShake.ShakeCamera(5f,0.5f);
    }
}
