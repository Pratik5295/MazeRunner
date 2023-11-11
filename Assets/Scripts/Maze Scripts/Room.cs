using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject door;

    public GameObject GetDoor() { return door; }    
}
