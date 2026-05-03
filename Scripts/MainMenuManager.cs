using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour
{
    
    public string gameSceneName = "SampleScene";

    public void StartGame()
    {
        //reset hodnoty pro nový začátek
        GameManager.savedLevel = 1;
        GameManager.totalScore = 0;

        Debug.Log("Resetuji level na 1. Načítám hru...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Vypínám hru...");
        Application.Quit(); //jen pro build, v editoru se nic nestane
    }
}