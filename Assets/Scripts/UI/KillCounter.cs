using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillCounter : MonoBehaviour
{
    //https://www.youtube.com/watch?v=StjQKOVPInw
    //video shows how to implement kill counter into enemy script once it is complete

    public TextMeshProUGUI counterText;
    public TextMeshProUGUI finalCounterText;
    int kills;
    
    // Start is called before the first frame update
    void Start()
    {
        kills = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ShowKills();
    }

    private void ShowKills()
    {
        counterText.text = kills.ToString();
        finalCounterText.text = kills.ToString();
    }

    public void AddKill()
    {
        kills++;
    }
}
