using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class ShopManager : MonoBehaviour
{
    public List<int> allItems;
    public List<int> itemPrices;
    public GameObject shopSelection;
    public GameObject background;
    public ItemManager itemManager;
    public GameObject highlightBox2;
    public GameObject playerStats;
    public bool activated;
    public int shopItemID;
    public bool bought;
    private int focusedIndex;
    public void buyedItem(int itemID)
    {
        bought = true;
        shopItemID = itemID;
    }

    public void activateShop()
    {
        activated = true;
        focusedIndex = 0;
    }
    void Awake()
    {
        //Only use if there is one instance of the gameobject!
        itemManager = FindObjectOfType<ItemManager>();
        activated = false;
        allItems = new List<int>{0, 1, 2, 3, 4};
        itemPrices = new List<int>{15, 30, 60, 60, 150};
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.GetChild(0).gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        StartCoroutine(boughtItem());
    }

    // Update is called once per frame
    void Update()
    {
        if(activated)
        {
            highlightBox2.SetActive(true);
            shopSelection.SetActive(true);
            background.SetActive(true);

            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                highlightBox2.SetActive(false);
                shopSelection.SetActive(false);
                background.SetActive(false);
                activated = false;
            } 
            if(Keyboard.current.upArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                focusedIndex--;
            }
            if(Keyboard.current.downArrowKey.wasPressedThisFrame && focusedIndex < allItems.Count - 1)
            {
                focusedIndex++;
            }
            highlightBox2.transform.position = playerStats.transform.GetChild(0).transform.position + new Vector3(-3.5f, -1.1f * focusedIndex - 0.5f, -1);
            if(Keyboard.current.zKey.wasPressedThisFrame && !SaveManager.instance.dialogueActive)
            {
                buyedItem(focusedIndex);
            }
            string nameInfo = "";
            string descriptionInfo = "";
            string priceInfo = "";
            string quantityInfo = "";
            nameInfo += String.Format("{0,-15}", "Name: "); 
            descriptionInfo +=  String.Format("{0,-30}", "Description: ");
            priceInfo += String.Format("{0,-10}", "Price: ");
            quantityInfo +=  String.Format("{0,-10}", "Quantity: ");
            nameInfo += "\n\n";
            descriptionInfo += "\n\n";
            priceInfo += "\n\n";
            quantityInfo += "\n\n";
            for(int i = 0; i < allItems.Count; i++)
            {
                //Padding on the bottom to make sure that the highlight box remains consistently stable
                int bottomPadding = 4;
                Item theItem = itemManager.allItems[allItems[i]];
                int quantity = 0;
                if(SaveManager.instance.inventory.ContainsKey(allItems[i]))
                {
                    quantity = SaveManager.instance.inventory[allItems[i]];
                }
                priceInfo += String.Format("{0,-5}", itemPrices[allItems[i]]);
                quantityInfo +=  String.Format("{0,-5}", quantity);
                String[] words = theItem.description.Split();
                String line = "";
                foreach(string word in words)
                {
                    if((line + word + " ").Length >= 40)
                    {
                        descriptionInfo += line + "\n";
                        line = word + " ";
                        bottomPadding--;
                    }
                    else
                    {
                        line += word + " ";
                    }
                }
                descriptionInfo += line;
                for(int j = 0; j < bottomPadding; j++)
                {
                    descriptionInfo += "\n";
                }
                line = "";
                words = theItem.name.Split();
                bottomPadding = 4;
                foreach(string word in words)
                {
                    if((line + word + " ").Length >= 10)
                    {
                        nameInfo += line + "\n";
                        line = word + " ";
                        bottomPadding--;
                    }
                    else
                    {
                        line += word + " ";
                    }
                }
                nameInfo += line;
                priceInfo += "\n\n\n\n";
                quantityInfo += "\n\n\n\n";
                for(int j = 0; j < bottomPadding; j++)
                {
                    nameInfo += "\n";
                }
            }
            shopSelection.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(nameInfo);
            shopSelection.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText(descriptionInfo);
            shopSelection.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().SetText(priceInfo);
            shopSelection.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().SetText(quantityInfo);
            shopSelection.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().SetText("Money: " + SaveManager.instance.money);
        }

    }

    public IEnumerator boughtItem()
    {
        Dictionary<int, int> inventory = SaveManager.instance.inventory;
        while(true)
        {
            yield return new WaitUntil(() => bought);
            if(SaveManager.instance.money >= itemPrices[shopItemID])
            {
                SaveManager.instance.money -= itemPrices[shopItemID];
                if(!inventory.ContainsKey(shopItemID))
                {
                    inventory.Add(shopItemID, 1);
                }
                else
                {
                    inventory[shopItemID] += 1;
                }
            }
            else
            {
                SaveManager.instance.spawnDialogue(new List<string>{"Hey, you don't have enough money!", "Come back when you're a little mmmmm...", "richer!"}, characterDialogue: true, dialogueCharacters: new List<int>{4, 4, 4});
            }
            bought = false;
        }
    }


}
