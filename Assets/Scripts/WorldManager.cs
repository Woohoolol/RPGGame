using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
public class WorldManager : MonoBehaviour
{
    public GameObject player;
    public GameObject background;
    public float encounterRate;
    private float encounterModifier;
    private float encounterRequirement;
    public GameObject[] actions;
    public GameObject actionMenu;
    public GameObject playerStats;
    public GameObject highlightBox;
    public GameObject highlightBox2;
    public GameObject specialManager;
    public GameObject itemManager;
    public GameObject itemSelection;
    public List<int> itemsToChoose;
    //0 = Overworld mode, 1 = stats menu, 2 = detailed stats, 3 = equipment menu, 4 = inventory menu
    public float mode;
    public int focusedIndex;
    public int testx;
    public int testy;
    public int testy2;
    public int chosenItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        encounterModifier = 1;
        encounterRate = 0;
        encounterRequirement = UnityEngine.Random.Range(1, 2);
        Instantiate(background, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        Instantiate(background, new Vector3(-19.20f, 0, 0), Quaternion.Euler(0, 0, 0));
        Instantiate(background, new Vector3(19.20f, 0, 0), Quaternion.Euler(0, 0, 0));
        mode = 0;
        focusedIndex = 0;
        StartCoroutine(showMenu());
        StartCoroutine(enterBattle());
    }

