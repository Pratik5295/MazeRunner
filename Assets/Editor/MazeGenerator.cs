using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class MazeGenerator : EditorWindow
{
    private int width;
    private int height;

    private int numberOfTraps;
    private int numberOfEnemies;

    [Tooltip("Enter the scriptable object data of type Maze Generator Data")]
    public MazeGeneratorData mazeData;

    [Space(10)]
    [Header("Prefabs Section")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject trapPrefab;
    public GameObject enemyPrefab;
    public GameObject keyPrefab;
    public GameObject trophyPrefab;

    //The ability to place custom rooms
    public GameObject roomPrefab;

    public GameObject[,] floorCells;
    public GameObject[,] contentCells;
    public List<GameObject> walkableCells;

    private List<GameObject> doors;

    private Stack<(int, int)> visitedLocations = new Stack<(int, int)>();


    public bool[,] visitedCells;  //For the purpose of finding the path

    // Parent game objects for all maze sub-objects
    private GameObject mazeObject;
    private GameObject outerWalls;
    private GameObject propsObject;


    [MenuItem("Custom Tools/Maze Generator")]
    public static void ShowWindow()
    {
        GetWindow<MazeGenerator>("Maze Generator");
    }

    private void OnGUI()
    {
        GUILayout.Space(30);
        GUILayout.Label("Add Maze Data Scriptable Object");
        mazeData = EditorGUILayout.ObjectField("Maze Data", mazeData,typeof(MazeGeneratorData), false) as MazeGeneratorData;

        if (mazeData == null)
        {

            GUILayout.Label("Please add scriptable object data to use the tool further");
        }
        else
        {
            width = mazeData.width;
            height = mazeData.height;

            floorPrefab = mazeData.floorPrefab;
            wallPrefab = mazeData.wallPrefab;
            trapPrefab = mazeData.trapPrefab;

            roomPrefab = mazeData.roomPrefab;

            enemyPrefab = mazeData.enemyPrefab;
            keyPrefab = mazeData.keyPrefab;

            trophyPrefab = mazeData.trophyPrefab;

            if (width >= 50 && height >= 50)
            {
                GUILayout.Space(30);
                numberOfTraps = mazeData.numberOfTraps;
                numberOfEnemies = mazeData.numberOfEnemies;
                if (GUILayout.Button("Create Maze"))
                {
                    Debug.Log($"Size: {width} and {height}");
                    CreateMaze(width, height);
                }

                GUILayout.Space(30);
                if (GUILayout.Button("Destroy Maze"))
                {
                    DestroyOldGrid();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Please keep the width and height to atleast 50");
            }
        }
    }

    /// <summary>
    /// Destroy old/existing maze and related game Objects
    /// </summary>
    private void DestroyOldGrid()
    {
        // Clear the existing grid if any
        GameObject existingGrid = GameObject.Find("Maze");
        if (existingGrid != null)
        {
            DestroyImmediate(existingGrid);
        }

        GameObject enemyParent = GameObject.Find("EnemyParent");
        if (enemyParent != null)
        {
            DestroyImmediate(enemyParent);
        }

        GameObject outerWallParent = GameObject.Find("OuterWalls");
        if (outerWallParent != null)
        {
            DestroyImmediate(outerWallParent);
        }

        GameObject props = GameObject.Find("PropsParent");

        if(props != null)
        {
            DestroyImmediate(props);
        }

        if (doors != null)
        {
            doors.Clear();
        }
    }

    /// <summary>
    /// Create the maze based on width and height entered
    /// </summary>
    /// <param name="gridWidth">Width of the maze</param>
    /// <param name="gridHeight">Height of the maze</param>
    private void CreateMaze(int gridWidth, int gridHeight)
    {

        DestroyOldGrid();

        mazeObject = new GameObject("Maze");

        propsObject = new GameObject("PropsParent");

        doors = new List<GameObject>();

        GenerateBaseMazeSkeleton(gridWidth, gridHeight);


    }

    /// <summary>
    /// Generate the base skeleton using walls and floor prefab
    /// </summary>
    /// <param name="gridWidth"></param>
    /// <param name="gridHeight"></param>
    private void GenerateBaseMazeSkeleton(int gridWidth, int gridHeight)
    {
        floorCells = new GameObject[gridWidth, gridHeight];
        contentCells = new GameObject[gridWidth, gridHeight];
        visitedCells = new bool[gridWidth, gridHeight];
        walkableCells = new List<GameObject>();

        //Creates the base floor of grid based on width and height
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                //Instantiate maze floor
                GameObject cell = Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity);
                cell.transform.SetParent(mazeObject.transform);
               

                //Instaniate wall prefab on the same co-ordinates to be removed later
                GameObject wall = Instantiate(wallPrefab);
                wall.transform.SetParent(mazeObject.transform);
                wall.transform.position = new Vector3(x, 1.5f, y);

                contentCells[x, y] = wall;
                floorCells[x, y] = cell;
                visitedCells[x, y] = false;
            }
        }

        GenerateOuterWalls();
        PopulateRoom();
        MazeBacktracking();

        //Populate Props
        PopulateAllKeys();
        PopulateTraps();
        //PopulateTrophy();
    }

    #region Outer Walls Generation
    /// <summary>
    /// Generate outer walls of the grid
    /// </summary>
    private void GenerateOuterWalls()
    {
        outerWalls = new GameObject("OuterWalls");
        LeftSideOuterWalls();
        RightSideOuterWalls();
        TopSideOuterWalls();
        BottomSideOuterWalls();
    }

    /// <summary>
    /// Left side outer walls
    /// </summary>
    private void LeftSideOuterWalls()
    {
        Vector3 wallScale = new Vector3(1.0f, 5.0f, 1.0f); // Adjust scale as needed
        //Wrap around height, so use height parameter
        Vector3 startPosition = new Vector3(-1.0f, 0.0f, 0.0f);
        for (int i = -1; i <= height; i++)
        {
            InstantiateWall(new Vector3(startPosition.x, startPosition.y, i + startPosition.z), wallScale);
        }
    }


    /// <summary>
    /// Right side outer walls
    /// </summary>
    private void RightSideOuterWalls()
    {
        Vector3 wallScale = new Vector3(1.0f, 5.0f, 1.0f); // Adjust scale as needed
        //Wrap around height, so use height parameter
        Vector3 startPosition = new Vector3(width, 0.0f, 0.0f);
        for (int i = -1; i <= height; i++)
        {
            InstantiateWall(new Vector3(startPosition.x, startPosition.y, i + startPosition.z), wallScale);
        }
    }


    /// <summary>
    /// Top side outer walls
    /// </summary>
    private void TopSideOuterWalls()
    {
        Vector3 wallScale = new Vector3(1.0f, 5.0f, 1.0f); // Adjust scale as needed
        //Wrap around height, so use height parameter
        Vector3 startPosition = new Vector3(-1.0f, 0.0f, height);
        for (int i = 0; i <= width; i++)
        {
            InstantiateWall(new Vector3(i + startPosition.x, startPosition.y, startPosition.z), wallScale);
        }
    }

    /// <summary>
    /// Bottom side outer walls
    /// </summary>
    private void BottomSideOuterWalls()
    {
        Vector3 wallScale = new Vector3(1.0f, 5.0f, 1.0f); // Adjust scale as needed
        //Wrap around height, so use height parameter
        Vector3 startPosition = new Vector3(0f, 0.0f, -1.0f);
        for (int i = -1; i < width; i++)
        {
            InstantiateWall(new Vector3(i + startPosition.x, startPosition.y,startPosition.z), wallScale);
        }
    }

    /// <summary>
    /// Generic wall generation function with position and scale specified
    /// </summary>
    /// <param name="position">Position of the wall</param>
    /// <param name="scale">Scale of the wall</param>
    private void InstantiateWall(Vector3 position, Vector3 scale)
    {
        // Instantiate a wall prefab at the specified position and scale
        GameObject outerWall = Instantiate(wallPrefab, position, Quaternion.identity);
        outerWall.transform.localScale = scale;

        if (outerWalls != null)
        {
            outerWall.transform.SetParent(outerWalls.transform);
        }
    }

    #endregion

    #region Populate Props

    /// <summary>
    /// Populate the trophy on any walkable tile in game
    /// </summary>
    private void PopulateTrophy()
    {
        if (trophyPrefab == null) return; 

        System.Random random = new System.Random();

        int randomCell = random.Next(0, walkableCells.Count);
        var trophyCell = walkableCells[randomCell];

        GameObject trophy = Instantiate(trophyPrefab,
         new Vector3(trophyCell.transform.position.x, 1f, trophyCell.transform.position.z),
         Quaternion.identity) as GameObject;

        trophy.transform.SetParent(propsObject.transform);
    }

    /// <summary>
    /// Populate a room, clear the walls in maze for the same
    /// </summary>
    private void PopulateRoom()
    {
        if (roomPrefab == null) return;

        int randX = UnityEngine.Random.Range(0, width - 1);
        int randY = UnityEngine.Random.Range(0, height - 1);

        int roomWidth = roomPrefab.GetComponent<Placeable>().width;
        int roomHeight = roomPrefab.GetComponent<Placeable>().height; 
        
        //These rooms can be placed in the level
        if(roomWidth < width && roomHeight < height)
        {
            //Check if the room can fit in horizontally
            float fitWidth = (roomWidth / 2) + randX;
            float fitHeight = (roomHeight / 2) + randY;

            if ((randX -(roomWidth / 2) > 1 && fitWidth < width - 1) 
                &&
                (randY -(roomHeight / 2) > 1 && fitHeight < height - 1))
            {

                GameObject room = Instantiate(roomPrefab,new Vector3(randX,1f,randY), Quaternion.identity);
                room.transform.SetParent(mazeObject.transform);

                GameObject door = room.GetComponent<Room>().GetDoor();
                RemoveWallsInRange(randX,randY,roomWidth,roomHeight);

                doors.Add(door);
                Debug.Log($"Adding door {door}");
            }
            else
            {
                PopulateRoom();
            }
        }
    }

    /// <summary>
    /// Populate all keys and assign doors to each key
    /// </summary>
    private void PopulateAllKeys()
    {
        if (keyPrefab == null) return;

        if (doors.Count > 0)
        {
            for(int i = 0; i < doors.Count; i++)
            {
               PopulateKey(doors[i]);
            }
        }
    }
    /// <summary>
    /// Populate the keys in the maze when a room is added
    /// </summary>
    /// <param name="door"></param>
    private void PopulateKey(GameObject door)
    {
        if(door != null)
        {
            System.Random random = new System.Random();

            int randomCell = random.Next(5, walkableCells.Count);
            var keyCell = walkableCells[randomCell];

            GameObject key = Instantiate(keyPrefab,
           new Vector3(keyCell.transform.position.x, 1f, keyCell.transform.position.z),
           Quaternion.identity) as GameObject;

           key.transform.SetParent(propsObject.transform);


          key.GetComponent<ItemKey>().SetDoor(door);
        }
    }


    /// <summary>
    /// Helper function to populate traps in the scene
    /// </summary>
    private void PopulateTraps()
    {
        if (trapPrefab == null) return;

        if (numberOfTraps > 0 && walkableCells.Count > 0)
        {
            System.Random random = new System.Random();

            for (int i = 0; i < numberOfTraps; i++)
            {
                int randomCell = random.Next(5, walkableCells.Count);
                var trapCell = walkableCells[randomCell];

                GameObject trap = Instantiate(trapPrefab,
                 new Vector3(trapCell.transform.position.x, 1f, trapCell.transform.position.z),
                 Quaternion.identity) as GameObject;

                trap.transform.SetParent(propsObject.transform);
            }
        }
    }
    #endregion

    #region Maze Backtracking Section
    /// <summary>
    /// Removing walls in a given Range
    /// </summary>
    /// <param name="startX">Starting X position</param>
    /// <param name="startY">Starting "Z" position</param>
    /// <param name="rangeX">Range of removing walls to be removed in x value of grid</param>
    /// <param name="rangeY">Range of removing walls to be removed in y value of grid</param>
    /// <param name="offsetX">Extra rows to be removed (for creating a walkable path)</param>
    /// <param name="offsetY">Extra columns to be removed (for creating a walkable path)</param>
    private void RemoveWallsInRange(int startX, int startY, int rangeX, int rangeY, int offsetX = 2, int offsetY = 2)
    {
        visitedCells[startX, startY] = true;

        int rightLength = startX + (rangeX / 2) + offsetX;
        int leftLength = startX - (rangeX / 2) - offsetX;

        int upSize = startY + (rangeY / 2) + offsetY;
        int downSize = startY - (rangeY / 2) - offsetY;

        for(int i = leftLength; i <= rightLength; i++)
        {
            for(int j = downSize; j <= upSize; j++)
            {
                if (i < width && j < height)
                {
                    if (contentCells[i, j] != null)
                    {
                        GameObject wallToRemove = contentCells[i, j];
                        DestroyImmediate(wallToRemove);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Remove walls in one particular direction
    /// </summary>
    /// <param name="x">X Content cell value</param>
    /// <param name="y">Y Content cell value</param>
    /// <param name="direction">0,1,2,3 direction</param>
    private void RemoveWallsInDirection(int x, int y, int direction)
    {
        //Current cell is visited
        if (visitedCells[x,y] == false)
        {
            visitedCells[x, y] = true;

            //Added to visited locations
            visitedLocations.Push((x, y));
        }
        else
        {
            //It already exists and is a recursion

        }
        

        Tuple<int, int> next = GetNextDirectionCell(direction, x, y);

        if (next.Item1 < 0 || next.Item1 >= width - 1)
        {
            //change direction to up or down
            direction = GetRandomBetween(0,1);

            RemoveWallsInDirection(x, y, direction);
        }
        else if (next.Item2 < 0 || next.Item2 >= height - 1)
        {
            //Change direction to left or right
            direction = GetRandomBetween(2,3);

            RemoveWallsInDirection(x, y, direction);
        }

        else
        {
           // Remove the wall between the current cell and the next cell
           GameObject wallToRemove = contentCells[next.Item1, next.Item2];

           if (visitedCells[next.Item1, next.Item2] == false)
           {
                DestroyImmediate(wallToRemove);

                GameObject correspondingFloorCell = floorCells[next.Item1,next.Item2];
                walkableCells.Add(correspondingFloorCell);

                RemoveWallsInDirection(next.Item1, next.Item2, direction);
           }
           else
           {
              if (visitedLocations.Count > 1)
              {
                BacktrackingPreviousLocations(direction);
              }
              else
              {
                //Nothing to display, end the algorithm
                Debug.Log("Algorithm ended here");
              }
           }
        }
    }

    /// <summary>
    /// Back track to previous processed cells to change the direction of algorithm
    /// </summary>
    /// <param name="direction"></param>
    private void BacktrackingPreviousLocations(int direction)
    {
        //Remove the top from stack (i.e. Current)
        int random = UnityEngine.Random.Range(0, width / 4);

        for(int i = 0; i < random; i++)
        {
            if(visitedLocations.Count > 0)
            {
                visitedLocations.Pop();
            }
            else
            {
                break;
            }
        }

        if (visitedLocations.Count > 0)
        {
            //Peek at the the previous or second to last element
            var previous = visitedLocations.Peek();
            //Put previous under recursive

            //If up down, then just switch to right left and vice versa
            if (direction == 0 || direction == 1)
            {
                direction = GetRandomBetween(2, 3);
            }
            else
            {
                direction = GetRandomBetween(0, 1);
            }
            RemoveWallsInDirection(previous.Item1, previous.Item2, direction);
        }
    }


    /// <summary>
    /// Returns the neighbour cell based on direction
    /// 0 is Up
    /// 1 is Down
    /// 2 is Right
    /// 3 is  Left
    /// </summary>
    /// <param name="index"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private Tuple<int, int> GetNextDirectionCell(int index, int x, int y)
    {
        int newX = x, newY = y;
        switch (index)
        {
            //UP
            case 0:
                newY = newY + 1;
                break;

            //DOWN
            case 1:
                newY = newY - 1;
                break;

            //RIGHT
            case 2:
                newX = newX - 1;
                break;

            //LEFT
            case 3:
                newX = newX + 1;
                break;
        }


        Tuple<int, int> result = Tuple.Create(newX, newY);
        return result;
    }

    /// <summary>
    /// Recursive Maze Back Tracking
    /// </summary>
    private void MazeBacktracking()
    {
        int index = UnityEngine.Random.Range(0, 4);

        if (index == 4)
        {
            index = 3;
        }

        //For testing, let start be at 1,1
        RemoveWallsInDirection(1, 1, index);


        //Remove the wall from first cell
        GameObject wallToRemove = contentCells[1, 1];
        DestroyImmediate(wallToRemove);

        GameObject floor = floorCells[1, 1];
        walkableCells.Add(floor);

        Debug.Log("Completed all backtracking and maze generation");

        HandleMazeNavigation();
    }

    #endregion



    #region AI and Navigation Section
    /// <summary>
    /// Helper function to handle maze navigation
    /// </summary>
    private void HandleMazeNavigation()
    {
        GameObjectUtility.SetStaticEditorFlags(mazeObject, StaticEditorFlags.NavigationStatic);
        

        foreach(var cell in walkableCells)
        {
            GameObjectUtility.SetStaticEditorFlags(cell, StaticEditorFlags.NavigationStatic);
        }
        //NavMeshSurface navMeshSurface = mazeObject.AddComponent<NavMeshSurface>();

        //navMeshSurface.BuildNavMesh();

        HandleEnemies();

    }

    /// <summary>
    /// Helper function to create enemies and add 
    /// </summary>
    private void HandleEnemies()
    {
        GameObject enemyParent = new GameObject("EnemyParent");

        for (int i = 0; i < numberOfEnemies; i++)
        {
            System.Random random = new System.Random();

            int randomValue = random.Next(0, walkableCells.Count);
            var enemyCell = walkableCells[randomValue];
            GameObject enemyAI = Instantiate(enemyPrefab,
                new Vector3(enemyCell.transform.position.x, 0.1f, enemyCell.transform.position.z),
                Quaternion.identity) as GameObject;

            enemyAI.transform.SetParent(enemyParent.transform);

            int destinationValue = random.Next(0, walkableCells.Count);
            var destinationCell = walkableCells[destinationValue];

            enemyAI.GetComponent<EnemyAI>().SetTarget(destinationCell);
        }
    }



    #endregion

    #region Utility
    /// <summary>
    /// Helper function to return a random number between two numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int GetRandomBetween(int a, int b)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(2); // Generates 0 or 1

        int selectedValue = randomIndex == 0 ? a : b;
        return selectedValue;
    }

    #endregion
}
