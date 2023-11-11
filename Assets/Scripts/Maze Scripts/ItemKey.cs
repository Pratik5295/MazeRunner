using UnityEngine;

/// <summary>
/// The key item, on pick up opens/unlocks a door
/// </summary>
public class ItemKey : PickUp
{
    [SerializeField] private GameObject door;

    public void SetDoor(GameObject _door)
    {
        door = _door;
    }
    protected override void OnPickUp()
    {
        if (door != null)
        {
            door.SetActive(false);
        }
        base.OnPickUp();
    }
}
