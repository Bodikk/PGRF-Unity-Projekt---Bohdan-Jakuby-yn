using UnityEngine;

public class MazeGeneratorCustom : MonoBehaviour
{
    //Pomahal mi gemini s tímhle generátorem, protože jsem chtěl přidat vlastní úpravu, která náhodně bourá zdi, aby vznikaly cykly a slepé uličky, což umožňuje generovat mince a pasti. Bez toho by to bylo jenom bludiště bez života.

    [Header("Nastavení (lichá čísla)")]
    public int width = 21;
    public int height = 21;

    [Header("Vlastní úprava: % šance na zbourání zdi navíc")]
    [Range(0, 100)]
    public int extraHolesPercentage = 15;

    [Header("Prefaby")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject coinPrefab;
    public GameObject finishPrefab;
    public GameObject trapPrefab;
    private int[,] maze;


    //standardní DFS generátor, ale s přidanou úpravou, která náhodně bourá zdi, aby vznikaly cykly a slepé uličky, což umožňuje generovat mince a pasti
    void Start()
    {
        GenerateCustomMaze();
        PlaceItems();
        DrawMaze();
        float spacing = 3f; 
        Vector3 finishPos = new Vector3((width - 2) * spacing, 1f, (height - 1) * spacing);
        Instantiate(finishPrefab, finishPos, Quaternion.identity, transform);
    }


  
    void GenerateCustomMaze()
    {
        maze = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 1;

        // 1. Základní vysekání cest (DFS)
        CarvePath(1, 1);

        // 2. NAŠE ÚPRAVA: Náhodné bourání zdí pro vznik cyklů
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (maze[x, y] == 1) // Pokud je tu zeď
                {
                    // Zbouráme ji jen pokud spojuje dvě chodby (vodorovně nebo svisle)
                    bool horizontalPath = (maze[x - 1, y] == 0 && maze[x + 1, y] == 0);
                    bool verticalPath = (maze[x, y - 1] == 0 && maze[x, y + 1] == 0);

                    if ((horizontalPath || verticalPath) && Random.Range(0, 100) < extraHolesPercentage)
                    {
                        maze[x, y] = 0;
                    }
                }
            }
        }

        // 3. Pouze jeden východ
        maze[width - 2, height - 1] = 0;
    }

    void CarvePath(int x, int y)
    {
        maze[x, y] = 0;
        int[] dx = { 0, 0, 2, -2 };
        int[] dy = { 2, -2, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int tempX = dx[i]; int tempY = dy[i];
            int randomIndex = Random.Range(i, 4);
            dx[i] = dx[randomIndex]; dx[randomIndex] = tempX;
            dy[i] = dy[randomIndex]; dy[randomIndex] = tempY;
        }

        for (int i = 0; i < 4; i++)
        {
            int nextX = x + dx[i];
            int nextY = y + dy[i];
            if (nextX > 0 && nextX < width - 1 && nextY > 0 && nextY < height - 1 && maze[nextX, nextY] == 1)
            {
                maze[x + dx[i] / 2, y + dy[i] / 2] = 0;
                CarvePath(nextX, nextY);
            }
        }
    }

    void DrawMaze()
    {
        float spacing = 3f;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);
                Vector3 floorPos = new Vector3(x * spacing, -1.5f, y * spacing);
                Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);
                if (maze[x, y] == 1)
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
            }
        }
    }
    void PlaceItems()
    {
        float spacing = 3f;
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                //bezpečnostní opatření, aby se nám nevygenerovaly mince a pasti hned u startu
                if (x <= 2 && y <= 2) continue;
                // pokud je aktuální políčko cesta
                if (maze[x, y] == 0)
                {
                    int wallCount = 0;
                    if (maze[x + 1, y] == 1) wallCount++;
                    if (maze[x - 1, y] == 1) wallCount++;
                    if (maze[x, y + 1] == 1) wallCount++;
                    if (maze[x, y - 1] == 1) wallCount++;

                    // slepá ulička (3 stěny)
                    if (wallCount == 3)
                    {
                        // nenapada mě lepší způsob, jak generovat mince a pasti než právě v těch slepých uličkách, protože tam je největší smysl, aby hráč riskoval a šel pro tu minci, i když tam může být past
                        if (Random.Range(0, 100) < 50)
                        {
                            // vygenerujeme minci na normální výšce at neni pod ani moc vysoko
                            Vector3 coinPos = new Vector3(x * spacing, 0.5f, y * spacing);
                            GameObject newCoin = Instantiate(coinPrefab, coinPos, Quaternion.identity, transform);
                            newCoin.name = "coin";
                        }
                        else
                        {
                            // vygenerujeme past mírně výš ve vzduchu (Y = 0.5f)
                            Vector3 trapPos = new Vector3(x * spacing, 0.5f, y * spacing);
                            Instantiate(trapPrefab, trapPos, Quaternion.identity, transform);
                        }
                    }
                }
            }
        }
    }
}