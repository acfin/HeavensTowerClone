using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI context;
    public GameObject door;
    public TextMeshProUGUI[] itemNums;
    private float interactionDistance = 2;
    private int costToBuy = 50;
    // Start is called before the first frame update
    void Start()
    {
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
            if (hit.collider.tag == "ShopItem")
            {
                string potionType = hit.collider.name;
                // Turns on the interaction prompt.
                context.gameObject.SetActive(true);
                context.text = "Press F To Buy " + potionType + " For + " + costToBuy + " Coins.";

                // Interacts with the object upon button press.
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //if(PlayerPrefs.GetInt("Currency") >= costToOpen)
                    BuyPotion(potionType);
                }

            }
            else if (hit.collider.tag == "Door")
            {
                context.gameObject.SetActive(true);
                context.text = "Press F  To Open";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    OpenDoor();
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

    private void OpenDoor()
    {
        Animator doorAnimator = door.GetComponent<Animator>();
        doorAnimator.SetTrigger("DoorATrigger");
        Debug.Log("Opening door" + doorAnimator.GetParameter(0));
    }

    private void BuyPotion(string potionType)
    {
        PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency") - costToBuy);
        CollectPotion(potionType);
    }

    private void CollectPotion(string potionType)
    {
        switch (potionType)
        {
            case ("Health Potion"):
                PlayerPrefs.SetInt("MaxHealthStacks", PlayerPrefs.GetInt("MaxHealthStacks") + 1);
                break;
            case ("Attack Speed Potion"):
                PlayerPrefs.SetInt("AttackSpeedStacks", PlayerPrefs.GetInt("AttackSpeedStacks") + 1);
                break;
            case ("Speed Potion"):
                PlayerPrefs.SetInt("SpeedStacks", PlayerPrefs.GetInt("SpeedStacks") + 1);
                break;
            case ("Crit Chance Potion"):
                PlayerPrefs.SetInt("CritChanceStacks", PlayerPrefs.GetInt("CritChanceStacks") + 1);
                break;
            case ("Damage Potion"):
                PlayerPrefs.SetInt("DamageStacks", PlayerPrefs.GetInt("DamageStacks") + 1);
                break;
        }
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
