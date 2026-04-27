using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
public class BattleManager : MonoBehaviour
{
    public List<GameObject> playerList;
    public int currentPlayerIndex;
    public List<GameObject> enemyList;
    public int currentEnemyIndex;
    public GameObject specialManager;
    public GameObject itemManager;
    public float expGain;
    public float moneyGain;
    public float attackTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //When entering random encounter, make a copy of gamedata team to fight with
        playerList = new List<GameObject>();
        for(int i = 0; i < SaveManager.instance.playerList.Count; i++)
        {
            // SaveManager.instance.playerList[i].GetComponent<Character>().stats.currenthp = SaveManager.instance.playerList[i].GetComponent<Character>().finalmaxhp;\
            // SaveManager.instance.playerList[i].GetComponent<Character>().stats.currentmp = SaveManager.instance.playerList[i].GetComponent<Character>().finalmaxmp;
        }
    }
    void Start()
    {
        expGain = 0;
        moneyGain = 0;
        currentPlayerIndex = 0;
        currentEnemyIndex = 0;
        for(int i = 0; i < enemyList.Count; i++)
        {
            expGain += enemyList[i].GetComponent<Character>().enemyExp;
            moneyGain += enemyList[i].GetComponent<Character>().enemyMoney;
        }
        StartCoroutine(enemyAttack());
        StartCoroutine(victoryReward());
    }

    // Update is called once per frame
    void Update()
    {
        //Check out of bounds first!
        while(currentPlayerIndex < playerList.Count && playerList[currentPlayerIndex].GetComponent<Character>().stats.currenthp <= 0)
        {
            currentPlayerIndex++;
        }
        for(int i = 0; i < playerList.Count; i++)
        {
            //Cannot initialize to final values as they cannot be set in the list
            if((playerList[i].GetComponent<Character>().stats.currenthp) > playerList[i].GetComponent<Character>().basemaxhp)
            {
                playerList[i].GetComponent<Character>().stats.currenthp = playerList[i].GetComponent<Character>().basemaxhp;
            }
            if((playerList[i].GetComponent<Character>().stats.currentmp) > playerList[i].GetComponent<Character>().basemaxmp)
            {
                playerList[i].GetComponent<Character>().stats.currentmp = playerList[i].GetComponent<Character>().basemaxmp;
            }
        }
        for(int i = 0; i < enemyList.Count; i++)
        {
            if((enemyList[i].GetComponent<Character>().stats.currenthp) > enemyList[i].GetComponent<Character>().basemaxhp)
            {
                enemyList[i].GetComponent<Character>().stats.currenthp = enemyList[i].GetComponent<Character>().basemaxhp;
            }
            if((enemyList[i].GetComponent<Character>().stats.currentmp) > enemyList[i].GetComponent<Character>().basemaxmp)
            {
                enemyList[i].GetComponent<Character>().stats.currentmp = enemyList[i].GetComponent<Character>().basemaxmp;
            }
            if(enemyList[i].GetComponent<Character>().stats.currenthp <= 0)
            {
                Debug.Log("ELIMINATED");
                Destroy(enemyList[i]);
                enemyList.RemoveAt(i);
            }
        }

    }

    public void switchBack()
    {
        SaveManager.instance.switchToWorldScene();
    }
    

    public void allyAttack(int enemyIndex)
    {
        // attackTimer
        Character attackingPlayer = playerList[currentPlayerIndex].GetComponent<Character>();
        Character attackedEnemy =  enemyList[enemyIndex].GetComponent<Character>();
        Animator attackAnimation = playerList[currentPlayerIndex].GetComponent<Animator>();
        //Third attackingPlayer.physical is to mitigate high atk/def differences
        attackedEnemy.stats.currenthp -= (float)Math.Ceiling((double)(attackingPlayer.finalphysical * (attackingPlayer.finalphysical/(1 + 0.75 * attackingPlayer.finalphysical + Math.Pow(attackedEnemy.finalpdefense, 0.75)))));
        attackAnimation.Play("Attack");
        currentPlayerIndex++;
    }

    public void allySpecial(int specialIndex, int theEnemyIndex)
    {
        Special theSpecial = specialManager.GetComponent<SpecialManager>().allSpecials[specialIndex];
        specialManager.GetComponent<SpecialManager>().activateSpecial(specialIndex, playerIndex: currentPlayerIndex, enemyIndex: theEnemyIndex);
        playerList[currentPlayerIndex].GetComponent<Character>().stats.currentmp -= theSpecial.mpcost;
        currentPlayerIndex++;
    }

    public void allyItem(int itemID, int theAllyIndex)
    {
        itemManager.GetComponent<ItemManager>().activateItem(itemID, playerIndex: theAllyIndex);
        currentPlayerIndex++;
    }
    public bool victory()
    {
        return enemyList.Count == 0;
    }

    //Unlike enemies we do not want to remove them as they are important
    public bool defeat()
    {
        bool wiped = true;
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].GetComponent<Character>().stats.currenthp > 0)
            { 
                wiped = false;
            }
        }
        return wiped;
    }
    public bool enemyTurn()
    {
        return currentPlayerIndex >= playerList.Count;
    }



    public bool escape()
    {
        int escapeRequirement= 75;
        int roll = UnityEngine.Random.Range(0, 101);
        return roll >= escapeRequirement;
    }
    public IEnumerator enemyAttack()
    {
        while(true)
        {
            if(!defeat() && enemyTurn())
            {
                yield return new WaitForSeconds(2);
                for(currentEnemyIndex = 0; currentEnemyIndex < enemyList.Count; currentEnemyIndex++)
                {
                    //Only target characters that are alive
                    List<int> validAttackingIndex = new List<int>();
                    for(int i = 0; i < playerList.Count; i++)
                    {
                        if(playerList[i].GetComponent<Character>().stats.currenthp > 0)
                        {
                            validAttackingIndex.Add(i);
                        }
                    }
                    int attackingIndex = validAttackingIndex[UnityEngine.Random.Range(0, validAttackingIndex.Count)];

                    Character attackingEnemy = enemyList[currentEnemyIndex].GetComponent<Character>();
                    //Int version of random range is exclusive on second number
                    //Float version of random range is inclusive on both
                    Character attackedAlly =  playerList[attackingIndex].GetComponent<Character>();
                    attackedAlly.stats.currenthp -= (float)Math.Ceiling((double)(attackingEnemy.finalphysical * (attackingEnemy.finalphysical/(1 + 0.75 * attackingEnemy.finalphysical + Math.Pow(attackedAlly.finalpdefense, 0.75)))));
                    //Enemies should stop hitting when everyone dead
                    if(defeat())
                    {
                        Debug.Log("ENEMIES STOP");
                        break;
                    }
                    //Should be in battlehud but easier to get timings here for highlighting
                    enemyList[currentEnemyIndex].GetComponent<SpriteRenderer>().color = Color.yellow;
                    yield return new WaitForSeconds(1);
                    enemyList[currentEnemyIndex].GetComponent<SpriteRenderer>().color = Color.white;
                }
                for(int i = 0; i < playerList.Count; i++)
                {
                    playerList[i].GetComponent<Character>().modifierDecay();
                }
                for(int i = 0; i < enemyList.Count; i++)
                {
                    enemyList[i].GetComponent<Character>().modifierDecay();
                }
                currentPlayerIndex = 0;
            }
            yield return null;
        }
    }
    public IEnumerator victoryReward()
    {
        while(!victory())
        {
            yield return null;
        }
        for(int i = 0; i < SaveManager.instance.gameData.playerStats.Count; i++)
        {
            SaveManager.instance.gameData.playerStats[i].exp += expGain;
        }
        SaveManager.instance.gameData.money += moneyGain;
        int itemAcquired = UnityEngine.Random.Range(0, 5);
        itemManager.GetComponent<ItemManager>().acquiredItem(itemAcquired, 3);
        yield return null;
    }
}
