using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanalSystem : MonoBehaviour
{   
    private const int WATER_SOURCE = 4;
    private const int ROOT_GOAL = 5;
    private const int EMPTY_BLOCK = 6;

    private const int STRAIGHT_BLOCK = 1;
    private const int CORNER_BLOCK = 2;
    private const int THREEWAY_BLOCK = 3;
    private int[,] grid;
    private int width;
    private int height;
    private Vector2Int waterSource;
    private Vector2Int rootBlock;

    private readonly int[] dx = { 0, 1, 0, -1 };
    private readonly int[] dy = { 1, 0, -1, 0 };

    private void Start()
    {
        // Create the grid here with the given tile types
        // Example grid:
        grid = new int[,] {
            { 1, 2, 3 },
            { 4, 6, 3 },
            { 1, 2, 5 }
        };

        width = grid.GetLength(0);
        height = grid.GetLength(1);

        // Store the location of the Water Source and Root goal
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (grid[i, j] == WATER_SOURCE)
                {
                    waterSource = new Vector2Int(i, j);
                }
                else if (grid[i, j] == ROOT_GOAL)
                {
                    rootBlock = new Vector2Int(i, j);
                }
            }
        }

        // Check if there is a path available
        bool pathAvailable = IsPathAvailable(waterSource, waterGoal);
        if (pathAvailable)
        {
            Debug.Log("Path is available.");
        }
        else
        {
            Debug.Log("Path is not available.");
        }
    }

    private bool IsPathAvailable(Vector2Int start, Vector2Int end)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);

        bool[,] visited = new bool[width, height];
        visited[start.x, start.y] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == end)
            {
                return true;
            }

            for (int i = 0; i < 4; i++)
            {
                int newX = current.x + dx[i];
                int newY = current.y + dy[i];
                if (newX >= 0 && newX < width && newY >= 0 && newY < height && !visited[newX, newY])
                {
                    // Check if the two tiles are connected
                    if (IsConnected(grid[current.x, current.y], grid[newX, newY]))
                    {
                        visited[newX, newY] = true;
                        queue.Enqueue(new Vector2Int(newX, newY));
                    }
                }
            }
        }

        return false;
    }

    private bool IsConnected(int current, int next)
    {
        if (block1 == EMPTY_BLOCK || block2 == EMPTY_BLOCK)
        {
            return false;
        }

        if (block1 == STRAIGHT_BLOCK && block2 == STRAIGHT_BLOCK)
        {
            return true;
        }

        if (block1 == CORNER_BLOCK && block2 == CORNER_BLOCK)
        {
            return true;
        }

        if (block1 == CORNER_BLOCK_RIGHT && block2 == CORNER_BLOCK_LEFT)
        {
            return true;
        }

        return false;
    }
}
