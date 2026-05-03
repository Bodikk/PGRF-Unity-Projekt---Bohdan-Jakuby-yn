using UnityEngine;

public class MazeGeneratorDFS : MonoBehaviour
{
    [Header("Nastavení bludiště (musí být lichá čísla!!!!)")]
    public int width = 21;
    public int height = 21;

    [Header("Prefaby k vykreslení ")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject coinPrefab;
    public GameObject finishPrefab;
    public GameObject trapPrefab;
    private int[,] maze;

    void Start()
    {
        GenerateMazeDFS();
        PlaceItems();
        DrawMaze();
        float spacing = 3f; 
        Vector3 finishPos = new Vector3((width - 2) * spacing, 1f, (height - 1) * spacing);
        Instantiate(finishPrefab, finishPos, Quaternion.identity, transform);
    }

    //https://en.wikipedia.org/wiki/Maze_generation_algorithm#Randomized_depth-first_search
    //jeste nějaky zdroje ale už nemužu najit, pak pomoc na detaily s gemini, dodal komentare, ale jinak je to klasický DFS algoritmus pro generování bludiště, který zajišťuje, že bludiště bude mít vždy řešení, protože se při generování nikdy nevrací zpět a nebourá zdi, které už jsou součástí cesty
    void GenerateMazeDFS()
    {
        maze = new int[width, height];

        // 1. všechno je na začátku stěna
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1;
            }
        }

        // 2. Vysekání cesty (DFS algoritmus)
        CarvePath(1, 1);

        // 3. Vytvoření POUZE VÝCHODU 
        maze[width - 2, height - 1] = 0; // Východ
    }

    void CarvePath(int x, int y)
    {
        maze[x, y] = 0;

        // Směry: Nahoru, Dolů, Doprava, Doleva (skoky po 2 kvůli tloušťce zdí)
        int[] dx = { 0, 0, 2, -2 };
        int[] dy = { 2, -2, 0, 0 };

        // Náhodné zamíchání směrů (aby to nešlo pořád jedním směrem)
        for (int i = 0; i < 4; i++)
        {
            int tempX = dx[i]; int tempY = dy[i];
            int randomIndex = Random.Range(i, 4);
            dx[i] = dx[randomIndex]; dx[randomIndex] = tempX;
            dy[i] = dy[randomIndex]; dy[randomIndex] = tempY;
        }

        // Procházení sousedních políček
        for (int i = 0; i < 4; i++)
        {
            int nextX = x + dx[i];
            int nextY = y + dy[i];

            // Pokud je políčko v rámci mapy a je to stěna
            if (nextX > 0 && nextX < width - 1 && nextY > 0 && nextY < height - 1 && maze[nextX, nextY] == 1)
            {
                // Zbouráme zeď mezi aktuálním a dalším políčkem
                maze[x + dx[i] / 2, y + dy[i] / 2] = 0;
                // Posuneme se dál (rekurze)
                CarvePath(nextX, nextY);
            }
        }
    }

    void DrawMaze()
    {
        float spacing = 3f; // Rozestupy pro velké 3x3x3 stěny

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);
                Vector3 floorPos = new Vector3(x * spacing, -1.5f, y * spacing);

                Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);

                if (maze[x, y] == 1)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

    //stejny jako u customu, ale pro jistotu jsem ho zkopíroval, aby se nám to nezamotalo, protože je to docela důležitá část, která dělá hru zajímavou
    void PlaceItems()
    {
        float spacing = 3f;
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {

                //bezpečnost aby se nespawnovaly mince a pasti hned u startu
                if (x <= 2 && y <= 2) continue;

                
                if (maze[x, y] == 0)
                {
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
                            Instantiate(trapPrefab, trapPos, Quaternion.identity, transform);
                        }
                    }
                }
            }
        }
    }
}