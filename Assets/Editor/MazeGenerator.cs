using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MazeGenerator : EditorWindow
{
    private int width = 5;
    private int height = 5;

    public GameObject floorPrefab;
    public GameObject wallPrefab;

    public GameObject[,] mazeCells;
    public GameObject[,] contentCells;

    private Stack<(int, int)> visitedLocations = new Stack<(int, int)>();


    public bool[,] visitedCells;  //For the purpose of finding the path


    private GameObject mazeObject;

    public enum NeighbourDirection
    {
        UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3,
    }

    private NeighbourDirection activeDirection;

    [MenuItem("Custom Tools/Maze Generator")]
    public static void ShowWindow()
    {
        GetWindow<MazeGenerator>("Maze Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter the width and height for maze");

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        floorPrefab = EditorGUILayout.ObjectField("Floor prefab", floorPrefab, typeof(GameObject), true) as GameObject;
        wallPrefab = EditorGUILayout.ObjectField("Wall prefab", wallPrefab, typeof(GameObject), true) as GameObject;

        if (width > 1 && height > 1)
        {
            if (GUILayout.Button("Create Maze"))
            {
                CreateMaze(width, height);
            }

            if (GUILayout.Button("Destroy Maze"))
            {
                DestroyOldGrid();
            }
        }
    }

    private void DestroyOldGrid()
    {
        // Clear the existing grid if any
        GameObject existingGrid = GameObject.Find("Maze");
        if (existingGrid != null)
        {
            DestroyImmediate(existingGrid);
        }
    }

    private void CreateMaze(int gridWidth, int gridHeight)
    {

        DestroyOldGrid();

        mazeObject = new GameObject("Maze");

        GenerateBaseMazeSkeleton(gridWidth, gridHeight);

    }

    private void GenerateBaseMazeSkeleton(int gridWidth, int gridHeight)
    {
        mazeCells = new GameObject[gridWidth, gridHeight];
        contentCells = new GameObject[gridWidth, gridHeight];
        visitedCells = new bool[gridWidth, gridHeight];

        var dirValues = Enum.GetValues(typeof(NeighbourDirection));

        //Creates the base floor of grid based on width and height
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                //if (x == 0 || x == gridWidth - 1 || y == 0 || y == gridHeight - 1)
              
                //Instantiate maze floor
                GameObject cell = Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity);
                cell.transform.SetParent(mazeObject.transform);

                //Instaniate wall prefab on the same co-ordinates to be removed later
                GameObject wall = Instantiate(wallPrefab);
                wall.transform.SetParent(mazeObject.transform);
                wall.transform.position = new Vector3(x, 1.5f, y);

                contentCells[x, y] = wall;
                mazeCells[x, y] = cell;
                visitedCells[x, y] = false;
            }
        }

       MazeBacktracking();
    }

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

            float random = UnityEngine.Random.Range(0, 100);

            //if(random < 50)
            //{
            //    if (visitedLocations.Count > 1)
            //    {
            //        visitedLocations.Pop();
            //    }
            //}

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

    private void BacktrackingPreviousLocations(int direction)
    {
        //Remove the top from stack (i.e. Current)
        int random = UnityEngine.Random.Range(0, width / 3);

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
  

    private void MazeBacktracking()
    {
        int index = UnityEngine.Random.Range(0, 4);

        if (index == 4)
        {
            index = 3;
        }

        //For testing, let start be at 1,1
        RemoveWallsInDirection(1, 1, index);
    }


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
}
