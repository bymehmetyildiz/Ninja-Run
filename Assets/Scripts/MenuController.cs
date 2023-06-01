using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Text maxCoinText;
    public Text maxDistanceText;

    
    void Start()
    {
        maxCoinText.text = "Best Coins " + PlayerPrefs.GetInt("highscoreC");
        maxDistanceText.text = "Best Distance " + PlayerPrefs.GetInt("highscoreD") + " M";

    }

    
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
