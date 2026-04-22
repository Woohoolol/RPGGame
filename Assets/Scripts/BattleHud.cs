using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class BattleHud : MonoBehaviour
{
    //0 attack, 1 special, 2 item, 3 escape
    public List<GameObject> actions;
    public GameObject actionMenu;
    public BattleManager battleManager;
    public GameObject specialMenu;
    public GameObject playerStats;
    public GameObject optionDescription;
    public GameObject returnButton;
    public GameObject selectionScreen;
    //0 main menu focused, 1 attack menu, 2 special menu, 3 item, 4 escape, 5 victory, 6 defeat, 7 enemy attack, 8 waiting for input (victory/defeat)
    public int mode;
    private int focusedIndex;
    private int attackIndex;
    public int oldPlayerIndex;
    private bool finished;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finished = false;
        focusedIndex = 0;
        mode = 0;
        for(int i = 0; i < SaveManager.instance.playerList.Count; i++)
        {    
            GameObject player = Instantiate(SaveManager.instance.playerList[i], playerStats.transform.GetChild(i).position + new Vector3(0, 4f, -1), Quaternion.Euler(0, 0, 0));
            battleManager.playerList.Add(player);
            player.transform.parent = selectionScreen.transform;
        } 
        for(int i = 0; i < battleManager.playerList.Count; i++)
        {
            playerStats.transform.GetChild(i).gameObject.SetActive(true);
            GameObject portrait = Instantiate(SaveManager.instance.portraits[battleManager.playerList[i].GetComponent<Character>().stats.characterType], 
            playerStats.transform.GetChild(i).position + new Vector3(0, 1.75f, -1), Quaternion.Euler(0, 1, 0));
            portrait.transform.parent = selectionScreen.transform;
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
        else if(battleManager.enemyTurn())
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
                if(focusedIndex == 0)
                {
                    Debug.Log("ATTACK");
                    mode = 1;
                    attackIndex = 0;
                }
                else if(focusedIndex == 1)
                {
                    Debug.Log("SPECIAL");
                    mode = 2;
                }
                else if(focusedIndex == 2)
                {
                    Debug.Log("GUARD");
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
            optionDescription.GetComponent<TextMeshProUGUI>().SetText("Select a target.");
            optionDescription.SetActive(true);
            showInactiveMenu();
            if(Keyboard.current.leftArrowKey.wasPressedThisFrame && attackIndex > 0)
            {
                battleManager.enemyList[attackIndex].GetComponent<SpriteRenderer>().color = Color.white;
                attackIndex--;
            }
            if(Keyboard.current.rightArrowKey.wasPressedThisFrame && attackIndex < battleManager.enemyList.Count - 1)
            {
                battleManager.enemyList[attackIndex].GetComponent<SpriteRenderer>().color = Color.white;
                attackIndex++;
            }  
            battleManager.enemyList[attackIndex].GetComponent<SpriteRenderer>().color = Color.blue;
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                bool killed = battleManager.allyAttack(attackIndex);
                if(!killed)
                {
                    //Checking if that spot is destroyed before resetting color
                    battleManager.enemyList[attackIndex].GetComponent<SpriteRenderer>().color = Color.white;
                }
                //Reset to top of menu after action
                mode = 0;
                focusedIndex = 0;
                //If something killed need to reset index to avoid out of bounds
                attackIndex = 0;
            }
            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                battleManager.enemyList[attackIndex].GetComponent<SpriteRenderer>().color = Color.white;
                mode = 0;
                focusedIndex = 0;     
            }
        }
        else if(mode == 6 && !finished)
        {
            optionDescription.SetActive(false);
            // Debug.Log("DEFEAT");
        }
        else if(mode == 7)
        {
            //Wait until enemies are done attacking
            optionDescription.SetActive(false);
            showInactiveMenu();
            if(!battleManager.enemyTurn())
            {
                mode = 0;
            }
        }
        for(int i = 0; i < battleManager.playerList.Count; i++)
        {
            string playerInfo = "hp: " + battleManager.playerList[i].GetComponent<Character>().currenthp + " mp: " + battleManager.playerList[i].GetComponent<Character>().currentmp;
            playerStats.transform.GetChild(i).gameObject.GetComponent<TextMeshProUGUI>().SetText(playerInfo);
        }
        //Making dead players transparent
        for(int i = 0; i < battleManager.playerList.Count; i++)
        {
            Color baseColor = battleManager.playerList[i].GetComponent<SpriteRenderer>().color;
            if(battleManager.playerList[i].GetComponent<Character>().currenthp <= 0)
            {
                baseColor[3] = 0.25f;
                battleManager.playerList[i].GetComponent<SpriteRenderer>().color = baseColor;
            }
            else
            {
                baseColor[3] = 1;
                battleManager.playerList[i].GetComponent<SpriteRenderer>().color = baseColor;
            }
        }
        //Current player highlighted
        if(oldPlayerIndex != battleManager.currentPlayerIndex)
        {
            if(oldPlayerIndex < battleManager.playerList.Count)
            {
                battleManager.playerList[oldPlayerIndex].GetComponent<SpriteRenderer>().color = Color.white;
            }
            oldPlayerIndex = battleManager.currentPlayerIndex;
        }
        if(battleManager.currentPlayerIndex < battleManager.playerList.Count)
        {
            battleManager.playerList[battleManager.currentPlayerIndex].GetComponent<SpriteRenderer>().color = Color.yellow;
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
                    yield return new WaitForSeconds(1);
                    battleManager.switchBack();
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
        yield return new WaitForSeconds(2);
        Debug.Log("VICTORY");
        Debug.Log("OBTAINED " + battleManager.expGain + " EXP");
        Debug.Log("OBTAINED " + battleManager.moneyGain + " MONEY");
        actionMenu.SetActive(false);
        //Add victory screen here
        returnButton.SetActive(true);
        yield return null;
    }
}
