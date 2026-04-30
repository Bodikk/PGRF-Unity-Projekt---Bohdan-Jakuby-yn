using UnityEngine;

public class MazeGeneratorDFS : MonoBehaviour
{
    [Header("Nastavení bludiště (musí být lichá čísla)")]
    public int width = 21;
    public int height = 21;

    [Header("Prefaby k vykreslení")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    // 2D pole: 1 = stěna, 0 = cesta
    private int[,] maze;

    void Start()
    {
        GenerateMazeDFS();
        DrawMaze();
    }

    void GenerateMazeDFS()
    {
        maze = new int[width, height];

        // Naplníme celé bludiště stěnami
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1;
            }
        }

        // Začneme vysekávat cestu
        CarvePath(1, 1);
    }

    void CarvePath(int x, int y)
    {
        maze[x, y] = 0;

        // Směry: Nahoru, Dolů, Doprava, Doleva (o 2 políčka)
        int[] dx = { 0, 0, 2, -2 };
        int[] dy = { 2, -2, 0, 0 };

        // Zamíchání směrů pro náhodnost
        for (int i = 0; i < 4; i++)
        {
            int tempX = dx[i]; int tempY = dy[i];
            int randomIndex = Random.Range(i, 4);
            dx[i] = dx[randomIndex]; dx[randomIndex] = tempX;
            dy[i] = dy[randomIndex]; dy[randomIndex] = tempY;
        }

        // Průchod do dalších políček
        for (int i = 0; i < 4; i++)
        {
            int nextX = x + dx[i];
            int nextY = y + dy[i];

            if (nextX > 0 && nextX < width - 1 && nextY > 0 && nextY < height - 1 && maze[nextX, nextY] == 1)
            {
                // Zbourání stěny mezi
                maze[x + dx[i] / 2, y + dy[i] / 2] = 0;
                CarvePath(nextX, nextY);
            }
        }
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                Vector3 floorPos = new Vector3(x, -1f, y);
                Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);

                if (maze[x, y] == 1)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}