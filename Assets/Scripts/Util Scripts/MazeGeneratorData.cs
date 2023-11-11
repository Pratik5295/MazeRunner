using UnityEngine;

[CreateAssetMenu(fileName = "Maze Generator Data",menuName = "Maze Generator Data")]
public class MazeGeneratorData : ScriptableObject
{
    [Header("Maze size: Width and Height")]
    [Range(50,Mathf.Infinity)]
    public int width = 50;
    [Range(50, Mathf.Infinity)]
    public int height = 50;

    [Space(10)]
    [Header("Prefabs Sections")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject trapPrefab;
    public GameObject enemyPrefab;
    public GameObject keyPrefab;
    public GameObject roomPrefab;
    public GameObject trophyPrefab;

    [Space(10)]
    [Range(1,10)]
    [Header("Props section")]
    public int numberOfTraps;

    [Range(1, 5)]
    public int numberOfEnemies;
}