    // Update is called once per frame
    void Update()
    {
        if(mode == 0)
        {
            actionMenu.SetActive(false);
            player.GetComponent<PlayerMovement>().canMove = true;
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                mode = 1;
            }
            if(player.GetComponent<PlayerMovement>().xDirection != 0 || player.GetComponent<PlayerMovement>().yDirection != 0)
            {
                encounterRate += Time.deltaTime * encounterModifier;
            }
        }
        else if(mode == 1)
        {
            actionMenu.SetActive(true);
            player.GetComponent<PlayerMovement>().canMove = false;
            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                mode = 0;
            }
           if(Keyboard.current.upArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                //Unhighlight option before moving
                actions[focusedIndex].GetComponent<SpriteRenderer>().color = new Color(0.3f, 0, 1);
                focusedIndex--;
            }
            if(Keyboard.current.downArrowKey.wasPressedThisFrame && focusedIndex < actions.Length - 1)
            {
                actions[focusedIndex].GetComponent<SpriteRenderer>().color = new Color(0.3f, 0, 1);
                focusedIndex++;
            }
            //Highlight current option
            actions[focusedIndex].GetComponent<SpriteRenderer>().color = Color.blue;
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                actions[focusedIndex].GetComponent<SpriteRenderer>().color = new Color(0.3f, 0, 1);
                if(focusedIndex == 0)
                {
                    Debug.Log("DETAILED CHARACTER STATS");
                    mode = 1.5f;
                    focusedIndex = 0;
                }
                // else if(focusedIndex == 1)
                // {
                //     Debug.Log("EQUIPMENT MENU");
                // }
                else if(focusedIndex == 1)
                {
                    Debug.Log("INVENTORY MENU");
                    mode = 3f;
                    focusedIndex = 0;
                }
                else if(focusedIndex == 2)
                {
                    Application.Quit();
                }
            }
        }
        else if(mode == 1.5 || mode == 3.5)
        {
            highlightBox.SetActive(true);
            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                if(mode == 1.5)
                {
                    mode = 1;
                }
                else if(mode == 3.5)
                {
                    mode = 3;
                }
                highlightBox.SetActive(false);
                focusedIndex = 0;
            }        
            if(Keyboard.current.upArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                focusedIndex--;
            }
            if(Keyboard.current.downArrowKey.wasPressedThisFrame && focusedIndex < SaveManager.instance.playerList.Count - 1)
            {
                focusedIndex++;
            }
            highlightBox.transform.position = playerStats.transform.GetChild(0).transform.position + new Vector3(-2f, -1.67f * focusedIndex, -1);
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if(mode == 1.5)
                {
                    highlightBox.SetActive(false);
                    mode = 2;
                }
                else if(mode == 3.5)
                {
                    if(chosenItem == 4 || SaveManager.instance.playerList[focusedIndex].GetComponent<Character>().stats.currenthp > 0)
                    {
                        highlightBox.SetActive(false);
                        itemManager.GetComponent<ItemManager>().activateItem(chosenItem, focusedIndex);
                        focusedIndex = 0;
                        mode = 1;
                    }
                }
            }
        }
        else if(mode == 2)
        {
            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                mode = 1.5f;
            }       
        }
        else if(mode == 3)
        {
            highlightBox2.SetActive(true);
            itemSelection.SetActive(true);
            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                mode = 1;
                focusedIndex = 0;
                highlightBox2.SetActive(false);
                itemSelection.SetActive(false);
            } 
            if(Keyboard.current.upArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                focusedIndex--;
            }
            if(Keyboard.current.downArrowKey.wasPressedThisFrame && focusedIndex < SaveManager.instance.inventory.Count - 1)
            {
                focusedIndex++;
            }
            highlightBox2.transform.position = new Vector3(2.75f, -1.1f * focusedIndex + 7.25f, -1);
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                itemSelection.SetActive(false);
                highlightBox2.SetActive(false);
                chosenItem = itemsToChoose[focusedIndex];
                focusedIndex = 0;
                mode = 3.5f;
            }
        }     
    }
    public IEnumerator showMenu()
    {
        while(true)
        {
            if(mode == 1 || mode == 1.5f || mode == 3.5f)
            {
                List<GameObject> portraits = new List<GameObject>();
                for(int i = 0; i < SaveManager.instance.playerList.Count; i++)
                {
                    playerStats.transform.GetChild(i).gameObject.SetActive(true);
                    GameObject portrait = Instantiate(SaveManager.instance.portraits[SaveManager.instance.playerList[i].GetComponent<Character>().stats.characterType], 
                    playerStats.transform.GetChild(i).position + new Vector3(-6f, 0f, -1), Quaternion.Euler(0, 0, 0));
                    portraits.Add(portrait);
                    portrait.transform.localScale = new Vector3(0.08f, 0.08f, 1);
                    portrait.transform.parent = actionMenu.transform;
                }
                for(int i = 0; i < SaveManager.instance.playerList.Count; i++)
                {
                    string playerInfo = String.Format("{0,-15}", "Level: " + SaveManager.instance.playerList[i].GetComponent<Character>().stats.level); 
                    Character thePlayer = SaveManager.instance.playerList[i].GetComponent<Character>();
                    playerInfo +=  "Exp: " +  Math.Ceiling(thePlayer.stats.exp) + "/" + thePlayer.expRequirement + "\n\n";
                    if(thePlayer.stats.currenthp <= 0)
                    {
                        playerInfo += "<color=red>";
                    }
                    else if(thePlayer.stats.currenthp < 0.5f * thePlayer.basemaxhp)
                    {
                        playerInfo += "<color=yellow>";
                    }
                    playerInfo += String.Format("{0,-15}", "Hp: " +  Math.Ceiling(thePlayer.stats.currenthp) + "/" + Math.Ceiling(thePlayer.basemaxhp));
                    if(thePlayer.stats.currenthp <= 0 || thePlayer.stats.currenthp < 0.5f * thePlayer.basemaxhp)
                    {
                        playerInfo += "</color>";
                    }   
                    playerInfo += "Mp: " +  Math.Ceiling(thePlayer.stats.currentmp) + "/" +  Math.Ceiling(thePlayer.basemaxmp);
                    playerStats.transform.GetChild(i).gameObject.GetComponent<TextMeshProUGUI>().SetText(playerInfo);
                }
                if(mode == 3.5)
                {
                    yield return new WaitUntil(() => mode != 3.5f);
                }
                else if(mode == 1 || mode == 1.5)
                {
                    yield return new WaitUntil(() => mode != 1 && mode != 1.5f);
                }
                for(int i = 0; i < portraits.Count; i++)
                {
                    Destroy(portraits[i]);
                }
                for(int i = 0; i < SaveManager.instance.playerList.Count; i++)
                {
                    playerStats.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else if(mode == 2)
            {
                    Character thePlayer =  SaveManager.instance.playerList[focusedIndex].GetComponent<Character>();
                    //4th index of playerstats is separate text box
                    GameObject portrait = Instantiate(SaveManager.instance.portraits[thePlayer.stats.characterType], 
                    new Vector3(playerStats.transform.GetChild(4).position.x + 7f, playerStats.transform.GetChild(4).position.y + 1.5f, -11), Quaternion.Euler(0, 0, 0));      
                    portrait.transform.localScale = new Vector3(0.25f, 0.25f, 1);      
                    portrait.transform.parent = actionMenu.transform;
                    playerStats.transform.GetChild(4).gameObject.SetActive(true);
                    playerStats.transform.GetChild(5).gameObject.SetActive(true);
                    string playerInfo = "Level: " + thePlayer.stats.level + "\n\n"; 
                    string skillsInfo = "Skills: " + "\n\n";
                    playerInfo +=  "Exp: " +  thePlayer.stats.exp + "/" + thePlayer.expRequirement + "\n\n";
                    if(thePlayer.stats.currenthp <= 0)
                    {
                        playerInfo += "<color=red>";
                    }
                    else if(thePlayer.stats.currenthp < 0.5f * thePlayer.basemaxhp)
                    {
                        playerInfo += "<color=yellow>";
                    }
                    playerInfo += "Hp: " + Math.Ceiling(thePlayer.stats.currenthp) + "/" + Math.Ceiling(thePlayer.basemaxhp) + "\n\n";
                    if(thePlayer.stats.currenthp <= 0 || thePlayer.stats.currenthp < 0.5f * thePlayer.basemaxhp)
                    {
                        playerInfo += "</color>";
                    }                
                    playerInfo += "Mp: " + Math.Ceiling(thePlayer.stats.currentmp) + "/" + Math.Ceiling(thePlayer.basemaxmp) + "\n\n";
                    playerInfo += "Physical: " + Math.Ceiling(thePlayer.basephysical) + "\n\n";
                    playerInfo += "Mental: " + Math.Ceiling(thePlayer.basemental) + "\n\n";
                    playerInfo += "Physical\nDefense: " + Math.Ceiling(thePlayer.basepdefense) + "\n\n";
                    playerInfo += "Mental\nDefense: " + Math.Ceiling(thePlayer.basemdefense) + "\n\n";
                    for(int i = 0; i < thePlayer.specialList.Count; i++)
                    {
                        int levelRequirement = thePlayer.specialList[i].Item2;
                        if(thePlayer.stats.level >= levelRequirement)
                        {
                            int specialIndex = thePlayer.specialList[i].Item1;
                            Special theSpecial = specialManager.GetComponent<SpecialManager>().allSpecials[specialIndex];
                            skillsInfo += theSpecial.name + "\n\n";
                        }
                    }
                    playerStats.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().SetText(playerInfo);
                    playerStats.transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>().SetText(skillsInfo);
                    yield return new WaitUntil(() => mode != 2);
                    Destroy(portrait);
                    playerStats.transform.GetChild(4).gameObject.SetActive(false);
                    playerStats.transform.GetChild(5).gameObject.SetActive(false);
            }
            else if(mode == 3)
            {
            string nameInfo = "";
            string descriptionInfo = "";
            string quantityInfo = "";
            nameInfo += String.Format("{0,-15}", "Name: "); 
            descriptionInfo +=  String.Format("{0,-30}", "Description: ");
            quantityInfo +=  String.Format("{0,-10}", "Quantity: ");
            nameInfo += "\n\n";
            descriptionInfo += "\n\n";
            quantityInfo += "\n\n";
            itemsToChoose = new List<int>();
            foreach(var item in SaveManager.instance.inventory)
            {
                itemsToChoose.Add(item.Key);
                //Padding on the bottom to make sure that the highlight box remains consistently stable
                int bottomPadding = 4;
                Item theItem = itemManager.GetComponent<ItemManager>().allItems[item.Key];
                int quantity = item.Value;
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
                quantityInfo += "\n\n\n\n";
                for(int j = 0; j < bottomPadding; j++)
                {
                    nameInfo += "\n";
                }
            }
            itemSelection.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(nameInfo);
            itemSelection.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText(descriptionInfo);
            itemSelection.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().SetText(quantityInfo);
            }
            yield return null;
        }
    }

    public IEnumerator enterBattle()
    {

        yield return new WaitUntil(() => encounterRate >= encounterRequirement);
        encounterRate = 0;
        //Change this note to self to be random
        SaveManager.instance.numberOfEnemies = UnityEngine.Random.Range(1, 5);
        StartCoroutine(SaveManager.instance.switchToScene("BattleScene"));
    }
}
