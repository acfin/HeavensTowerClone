using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void Awake()
    {
        //Makes sure the player starts with 0 currency
        PlayerPrefs.SetInt("Currency", 0);
        //Make sure player starts with 0 of all potions
        PlayerPrefs.SetInt("SpeedStacks", 0);
        PlayerPrefs.SetInt("DamageStacks", 0);
        PlayerPrefs.SetInt("AttackSpeedStacks", 0);
        PlayerPrefs.SetInt("CritChanceStacks", 0);
        PlayerPrefs.SetInt("MaxHealthStacks", 0);
    }
    public void LoadScene(string inputScene)
    {
        if (inputScene == "Level 1")
            PlayerPrefs.SetInt("lastLevel", 0);
        SceneManager.LoadScene(inputScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetTheGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}