using UnityEngine;

public class HitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        OnHitBoxTriggered(other);
    }

    protected virtual void OnHitBoxTriggered(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //For this prototyping instance, we want to select game over if detected in hit box
            Debug.Log("Game  has now ended");

            
            if (GameManager.instance != null)
            {
                GameManager.instance.EndGame(false);
            }
        }
    }
}
