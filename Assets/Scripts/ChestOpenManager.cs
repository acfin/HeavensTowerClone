using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestOpenManager : MonoBehaviour
{
    GameObject chest;
    public TextMeshProUGUI context;
    public TextMeshProUGUI[] itemNums;
    private float interactionDistance = 2;
    public GameObject[] items;
    private int costToOpen = 50;
    int item = 0;
    string itemText;
    Animator itemAnimator;
    bool pickedUp = false;
    // Start is called before the first frame update
    void Start()
    {
        chest = GameObject.Find("Chest");
        item = Random.Range(0, items.Length);
        switch(item)
        {
            case (0):
                itemText = "Increase Max Health";
                break;
            case (1):
                itemText = "Increase Attack Speed";
                break;
            case (2):
                itemText = "Increase Move Speed";
                break;
            case (3):
                itemText = "Increase Critical Hit Chance";
                break;
            case (4):
                itemText = "Increase Damage";
                break;
        }
        UpdateItemNums();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);
            if (hit.collider.tag == "Interactable")
            {

                // Turns on the interaction prompt.
                context.gameObject.SetActive(true);
                context.text = "Press F To Open (Will Cost " + costToOpen + " Coins)";

                // Interacts with the object upon button press.
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //if(PlayerPrefs.GetInt("Currency") >= costToOpen)
                    OpenChest();
                }

            }
            else if(hit.collider.tag == "Pickup" && !pickedUp)
            {
                context.gameObject.SetActive(true);
                context.text = "Press F  To Drink And " + itemText;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    pickedUp = true;
                    CollectPotion();
                }
            }
            else
            {
                context.gameObject.SetActive(false);
            }
        }
        else
        {
            context.gameObject.SetActive(false);
        }
    }

    private void OpenChest()
    {
        GameObject itemObj = items[item];
        itemObj.transform.position = chest.transform.position + (Vector3.up * 0.25f);
        Destroy(chest);
        GameObject instantiatedItem = Instantiate(itemObj);
        itemAnimator = instantiatedItem.GetComponent<Animator>();
        itemAnimator.SetFloat("speedMultiplier", 0);
        PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency") - costToOpen);
    }

    private void CollectPotion()
    {
        switch (item)
        {
            case (0):
                PlayerPrefs.SetInt("MaxHealthStacks", PlayerPrefs.GetInt("MaxHealthStacks") + 1);
                break;
            case (1):
                PlayerPrefs.SetInt("AttackSpeedStacks", PlayerPrefs.GetInt("AttackSpeedStacks") + 1);
                break;
            case (2):
                PlayerPrefs.SetInt("SpeedStacks", PlayerPrefs.GetInt("SpeedStacks") + 1);
                break;
            case (3):
                PlayerPrefs.SetInt("CritChanceStacks", PlayerPrefs.GetInt("CritChanceStacks") +1);
                break;
            case (4):
                PlayerPrefs.SetInt("DamageStacks", PlayerPrefs.GetInt("DamageStacks") + 1);
                break;
        }
        if(itemAnimator != null)
            itemAnimator.SetFloat("speedMultiplier", 2);
        UpdateItemNums();
    }

    private void UpdateItemNums()
    {
        itemNums[0].text = PlayerPrefs.GetInt("MaxHealthStacks").ToString();
        itemNums[1].text = PlayerPrefs.GetInt("AttackSpeedStacks").ToString();
        itemNums[2].text = PlayerPrefs.GetInt("SpeedStacks").ToString();
        itemNums[3].text = PlayerPrefs.GetInt("CritChanceStacks").ToString();
        itemNums[4].text = PlayerPrefs.GetInt("DamageStacks").ToString();
    }
}
