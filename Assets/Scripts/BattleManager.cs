using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class BattleManager : MonoBehaviour
{
    public List<GameObject> playerList;
    public int currentPlayerIndex;
    public List<GameObject> enemyList;
    public int currentEnemyIndex;
    public float expGain;
    public float moneyGain;
    public float attackTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        expGain = 0;
        moneyGain = 0;
        currentPlayerIndex = 0;
        currentEnemyIndex = 0;
        for(int i = 0; i < enemyList.Count; i++)
        {
            expGain += enemyList[i].GetComponent<Character>().stats.enemyExp;
            moneyGain += enemyList[i].GetComponent<Character>().stats.enemyMoney;
        }
        StartCoroutine(enemyAttack());
        StartCoroutine(victoryReward());
    }

    // Update is called once per frame
    void Update()
    {
        //Check out of bounds first!
        while(currentPlayerIndex < playerList.Count && playerList[currentPlayerIndex].GetComponent<Character>().stats.hp <= 0)
        {
            currentPlayerIndex++;
        }
    }

    public void switchBack()
    {
        SaveManager.instance.switchToWorldScene();
    }
    

    public bool allyAttack(int enemyIndex)
    {
        // attackTimer
        CharacterStats attackingPlayer = playerList[currentPlayerIndex].GetComponent<Character>().stats;
        CharacterStats attackedEnemy =  enemyList[enemyIndex].GetComponent<Character>().stats;
        Animator attackAnimation = playerList[currentPlayerIndex].GetComponent<Animator>();
        attackedEnemy.hp -= (attackingPlayer.physical -  attackedEnemy.pdefense);
        Debug.Log("ATTACKED " + enemyIndex + " FOR " + (attackingPlayer.physical -  attackedEnemy.pdefense) + " HP ");
        attackAnimation.Play("Attack");
        currentPlayerIndex++;
        if(attackedEnemy.hp <= 0)
        {
            Debug.Log("ELIMINATED");
            Destroy(enemyList[enemyIndex]);
            enemyList.RemoveAt(enemyIndex);
            return true;
        }
        else
        {
            return false;
        }
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
            if(playerList[i].GetComponent<Character>().stats.hp > 0)
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
        int roll = Random.Range(0, 101);
        return roll >= escapeRequirement;
    }
    public IEnumerator enemyAttack()
    {
        while(true)
        {
            if(playerList.Count != 0 && enemyTurn())
            {
                yield return new WaitForSeconds(2);
                for(currentEnemyIndex = 0; currentEnemyIndex < enemyList.Count; currentEnemyIndex++)
                {
                    //Only target characters that are alive
                    List<int> validAttackingIndex = new List<int>();
                    for(int i = 0; i < playerList.Count; i++)
                    {
                        if(playerList[i].GetComponent<Character>().stats.hp > 0)
                        {
                            validAttackingIndex.Add(i);
                        }
                    }
                    int attackingIndex = validAttackingIndex[Random.Range(0, validAttackingIndex.Count)];

                    CharacterStats attackingEnemy = enemyList[currentEnemyIndex].GetComponent<Character>().stats;
                    //Int version of random range is exclusive on second number
                    //Float version of random range is inclusive on both
                    CharacterStats attackedAlly =  playerList[attackingIndex].GetComponent<Character>().stats;
                    attackedAlly.hp -= (attackingEnemy.physical -  attackedAlly.pdefense);
                    Debug.Log("ATTACKED ALLY" + " FOR " + (attackingEnemy.physical -  attackedAlly.pdefense) + " HP ");

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
        // SaveManager.instance.gameData.exp += expGain;
        SaveManager.instance.gameData.money += moneyGain;
        yield return null;
    }
}
