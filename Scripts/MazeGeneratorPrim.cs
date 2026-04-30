using UnityEngine;
using System.Collections.Generic;

public class MazeGeneratorPrim : MonoBehaviour
{
    [Header("Nastavení bludiště (musí být lichá čísla)")]
    public int width = 21;
    public int height = 21;

    [Header("Prefaby k vykreslení")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    private int[,] maze;

    void Start()
    {
        GenerateMazePrim();
        DrawMaze();
    }

    void GenerateMazePrim()
    {
        maze = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 1;

        List<Vector2Int> walls = new List<Vector2Int>();

        maze[1, 1] = 0;
        AddWalls(1, 1, walls);

        while (walls.Count > 0)
        {
            int randomIndex = Random.Range(0, walls.Count);
            Vector2Int wall = walls[randomIndex];
            walls.RemoveAt(randomIndex);

            int x = wall.x;
            int y = wall.y;

            // Kontroly a bourání stěn
            if (x > 0 && x < width - 1 && maze[x - 1, y] == 0 && maze[x + 1, y] == 1)
            {
                maze[x, y] = 0; maze[x + 1, y] = 0; AddWalls(x + 1, y, walls);
            }
            else if (x > 0 && x < width - 1 && maze[x + 1, y] == 0 && maze[x - 1, y] == 1)
            {
                maze[x, y] = 0; maze[x - 1, y] = 0; AddWalls(x - 1, y, walls);
            }
            else if (y > 0 && y < height - 1 && maze[x, y - 1] == 0 && maze[x, y + 1] == 1)
            {
                maze[x, y] = 0; maze[x, y + 1] = 0; AddWalls(x, y + 1, walls);
            }
            else if (y > 0 && y < height - 1 && maze[x, y + 1] == 0 && maze[x, y - 1] == 1)
            {
                maze[x, y] = 0; maze[x, y - 1] = 0; AddWalls(x, y - 1, walls);
            }
        }
    }

    void AddWalls(int x, int y, List<Vector2Int> walls)
    {
        if (x > 1) walls.Add(new Vector2Int(x - 1, y));
        if (x < width - 2) walls.Add(new Vector2Int(x + 1, y));
        if (y > 1) walls.Add(new Vector2Int(x, y - 1));
        if (y < height - 2) walls.Add(new Vector2Int(x, y + 1));
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                Instantiate(floorPrefab, new Vector3(x, -1f, y), Quaternion.identity, transform);

                if (maze[x, y] == 1)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}