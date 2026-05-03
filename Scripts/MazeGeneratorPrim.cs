using UnityEngine;
using System.Collections.Generic;

public class MazeGeneratorPrim : MonoBehaviour
{
    [Header("Nastavení bludiště (musí být lichá čísla!!!!)")]
    public int width = 21;
    public int height = 21;

    [Header("Prefaby k vykreslení")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject coinPrefab;
    public GameObject finishPrefab;
    public GameObject trapPrefab;
    private int[,] maze;

    void Start()
    {
        GenerateMazePrim();
        PlaceItems();
        DrawMaze();
        float spacing = 3f; 
        Vector3 finishPos = new Vector3((width - 2) * spacing, 1f, (height - 1) * spacing);
        Instantiate(finishPrefab, finishPos, Quaternion.identity, transform);
    }

    // Primův algoritmus pro generování bludiště
    //https://www.youtube.com/watch?v=bF4YGFNZIxM
    //https://www.youtube.com/watch?v=d5yzKkG1n1U
    //https://stackoverflow.com/questions/72633681/c-sharp-prims-algorithm-isnt-generating-maze-correctly
    //zdroje

    void GenerateMazePrim()
    {

        //velikost musi byt licha, aby se zajistilo, že budou cesty a stěny správně rozloženy
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

        // vytvoření vchodu a východu
      //  maze[1, 0] = 0; // Vchod
        maze[width - 2, height - 1] = 0; // Východ
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
        float spacing = 3f; // 3x3 prefaby, takže musíme posunout o 3 jednotky, aby se správně vykreslily vedle sebe

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // vykreslíme podlahu pro každé políčko (i pro stěny, aby to vypadalo lépe)
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);
                Vector3 floorPos = new Vector3(x * spacing, -1.5f, y * spacing);
                Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);
                // pokud je to stěna, vykreslíme i stěnu
                if (maze[x, y] == 1)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

  
    //stejny jako u customu
    void PlaceItems()
    {
        float spacing = 3f;
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                // pokud je aktuální políčko cesta
                if (maze[x, y] == 0)
                {
                    //bezpečnost aby se nespawnovaly mince a pasti hned u startu
                    if (x <= 2 && y <= 2) continue;

                    int wallCount = 0;
                    if (maze[x + 1, y] == 1) wallCount++;
                    if (maze[x - 1, y] == 1) wallCount++;
                    if (maze[x, y + 1] == 1) wallCount++;
                    if (maze[x, y - 1] == 1) wallCount++;

                  
                    if (wallCount == 3)
                    {
                        if (Random.Range(0, 100) < 50)
                        {
                           
                            Vector3 coinPos = new Vector3(x * spacing, 0.5f, y * spacing);
                            GameObject newCoin = Instantiate(coinPrefab, coinPos, Quaternion.identity, transform);
                            newCoin.name = "coin";
                        }
                        else
                        {
                            Vector3 trapPos = new Vector3(x * spacing, 0.5f, y * spacing);
                            GameObject newTrap = Instantiate(trapPrefab, trapPos, Quaternion.identity, transform);
                        }
                    }
                }
            }
        }
    }
}