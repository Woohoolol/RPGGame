using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
public class BattleHud : MonoBehaviour
{
    //0 attack, 1 special, 2 item, 3 escape
    public List<GameObject> actions;
    public List<GameObject> specials;
    public GameObject actionMenu;
    public BattleManager battleManager;
    public GameObject specialMenu;
    public GameObject playerStats;
    public GameObject optionDescription;
    public GameObject returnButton;
    public GameObject selectionScreen;
    public GameObject highlightBox;
    public GameObject enemySpawn;
    public float testx;
    public float testy;
    public float testy2;
    //0 main menu focused, 1 attack menu, 2 special menu, 3 item, 4 escape, 5 victory, 6 defeat, 7 enemy attack, 8 waiting for input (victory/defeat)
    public float mode;
    public int focusedIndex;
    public int chosenSpecial;
    public int chosenItem;
    private bool finished;
    private List<GameObject> portraits;
    private List<int> itemsToChoose;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Where enemies will spawn
        Transform positions = enemySpawn.transform.GetChild(SaveManager.instance.numberOfEnemies - 1);
        for(int i = 0; i < SaveManager.instance.numberOfEnemies; i++)
        {
            Transform thePosition = positions.GetChild(i);
            List<GameObject> listOfEnemies = SaveManager.instance.enemies[SaveManager.instance.biome - 1].biomeEnemies;
            GameObject spawnedType = listOfEnemies[UnityEngine.Random.Range(0, listOfEnemies.Count)];
            GameObject spawnedEnemy = Instantiate(spawnedType, thePosition.position, Quaternion.Euler(0, 0, 0));
            battleManager.enemyList.Add(spawnedEnemy);
        }
        portraits = new List<GameObject>();
        finished = false;
        focusedIndex = 0;
        mode = 0;
        for(int i = 0; i < SaveManager.instance.playerList.Count; i++)
        {   
            //Note that only serizable data is transferred via instantiation, aka the lists will not transfer over for the special menu 
            GameObject player = Instantiate(SaveManager.instance.playerList[i], playerStats.transform.GetChild(i).position + new Vector3(0, 4f, -1), Quaternion.Euler(0, 0, 0));
            battleManager.playerList.Add(player);
            player.transform.parent = selectionScreen.transform;
        } 
        for(int i = 0; i < battleManager.playerList.Count; i++)
        {
            playerStats.transform.GetChild(i).gameObject.SetActive(true);
            GameObject portrait = Instantiate(SaveManager.instance.portraits[battleManager.playerList[i].GetComponent<Character>().stats.characterType], 
            playerStats.transform.GetChild(i).position + new Vector3(0, 1.725f, -1), Quaternion.Euler(0, 1, 0));
            portraits.Add(portrait);
            portrait.transform.parent = selectionScreen.transform;
        }
        for(int i = 0; i < SaveManager.instance.numberOfEnemies; i++)
        {

        }
        StartCoroutine(showVictory());
        StartCoroutine(escape());
    }

    // Update is called once per frame
    void Update()
    {
        //Victory/defeat should always take priority over enemy's turn if there are no allies/enemies
        if(battleManager.victory())
        {
            mode = 5;
        }
        else if(battleManager.defeat())
        {
            mode = 6;
        }
        else if(battleManager.enemyTurn() || battleManager.specialManager.GetComponent<SpecialManager>().activated)
        {
            mode = 7;
        }

        if(mode == 0)
        {
            optionDescription.SetActive(false);
            showActiveMenu();
            if(Keyboard.current.upArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                //Unhighlight option before moving
                actions[focusedIndex].GetComponent<SpriteRenderer>().color = new Color(0.3f, 0, 1);
                focusedIndex--;
            }
            if(Keyboard.current.downArrowKey.wasPressedThisFrame && focusedIndex < actions.Count - 1)
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
                    Debug.Log("ATTACK");
                    mode = 1;
                    focusedIndex = 0;
                }
                else if(focusedIndex == 1)
                {
                    Debug.Log("SPECIAL");
                    focusedIndex = 0;
                    mode = 2;
                }
                else if(focusedIndex == 2)
                {
                    Debug.Log("ITEM");
                    focusedIndex = 0;
                    mode = 3;
                }
                else if(focusedIndex == 3)
                {
                    Debug.Log("ESCAPE");
                    mode = 4;
                }
            }
        }
        else if(mode == 1)
        {
            optionDescription.GetComponent<TextMeshProUGUI>().SetText("Select a target to attack.");
            optionDescription.SetActive(true);
            showInactiveMenu();
            if(Keyboard.current.leftArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                focusedIndex--;
            }
            if(Keyboard.current.rightArrowKey.wasPressedThisFrame &&  focusedIndex < battleManager.enemyList.Count - 1)
            {
                battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                focusedIndex++;
            }  
            battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.blue;
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                battleManager.allyAttack(focusedIndex);
                //Reset to top of menu after action
                mode = 0;
                focusedIndex = 0;
            }
            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                mode = 0;
                focusedIndex = 0;     
            }
        }
        else if(mode == 2)
        {
            highlightBox.SetActive(true);
            specialMenu.SetActive(true);
            optionDescription.SetActive(true);
            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                mode = 0;
                focusedIndex = 0;
                highlightBox.SetActive(false);
                specialMenu.SetActive(false);
            }       
            List<int> validSpecials = new List<int>();
            for(int i = 0; i < SaveManager.instance.playerList[battleManager.currentPlayerIndex].GetComponent<Character>().specialList.Count; i++)
            {
                float specialLevelRequirement = SaveManager.instance.playerList[battleManager.currentPlayerIndex].GetComponent<Character>().specialList[i].Item2;
                float currentLevel = battleManager.playerList[battleManager.currentPlayerIndex].GetComponent<Character>().stats.level;
                if(currentLevel >= specialLevelRequirement)
                {
                    validSpecials.Add(SaveManager.instance.playerList[battleManager.currentPlayerIndex].GetComponent<Character>().specialList[i].Item1);
                }
            }
            if(Keyboard.current.upArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                focusedIndex--;
            }
            if(Keyboard.current.downArrowKey.wasPressedThisFrame && focusedIndex < validSpecials.Count - 1)
            {
                focusedIndex++;
            }
            highlightBox.transform.position = new Vector3(0, -1 * focusedIndex + 1.5f, -1);

            string nameInfo = "\n\n";
            string descriptionInfo = "\n\n";
            string mpcostInfo = "\n\n";

            nameInfo += String.Format("{0,-15}", "Name: "); 
            descriptionInfo +=  String.Format("{0,-35}", "Description: ");
            mpcostInfo +=  String.Format("{0,-10}", "MP Cost: ");
            nameInfo += "\n\n";
            descriptionInfo += "\n\n";
            mpcostInfo += "\n\n";
            for(int i = 0; i < validSpecials.Count; i++)
            {
                //Padding on the bottom to make sure that the highlight box remains consistently stable
                int bottomPadding = 3;
                Special theSpecial = battleManager.specialManager.GetComponent<SpecialManager>().allSpecials[validSpecials[i]];
                nameInfo += String.Format("{0,-15}", theSpecial.name); 
                mpcostInfo +=  String.Format("{0,-5}", theSpecial.mpcost);
                String description = theSpecial.description;
                String[] words = description.Split();
                String line = "";
                foreach(string word in words)
                {
                    line += word + " ";
                    if(line.Length >= 45)
                    {
                        descriptionInfo += line;
                        line = "";
                        bottomPadding--;
                    }
                }
                descriptionInfo += line;
                nameInfo += "\n\n\n";
                mpcostInfo += "\n\n\n";
                for(int j = 0; j < bottomPadding; j++)
                {
                    descriptionInfo += "\n";
                }
            }
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if(battleManager.playerList[battleManager.currentPlayerIndex].GetComponent<Character>().stats.currentmp >= battleManager.specialManager.GetComponent<SpecialManager>().allSpecials[validSpecials[focusedIndex]].mpcost)
                {
                    specialMenu.SetActive(false);
                    highlightBox.SetActive(false);
                    chosenSpecial = validSpecials[focusedIndex];
                    focusedIndex = 0;
                    mode = 2.5f;
                }
            }
            specialMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(nameInfo);
            specialMenu.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText(descriptionInfo);
            specialMenu.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().SetText(mpcostInfo);
            optionDescription.GetComponent<TextMeshProUGUI>().SetText("Select a spell.");
        }
        else if(mode == 2.5)
        {
            int targeting = battleManager.specialManager.GetComponent<SpecialManager>().allSpecials[chosenSpecial].targeting;
            if(targeting == 0)
            {
                optionDescription.GetComponent<TextMeshProUGUI>().SetText("Select an enemy to target.");
                optionDescription.SetActive(true);
                showInactiveMenu();
                if(Keyboard.current.xKey.wasPressedThisFrame)
                {
                    battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    mode = 2;
                    focusedIndex = 0;
                }   
                if(Keyboard.current.leftArrowKey.wasPressedThisFrame && focusedIndex > 0)
                {
                    battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    focusedIndex--;
                }
                if(Keyboard.current.rightArrowKey.wasPressedThisFrame &&  focusedIndex < battleManager.enemyList.Count - 1)
                {
                    battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    focusedIndex++;
                }  
                battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.blue;
                if(Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    battleManager.enemyList[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    battleManager.allySpecial(chosenSpecial, focusedIndex);
                    mode = 0;
                    focusedIndex = 0;
                    optionDescription.SetActive(false);
                }
            }
            else if(targeting == 1)
            {
                battleManager.allySpecial(chosenSpecial, -1);
                mode = 0;
                focusedIndex = 0;
            }
            else if(targeting == 2)
            {
                optionDescription.GetComponent<TextMeshProUGUI>().SetText("Select an ally to target.");
                optionDescription.SetActive(true);
                showInactiveMenu();
                if(Keyboard.current.xKey.wasPressedThisFrame)
                {
                    portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    mode = 2;
                    focusedIndex = 0;
                }   
                if(Keyboard.current.leftArrowKey.wasPressedThisFrame && focusedIndex > 0)
                {
                    portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    focusedIndex--;
                }
                if(Keyboard.current.rightArrowKey.wasPressedThisFrame &&  focusedIndex < portraits.Count - 1)
                {
                    portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    focusedIndex++;
                }  
                portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.blue;
                if(Keyboard.current.enterKey.wasPressedThisFrame && battleManager.playerList[focusedIndex].GetComponent<Character>().stats.currenthp > 0)
                {
                    portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    battleManager.allySpecial(chosenSpecial, focusedIndex);
                    mode = 0;
                    focusedIndex = 0;
                    optionDescription.SetActive(false);
                }
            }
            else if(targeting == 3)
            {
                battleManager.allySpecial(chosenSpecial, -1);
                mode = 0;
                focusedIndex = 0;
            }
            else if(targeting == 4)
            {
                battleManager.allySpecial(chosenSpecial, -1);     
                mode = 0;
                focusedIndex = 0;
            }
        }
        else if(mode == 3)
        {
            highlightBox.SetActive(true);
            specialMenu.SetActive(true);
            optionDescription.SetActive(true);
            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                mode = 0;
                focusedIndex = 0;
                highlightBox.SetActive(false);
                specialMenu.SetActive(false);
            }       
            if(Keyboard.current.upArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                focusedIndex--;
            }
            if(Keyboard.current.downArrowKey.wasPressedThisFrame && focusedIndex < SaveManager.instance.inventory.Count - 1)
            {
                focusedIndex++;
            }
            highlightBox.transform.position = new Vector3(0, -1.35f * focusedIndex + 2f, -1);
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                specialMenu.SetActive(false);
                highlightBox.SetActive(false);
                chosenItem = itemsToChoose[focusedIndex];
                focusedIndex = 0;
                mode = 3.5f;
            }
            string nameInfo = "";
            string descriptionInfo = "";
            string quantityInfo = "";
            nameInfo += String.Format("{0,-15}", "Name: "); 
            descriptionInfo +=  String.Format("{0,-45}", "Description: ");
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
                Item theItem = battleManager.itemManager.GetComponent<ItemManager>().allItems[item.Key];
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
            specialMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(nameInfo);
            specialMenu.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText(descriptionInfo);
            specialMenu.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().SetText(quantityInfo);
        }
        else if(mode == 3.5)
        {
                optionDescription.GetComponent<TextMeshProUGUI>().SetText("Select an ally to target.");
                optionDescription.SetActive(true);
                showInactiveMenu();
                if(Keyboard.current.xKey.wasPressedThisFrame)
                {
                    portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    mode = 3;
                    focusedIndex = 0;
                }   
                if(Keyboard.current.leftArrowKey.wasPressedThisFrame && focusedIndex > 0)
                {
                    portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    focusedIndex--;
                }
                if(Keyboard.current.rightArrowKey.wasPressedThisFrame &&  focusedIndex < portraits.Count - 1)
                {
                    portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    focusedIndex++;
                }  
                portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.blue;
                //Use item on nondead ally unless that item is an elixir
                if(Keyboard.current.enterKey.wasPressedThisFrame && (battleManager.playerList[focusedIndex].GetComponent<Character>().stats.currenthp > 0 || chosenItem == 4))
                {
                    portraits[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                    battleManager.allyItem(chosenItem, focusedIndex);
                    mode = 0;
                    focusedIndex = 0;
                    optionDescription.SetActive(false);
                }   
        }
        else if(mode == 6 && !finished)
        {
            optionDescription.SetActive(false);
            // Debug.Log("DEFEAT");
        }
        else if(mode == 7)
        {
            //Wait until enemies are done attacking or is attacking
            optionDescription.SetActive(false);
            showInactiveMenu();
            if(!battleManager.enemyTurn() && !battleManager.specialManager.GetComponent<SpecialManager>().activated)
            {
                mode = 0;
            }
        }
        for(int i = 0; i < battleManager.playerList.Count; i++)
        {
            string playerInfo = "Hp: " + Math.Ceiling(battleManager.playerList[i].GetComponent<Character>().stats.currenthp) + "/" + Math.Ceiling(battleManager.playerList[i].GetComponent<Character>().finalmaxhp) + "\n";
            playerInfo += "Mp: " + Math.Ceiling(battleManager.playerList[i].GetComponent<Character>().stats.currentmp) + "/" + Math.Ceiling(battleManager.playerList[i].GetComponent<Character>().finalmaxmp);
            playerStats.transform.GetChild(i).gameObject.GetComponent<TextMeshProUGUI>().SetText(playerInfo);
        }
        //Making dead players transparent
        //Current player highlighted
        for(int i = 0; i < battleManager.playerList.Count; i++)
        {
            Color baseColor = battleManager.playerList[i].GetComponent<SpriteRenderer>().color;
            if(battleManager.playerList[i].GetComponent<Character>().stats.currenthp <= 0)
            {
                baseColor[3] = 0.25f;
            }
            else
            {
                baseColor[3] = 1;
            }
            if(i == battleManager.currentPlayerIndex)
            {
                baseColor[2] = 0;
            }
            else
            {
                baseColor[2] = 1;
            }
            battleManager.playerList[i].GetComponent<SpriteRenderer>().color = baseColor;
        }

 
    }

    public void showActiveMenu()
    {
        actionMenu.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void showInactiveMenu()
    {
        actionMenu.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
    }
    public void updatePlayerValues()
    {
        for(int i = 0; i < battleManager.playerList.Count; i++)
        {
            SaveManager.instance.gameData.playerStats[i].currenthp = battleManager.playerList[i].GetComponent<Character>().stats.currenthp;
            SaveManager.instance.gameData.playerStats[i].currentmp = battleManager.playerList[i].GetComponent<Character>().stats.currentmp;
        }
    }

    // public void useSpecial()
    // {
    //     Special theSpecial = specialList[focusedIndex];
    // }
    public IEnumerator escape()
    {
        while(true)
        {
            if(mode == 4)
            {
                bool escaped = battleManager.escape();
                if(!escaped)
                {
                    optionDescription.SetActive(true);
                    optionDescription.GetComponent<TextMeshProUGUI>().SetText("Escape failed!");
                    yield return new WaitForSeconds(1);
                    //Failed escape should still take up a turn
                    //Should be in battleManager but simpler to put here to not worry about timing
                    battleManager.currentPlayerIndex++;
                    mode = 0;
                }
                else
                {
                    optionDescription.SetActive(true);
                    optionDescription.GetComponent<TextMeshProUGUI>().SetText("Escape successful!");
                    updatePlayerValues();
                    yield return new WaitForSeconds(1);
                    StartCoroutine(SaveManager.instance.switchToScene("WorldScene"));
                    break;
                }
            }
            yield return null;
        }
    }
    public IEnumerator showVictory()
    {
        while(mode != 5)
        {
            yield return null;
        }
        optionDescription.SetActive(false);
        updatePlayerValues();
        yield return new WaitForSeconds(2);
        Debug.Log("VICTORY");
        Debug.Log("OBTAINED " + battleManager.expGain + " EXP");
        Debug.Log("OBTAINED " + battleManager.moneyGain + " MONEY");
        actionMenu.SetActive(false);
        //Add victory screen here
        // returnButton.SetActive(true);
        StartCoroutine(SaveManager.instance.switchToScene("WorldScene"));
        yield return null;
    }
}
