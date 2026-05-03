using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Nastavení času")]
    public float timeLeft = 60f;
    public bool isGameActive = true;

    [Header("UI Elementy")]
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;

    //dynamic texty pro Game Over panel, aby se dal použít jak pro prohru, tak pro výhru
    public TextMeshProUGUI panelTitleText; // zemrel jsi / level dokončen
    public TextMeshProUGUI actionButtonText; // text v prvním tlačítku
    public GameObject actionButtonObj; // samotné první tlačítko (abychom ho mohli na konci vypnout)

    [Header("Levely (pro paměť hry)")]
    public static int savedLevel = 1;

    public GameObject level1Manager;
    public GameObject level2Manager;
    public GameObject level3Manager;
    public GameObject level4Manager;

    [Header("Pause Menu")]
    public GameObject pausePanel; 
    public bool isPaused = false;

    public static int totalScore = 0; // globalní skóre, které se bude přičítat napříč levely
    public GameObject leaderboardObj; // celyx panel pro zobrazení konečného žebříčku
    public TextMeshProUGUI leaderboardText; //text se skore, který se zobrazí v leaderboardu

    void Start()
    {

        //testovací výpis do konzole, abych věděl, že se správně načítá uložený level
        Debug.Log("Aktuální level: " + savedLevel);
        Debug.Log("Hra začala. Aktuální level: " + savedLevel);

        //musi to tu byt, aby se načetl správný level, když se zrovna dohrál a pokračuje se na další
        isGameActive = true;
        Time.timeScale = 1f;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        //nastavení správného levelu podle toho, co je uložené v savedLevel
        SetActiveLevel(savedLevel);
    }

    void Update()
    {

        // kontrola pro pauzu

        if (Input.GetKeyDown(KeyCode.Escape) && isGameActive)
        {
            TogglePause();
        }

        // pokud je hra aktivní a není pozastavená, odpočítáváme čas

        if (isGameActive)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateDisplay();
            }
            else
            {
                timeLeft = 0;
                isGameActive = false;
                TriggerGameOver();
            }
        }

    }

    // tahle metoda se stará o aktualizaci textu s časem, aby to vypadalo jako 00:59, 00:58 atd.
    void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    //konec hry pro prohru - zobrazí panel, změní texty, zastaví čas a odemkne kurzor
    public void TriggerGameOver()
    {
        timerText.text = "KONEC ČASU!";
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // texty pro prohru
        if (panelTitleText != null) panelTitleText.text = "ZEMŘEL JSI";
        if (actionButtonText != null) actionButtonText.text = "Opakovat tento level";
        if (actionButtonObj != null) actionButtonObj.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void WinGame()
    {
        //POJISTKA PRO ZAMEK - pokud už není hra aktivní, nic nedělej (tím zabráníme vícenásobnému volání této metody)
        if (!isGameActive) return;

        // okamžitě vypneme aktivitu hry - tohle je ten ZÁMEK
        isGameActive = false;
        //test at vim
        Debug.Log("wingame zavoláno pro Level: " + savedLevel);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // zobrazíme Game Over panel, ale s texty pro výhru
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        //test at vim
        if (panelTitleText != null)
            panelTitleText.text = "LEVEL " + savedLevel + " DOKONČEN!";

        // KONTROLA KONCE HRY
        if (savedLevel >= 4) // konec hry, protože máme jen 4 levely
        {
            if (panelTitleText != null) panelTitleText.text = "GRATULUJI! DOHRÁL JSI CELOU HRU!";
            if (actionButtonObj != null) actionButtonObj.SetActive(false);

            if (timerText != null) timerText.gameObject.SetActive(false);
            //final
            ShowFinalLeaderboard();
        }
        else
        {
            // teprve tady, když víme, že hra nekončí, zvýšíme level pro příště
            savedLevel++;

            if (actionButtonText != null)
                actionButtonText.text = "Pokračovat na Level " + savedLevel;

            if (actionButtonObj != null) actionButtonObj.SetActive(true);
        }
    }

    void ShowFinalLeaderboard()
    {
        if (leaderboardObj != null) leaderboardObj.SetActive(true);

        // uložím rekord
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (totalScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", totalScore);
        }

        if (leaderboardText != null)
        {
            leaderboardText.text = "Tvé skóre: " + totalScore + "\nNejlepší výkon: " + PlayerPrefs.GetInt("HighScore");
        }
    }


    //restart pro tlačítko "Opakovat tento level" - načte znovu aktuální scénu, ale nezmění savedLevel, takže se načte stejný level
    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //restart pro tlačítko "Pokračovat na další level" - načte znovu aktuální scénu, ale protože savedLevel už je navýšené, načte se další level
    public void RestartGameFromStart()
    {
        savedLevel = 1;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //tlačítko pro ukončení hry, které se zobrazí na konci, když dohraješ všechny levely
    public void QuitGame()
    {
        Debug.Log("vypina se "); //test
        Application.Quit(); //jen pro build, v editoru se nic nestane
    }

    void SetActiveLevel(int levelIndex)
    {
        if (level1Manager != null) level1Manager.SetActive(levelIndex == 1);
        if (level2Manager != null) level2Manager.SetActive(levelIndex == 2);
        if (level3Manager != null) level3Manager.SetActive(levelIndex == 3);
        if (level4Manager != null) level4Manager.SetActive(levelIndex == 4);
    }
    public void TogglePause()
    {
        isPaused = !isPaused; // přepne stav (zapnuto/vypnuto)

        if (isPaused)
        {
            // zastavit hru
            if (pausePanel != null) pausePanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // pokračovat ve hře
            if (pausePanel != null) pausePanel.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // tahle metoda se napojí na ten Slider z UI
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // tohle globálně ztlumí/zesílí celou hru v Unity
    }

}