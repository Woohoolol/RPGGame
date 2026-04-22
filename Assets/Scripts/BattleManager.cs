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
    void Awake()
    {
        //When entering random encounter, make a copy of gamedata team to fight with
        playerList = new List<GameObject>();
        for(int i = 0; i < SaveManager.instance.playerList.Count; i++)
        {
            SaveManager.instance.playerList[i].GetComponent<Character>().currenthp = SaveManager.instance.playerList[i].GetComponent<Character>().maxhp;
            SaveManager.instance.playerList[i].GetComponent<Character>().currentmp = SaveManager.instance.playerList[i].GetComponent<Character>().maxmp;
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
        while(currentPlayerIndex < playerList.Count && playerList[currentPlayerIndex].GetComponent<Character>().currenthp <= 0)
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
        Character attackingPlayer = playerList[currentPlayerIndex].GetComponent<Character>();
        Character attackedEnemy =  enemyList[enemyIndex].GetComponent<Character>();
        Animator attackAnimation = playerList[currentPlayerIndex].GetComponent<Animator>();
        attackedEnemy.currenthp -= (attackingPlayer.physical -  attackedEnemy.pdefense);
        Debug.Log("ATTACKED " + enemyIndex + " FOR " + (attackingPlayer.physical -  attackedEnemy.pdefense) + " HP ");
        attackAnimation.Play("Attack");
        currentPlayerIndex++;
        if(attackedEnemy.currenthp <= 0)
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
            if(playerList[i].GetComponent<Character>().currenthp > 0)
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
                        if(playerList[i].GetComponent<Character>().currenthp > 0)
                        {
                            validAttackingIndex.Add(i);
                        }
                    }
                    int attackingIndex = validAttackingIndex[Random.Range(0, validAttackingIndex.Count)];

                    Character attackingEnemy = enemyList[currentEnemyIndex].GetComponent<Character>();
                    //Int version of random range is exclusive on second number
                    //Float version of random range is inclusive on both
                    Character attackedAlly =  playerList[attackingIndex].GetComponent<Character>();
                    attackedAlly.currenthp -= (attackingEnemy.physical -  attackedAlly.pdefense);
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
        for(int i = 0; i < SaveManager.instance.gameData.playerStats.Count; i++)
        {
            SaveManager.instance.gameData.playerStats[i].exp += expGain;
        }
        SaveManager.instance.gameData.money += moneyGain;
        yield return null;
    }
}
