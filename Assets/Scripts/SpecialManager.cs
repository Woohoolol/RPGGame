using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
public class SpecialManager : MonoBehaviour
{
    public List<Special> allSpecials;
    public int specialIndex;
    public GameObject currentPlayer;
    public GameObject battleManager;
    public int playerIndex;
    public int enemyIndex;
    public bool activated;
    float damage = 0;
    float healing = 0;
    //Constructor requirements: Name, Description, Targeting
    //Constructor options: Intensity, numberOfHits, isPhysical, isMagic, isHealing, isBuff, isDebuff
    //buffStats (when isBuff = true), debuffStats (when isDebuff = true)
    void Awake()
    {
        allSpecials = new List<Special>();
        allSpecials.Add(new Special("Double Hit", "Hits an enemy twice for 0.75x physical.", 5, 0, intensity: 0.75f, numberOfHits: 2, isPhysical: true));
        allSpecials.Add(new Special("Respite", "Recovers an ally's hp for 0.5x mental.", 4, 2, intensity: 0.5f, numberOfHits: 1, isHealing: true, isMental: true));
        allSpecials.Add(new Special("Arrow Storm", "Strikes all enemies 3 times for 0.33x physical.", 7, 1, intensity: 0.33f, numberOfHits: 3, isPhysical: true));
        allSpecials.Add(new Special("Meteor", "Strikes an enemy powerfully for 2x mental.", 6, 0, intensity: 2f, numberOfHits: 1, isMental: true));

    }
    void Start()
    {
        StartCoroutine(useSpecial());
    }
    
    void Update()
    {

    }

    public void activateSpecial(int specialIndex, GameObject currentPlayer, int playerIndex = 0, int enemyIndex = 0)
    {
        activated = true;
        this.specialIndex = specialIndex;
        this.currentPlayer = currentPlayer;
        this.playerIndex = playerIndex;
        this.enemyIndex = enemyIndex;
    }

    public void damageHealingCalculation(Special theSpecial, Character thePlayer, Character theEnemy)
    {
        if(theSpecial.isPhysical)
        {
            if(theSpecial.isHealing)
            {
                healing = thePlayer.physical * theSpecial.intensity;
            }
            else
            {
                damage = (float)Math.Ceiling((double)(thePlayer.physical * theSpecial.intensity * thePlayer.physical/(1 + 0.75 * thePlayer.physical + theEnemy.pdefense)));
            }
        }
        else if(theSpecial.isMental)
        {
            if(theSpecial.isHealing)
            {
                healing = thePlayer.mental * theSpecial.intensity;
            }
            else
            {
                damage = (float)Math.Ceiling((double)(thePlayer.mental * theSpecial.intensity * thePlayer.mental/(1 + 0.75 * thePlayer.mental + theEnemy.mdefense)));
            }
        }
    }
    public IEnumerator useSpecial()
    {
        while(true)
        {
            yield return new WaitUntil(() => activated);
            Special theSpecial = allSpecials[specialIndex];
            Character thePlayer = currentPlayer.GetComponent<Character>();
            BattleManager theBattleManager = battleManager.GetComponent<BattleManager>();
            if(theSpecial.targeting == 0)
            {
                Character theEnemy = theBattleManager.enemyList[enemyIndex].GetComponent<Character>();
                damageHealingCalculation(theSpecial, thePlayer, theEnemy);
                for(int i = 0; i < theSpecial.numberOfHits; i++)
                {
                    theEnemy.stats.currenthp -= damage;
                    theEnemy.stats.currenthp += healing;
                    Debug.Log("HEALED ENEMY FOR " + healing + " DAMAGED ENEMY FOR " + damage);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else if(theSpecial.targeting == 1)
            {
                for(int i = 0; i < theBattleManager.enemyList.Count; i++)
                {
                    Character theEnemy = theBattleManager.enemyList[i].GetComponent<Character>();
                    damageHealingCalculation(theSpecial, thePlayer, theEnemy);
                    for(int j = 0; j < theSpecial.numberOfHits; j++)
                    {
                        theEnemy.stats.currenthp -= damage;
                        theEnemy.stats.currenthp += healing;
                        Debug.Log("HEALED ENEMY FOR " + healing + " DAMAGED ENEMY FOR " + damage);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            else if(theSpecial.targeting == 2)
            {
                Character theEnemy = theBattleManager.playerList[playerIndex].GetComponent<Character>();
                damageHealingCalculation(theSpecial, thePlayer, theEnemy);
                for(int i = 0; i < theSpecial.numberOfHits; i++)
                {
                    theEnemy.stats.currenthp -= damage;
                    theEnemy.stats.currenthp += healing;
                    Debug.Log("HEALED ALLY FOR " + healing + " DAMAGED ALLY FOR " + damage);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else if(theSpecial.targeting == 3)
            {
                for(int i = 0; i < theBattleManager.playerList.Count; i++)
                {
                    Character theEnemy = theBattleManager.playerList[i].GetComponent<Character>();
                    damageHealingCalculation(theSpecial, thePlayer, theEnemy);
                    for(int j = 0; j < theSpecial.numberOfHits; j++)
                    {
                        if(theEnemy.stats.currenthp >= 0)
                        {
                            theEnemy.stats.currenthp -= damage;
                            theEnemy.stats.currenthp += healing;
                            Debug.Log("HEALED ENEMY FOR " + healing + " DAMAGED ENEMY FOR " + damage);
                            yield return new WaitForSeconds(0.5f);
                        }
                    }
                }    
            }
            activated = false;
        }
    }
}
