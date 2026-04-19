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
            expGain += enemyList[i].GetComponent<Character>().exp;
            moneyGain += enemyList[i].GetComponent<Character>().money;
        }
        StartCoroutine(enemyAttack());
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].GetComponent<Character>().hp <= 0)
            {
                Debug.Log("ELIMINATED ALLY");
                playerList.RemoveAt(i);
            }
        }
    }

    public void switchBack()
    {
        SaveManager.instance.switchToWorldScene();
    }
    
    public bool allyAttack(int enemyIndex)
    {
        // attackTimer
        Character attackingPlayer = playerList[currentPlayerIndex].GetComponent<Character>();
        Character attackedEnemy =  enemyList[enemyIndex].GetComponent<Character>();
        attackedEnemy.hp -= (attackingPlayer.physical -  attackedEnemy.pdefense);
        Debug.Log("ATTACKED " + enemyIndex + " FOR " + (attackingPlayer.physical -  attackedEnemy.pdefense) + " HP ");
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

    public bool defeat()
    {
        return playerList.Count == 0;
    }
    public bool enemyTurn()
    {
        return currentPlayerIndex >= playerList.Count;
    }
    // public bool enemyAttack()
    // {
    //     for(int i = 0; i < enemyList.Count; i++)
    //     {
    //         Character attackingEnemy = enemyList[i].GetComponent<Character>();
    //         //Int version of random range is exclusive on second number
    //         //Float version of random range is inclusive on both
    //         Character attackedAlly =  playerList[Random.Range(0, playerList.Count)].GetComponent<Character>();
    //         attackedAlly.hp -= (attackingEnemy.physical -  attackedAlly.pdefense);
    //         Debug.Log("ATTACKED ALLY" + " FOR " + (attackingEnemy.physical -  attackedAlly.pdefense) + " HP ");
    //         if(attackedAlly.hp <= 0)
    //         {
    //             Debug.Log("ELIMINATED ALLY");
    //             playerList.RemoveAt(0);
    //         }
    //         //Enemies should stop hitting when everyone dead
    //         if(defeat())
    //         {
    //             Debug.Log("ENEMIES STOP");
    //             break;
    //         }
    //     }
    //     currentPlayerIndex = 0;
    //     return true;
    // }
    public IEnumerator enemyAttack()
    {
        while(true)
        {
            if(playerList.Count != 0 && enemyTurn())
            {
                for(currentEnemyIndex = 0; currentEnemyIndex < enemyList.Count; currentEnemyIndex++)
                {
                    int attackingIndex = Random.Range(0, playerList.Count);
                    Character attackingEnemy = enemyList[currentEnemyIndex].GetComponent<Character>();
                    //Int version of random range is exclusive on second number
                    //Float version of random range is inclusive on both
                    Character attackedAlly =  playerList[attackingIndex].GetComponent<Character>();
                    attackedAlly.hp -= (attackingEnemy.physical -  attackedAlly.pdefense);
                    Debug.Log("ATTACKED ALLY" + " FOR " + (attackingEnemy.physical -  attackedAlly.pdefense) + " HP ");

                    //Enemies should stop hitting when everyone dead
                    if(defeat())
                    {
                        Debug.Log("ENEMIES STOP");
                        break;
                    }
                    //Should be in battlehud but easier to get timings here for highlighting
                    enemyList[currentEnemyIndex].GetComponent<SpriteRenderer>().color = Color.red;
                    yield return new WaitForSeconds(1);
                    enemyList[currentEnemyIndex].GetComponent<SpriteRenderer>().color = Color.white;
                }
                currentPlayerIndex = 0;
            }
            yield return null;
        }

    }
}
