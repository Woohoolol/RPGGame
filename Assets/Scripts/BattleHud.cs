using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class BattleHud : MonoBehaviour
{
    //0 attack, 1 special, 2 guard, 3 escape
    public GameObject[] actions;
    public GameObject actionMenu;
    public BattleManager battleManager;
    public GameObject specialMenu;
    public GameObject[] enemyVisuals;
    //0 main menu focused, 1 attack menu, 2 special menu, 3 guard, 4 escape, 5 victory, 6 defeat, 7 enemy attack
    public int mode;
    private int focusedIndex;
    private int attackIndex;
    public bool focused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        focusedIndex = 0;
        focused = true;
        mode = 0;
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
            showActiveMenu();
            if(Keyboard.current.upArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                //Unhighlight option before moving
                actions[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
                focusedIndex--;
            }
            if(Keyboard.current.downArrowKey.wasPressedThisFrame && focusedIndex < actions.Length - 1)
            {
                actions[focusedIndex].GetComponent<SpriteRenderer>().color = Color.white;
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
                }
                else if(focusedIndex == 2)
                {
                    Debug.Log("GUARD");
                }
                else if(focusedIndex == 3)
                {
                    Debug.Log("ESCAPE");
                }
            }
        }
        else if(mode == 1)
        {
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
            battleManager.playerList[battleManager.currentPlayerIndex].GetComponent<SpriteRenderer>().color = Color.yellow;
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                battleManager.playerList[battleManager.currentPlayerIndex].GetComponent<SpriteRenderer>().color = Color.white;
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
                battleManager.playerList[battleManager.currentPlayerIndex].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        else if(mode == 5)
        {
            Debug.Log("VICTORY");
            Debug.Log("OBTAINED " + battleManager.expGain + " EXP");
            Debug.Log("OBTAINED " + battleManager.moneyGain + " MONEY");

            // battleManager.g
        }
        else if(mode == 6)
        {
            Debug.Log("DEFEAT");
        }
        else if(mode == 7)
        {
            //Wait until enemies are done attacking
            showInactiveMenu();
            if(!battleManager.enemyTurn())
            {
                mode = 0;
            }
        }
    }

    public void showActiveMenu()
    {
        actionMenu.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
    }

    public void showInactiveMenu()
    {
        actionMenu.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.5f);
    }

    public void showVictory()
    {
        actionMenu.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0f);
        //Add victory screen here
    }
}
