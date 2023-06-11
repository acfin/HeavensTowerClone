using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }
    public LevelManager GetInstance() { return instance;  }

    private int subLevel;
    private int finalLevel;
    private int level;
    private bool subLevelOne = true;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        if (SceneManager.GetActiveScene().name == "Level 1")
            level = 1;
        if (SceneManager.GetActiveScene().name == "Level 2")
            level = 2;
        if (SceneManager.GetActiveScene().name == "Level 3")
            level = 3;
        finalLevel = 3;
        subLevel = PlayerPrefs.GetInt("lastLevel") + 1;
    }

    public void Update()
    {
        Debug.Log(level + "-" + subLevel);
    }
    public void nextSubLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void nextLevel()
    {
        switch (level)
        {
            case 1:
                subLevel = 0;
                PlayerPrefs.SetInt("lastLevel", 0);
                SceneManager.LoadScene("Level 2");
                break;
            case 2:
                subLevel = 0;
                PlayerPrefs.SetInt("lastLevel", 0);
                SceneManager.LoadScene("Level 3");
                break;
            case 3:
                subLevel = 0;
                PlayerPrefs.SetInt("lastLevel", 0);
                SceneManager.LoadScene("Menu");
                break;
            default:
                Debug.Log("Error");
                break;
        }
    }

    public int getSubLevel() { return subLevel; }
    public int getLevel() { return level; } 
    public int getFinalLevel() { return finalLevel;  }
    public void setSubLevel(int subLevel) { this.subLevel = subLevel; }
}
