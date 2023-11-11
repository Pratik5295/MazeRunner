using System;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    public static PlayerHealthSystem instance;

    public MazeGeneratorData mazeGeneratorData;

    [SerializeField] private int health;

    public Action<int> OnHealthUpdatedEvent;

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

    public void Start()
    {
        health = mazeGeneratorData.numberOfTraps;
    }

    public void DecreaseHealth()
    {
        if (health > 0)
        {
            health--;

            OnHealthUpdatedEvent?.Invoke(health);

            if(health <= 0)
            {
                GameManager.instance.EndGame(false);
            }
        }
    }
}
