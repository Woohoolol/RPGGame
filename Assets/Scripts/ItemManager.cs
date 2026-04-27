using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
public class ItemManager : MonoBehaviour
{
    public List<Item> allItems;
    public GameObject battleManager;
    public int playerIndex;
    //Item ID 0 = 50% hp potion, 1 = 50% mp potion, 2 = 100% hp potion, 3 = 100% mp potion, 4 = Elixir
    public int itemID;
    public bool acquired;
    public bool activated;
    public int quantity;
    public void acquiredItem(int itemID, int quantity)
    {
        acquired = true;
        this.itemID = itemID;
        this.quantity = quantity;
    }
    public void activateItem(int itemID, int playerIndex = 0)
    {
        activated = true;
        this.itemID = itemID;
        this.playerIndex = playerIndex;
    }

    void Awake()
    {
        allItems = new List<Item>();
        allItems.Add(new Item(0, "Health Vial", "Restores 50% of max hp."));
        allItems.Add(new Item(1, "Mana Vial", "Restores 50% of max mp."));
        allItems.Add(new Item(2, "Health Jug", "Restores all hp."));
        allItems.Add(new Item(3, "Mana Jug", "Restores all mp."));
        allItems.Add(new Item(4, "Elixir", "Fully revives a dead ally."));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(addItem());
        StartCoroutine(useItem());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator addItem()
    {
        Dictionary<int, int> inventory = SaveManager.instance.inventory;
        while(true)
        {
            yield return new WaitUntil(() => acquired);
            Debug.Log("ACTIAVETD");
            if(!inventory.ContainsKey(itemID))
            {
                Debug.Log("Addedfdfds");
                inventory.Add(itemID, quantity);
            }
            else
            {
                inventory[itemID] += quantity;
            }
            acquired = false;
        }
    }
    public IEnumerator useItem()
    {
        Dictionary<int, int> inventory = SaveManager.instance.inventory;
        while(true)
        {
            yield return new WaitUntil(() => activated);
            if(inventory[itemID] == 1)
            {
                inventory.Remove(itemID);
            }
            else
            {
                inventory[itemID]--;
            }
            //In battle, use on the instantiated allies
            Character selectedCharacter;
            if(battleManager == null)
            {
                selectedCharacter = SaveManager.instance.playerList[playerIndex].GetComponent<Character>();

            }
            //In overworld just use on the allies in savemanager
            else
            {
                selectedCharacter = battleManager.GetComponent<BattleManager>().playerList[playerIndex].GetComponent<Character>();
            }
            if(itemID == 0)
            {
                selectedCharacter.stats.currenthp += 0.5f * selectedCharacter.basemaxhp;
            }
            else if(itemID == 1)
            {
                selectedCharacter.stats.currentmp += 0.5f * selectedCharacter.basemaxmp;
            }
            else if(itemID == 2)
            {
                selectedCharacter.stats.currenthp += 1f * selectedCharacter.basemaxhp;
            }
            else if(itemID == 3)
            {
                selectedCharacter.stats.currentmp += 1f * selectedCharacter.basemaxmp;
            }
            else if(itemID == 4)
            {
                selectedCharacter.stats.currenthp += 1f * selectedCharacter.basemaxhp;
            }
            activated = false;
        }
    }

}
