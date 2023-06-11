using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyCounter : MonoBehaviour
{

    public TextMeshProUGUI counterText;
    const int copperValue = 5;
    const int silverValue = 25;
    const int goldValue = 100;

    
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ShowCurrency();
    }

    private void ShowCurrency()
    {
        counterText.text = PlayerPrefs.GetInt("Currency").ToString();

    }

    public void AddCopper()
    {
        PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency") + copperValue);
    }

    public void AddSilver()
    {
        PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency") + silverValue);
    }

    public void AddGold()
    {
        PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency") + goldValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Copper")
        {
            AddCopper();
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Silver")
        {
            AddSilver();
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Gold")
        {
            AddGold();
            Destroy(other.gameObject);
        }
    }
}
