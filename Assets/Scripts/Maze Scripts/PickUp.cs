using UnityEngine;

public class PickUp : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            OnPickUp();
        }
    }

    protected virtual void OnPickUp()
    {
        Destroy(gameObject);
    }
}
