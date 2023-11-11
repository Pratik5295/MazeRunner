using UnityEngine;

public class Trap : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ActivateTrap();
        }
    }

    private void ActivateTrap()
    {
        Debug.Log("Damage player");

        if (GameManager.instance != null)
        {
            GameManager.instance.CameraShake();
        }

        if(PlayerHealthSystem.instance != null)
        {
            PlayerHealthSystem.instance.DecreaseHealth();
        }
    }
}
