using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public static LevelManager levelManager;

    private void Awake()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();    
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            if (levelManager.GetInstance().getSubLevel() == levelManager.GetInstance().getFinalLevel())
            {
                levelManager.nextLevel();
            }
            else
            {
                PlayerPrefs.SetInt("lastLevel", levelManager.GetInstance().getSubLevel());
                levelManager.GetInstance().nextSubLevel();
            }
        }
        Debug.Log(other.tag);
    }
}
