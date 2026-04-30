using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
public class SpecialManager : MonoBehaviour
{
    public List<Special> allSpecials;
    public int specialIndex;
    public GameObject battleManager;
    public BattleHud battleHud;
    public int playerIndex;
    public int enemyIndex;
    public bool activated;
    float damage = 0;
    float healing = 0;
    //Constructor requirements: Name, Description, Mp cost, Targeting
    //Constructor options: Intensity, numberOfHits, isPhysical, isMagic, isHealing, isBuff, isDebuff
    //buffStats (when isBuff = true), debuffStats (when isDebuff = true)
    //Buff/Debuff array = {Stat type, intensity, duration}
    //Stat type: 1 = physical, 2 = mental, 3 = pdefense, 4 = mdefense
    //Negative versions of the stat type are considered debuffs
    void Awake()
    {
        allSpecials = new List<Special>();
        //Ally specials
        allSpecials.Add(new Special("Double Hit", "Hits an enemy twice for 0.75x physical.", 5, 0, intensity: 0.75f, numberOfHits: 2, isPhysical: true));
        allSpecials.Add(new Special("Respite", "Recovers an ally's hp for 0.75x mental.", 4, 2, intensity: 0.5f, numberOfHits: 1, isHealing: true, isMental: true));
        allSpecials.Add(new Special("Arrow Storm", "Strikes all enemies 3 times for 0.33x physical.", 12, 1, intensity: 0.33f, numberOfHits: 3, isPhysical: true));
        allSpecials.Add(new Special("Meteor", "Strikes an enemy powerfully for 2.5x mental.", 9, 0, intensity: 2.5f, numberOfHits: 1, isMental: true));
        allSpecials.Add(new Special("Circle of Life", "Recover all allies' hp for 0.33x mental.", 12, 3, intensity: 0.33f, numberOfHits: 1, isMental: true, isHealing: true));
        allSpecials.Add(new Special("Sharpen", "Increase own physical by 0.5x for 3 turns.", 5, 4, isModifier: true, modifierStats: new List<float[]>{new float[]{1, 0.5f, 1}}));
        allSpecials.Add(new Special("Wither", "Decrease an enemy's physical defense and mental defense by 0.5x for 2 turns.", 9, 0, isModifier: true, modifierStats: new List<float[]>(){new float[]{-3, 0.5f, 2}, new float[]{-4, 0.5f, 2}}));
        allSpecials.Add(new Special("Guard", "Increase own physical defense by 0.75x for 2 turns.", 4, 4, isModifier: true, modifierStats: new List<float[]>(){new float[]{3, 0.75f, 2}}));
        allSpecials.Add(new Special("Mana Barrier", "Increase all allies' magic defense by 0.33x for 3 turns.", 10, 3, isModifier: true, modifierStats: new List<float[]>(){new float[]{4, 0.33f, 3}}));

        //Enemy specials

    }
    void Start()
    {
        StartCoroutine(useSpecial());
    }
    
    void Update()
    {

    }

    public void activateSpecial(int specialIndex, int playerIndex = 0, int enemyIndex = 0)
    {
        activated = true;
        this.specialIndex = specialIndex;
        this.playerIndex = playerIndex;
        this.enemyIndex = enemyIndex;
    }

    public void damageHealingCalculation(Special theSpecial, Character thePlayer, Character theEnemy)
    {
        damage = 0;
        healing = 0;
        if(theSpecial.isPhysical)
        {
            if(theSpecial.isHealing)
            {
                healing = thePlayer.finalphysical * theSpecial.intensity;
            }
            else
            {
                damage = (float)Math.Ceiling((double)(thePlayer.finalphysical * theSpecial.intensity * thePlayer.finalphysical/(1 + 0.75 * thePlayer.finalphysical + Math.Pow(theEnemy.finalpdefense, 0.75))));
            }
        }
        else if(theSpecial.isMental)
        {
            if(theSpecial.isHealing)
            {
                healing = thePlayer.finalmental * theSpecial.intensity;
            }
            else
            {
                damage = (float)Math.Ceiling((double)(thePlayer.finalmental * theSpecial.intensity * thePlayer.finalmental/(1 + 0.75 * thePlayer.finalmental + Math.Pow(theEnemy.finalmdefense, 0.75))));
            }
        }
    }

    public IEnumerator useSpecial()
    {
        while(true)
        {
            yield return new WaitUntil(() => activated);
            Special theSpecial = allSpecials[specialIndex];
            BattleManager theBattleManager = battleManager.GetComponent<BattleManager>();
            //Using playerindex to lock in who is using the spell since it will change index right away in battle manager
            Character thePlayer = theBattleManager.playerList[playerIndex].GetComponent<Character>();
            if(theSpecial.targeting == 0)
            {
                Character theEnemy = theBattleManager.enemyList[enemyIndex].GetComponent<Character>();
                damageHealingCalculation(theSpecial, thePlayer, theEnemy);
                for(int i = 0; i < theSpecial.numberOfHits; i++)
                {
                    if(theEnemy != null)
                    {
                        if(theSpecial.isHealing)
                        {
                            battleHud.spawnParticle(2, theBattleManager.enemyList[enemyIndex]);
                        }
                        else if(theSpecial.isPhysical)
                        {
                            battleHud.spawnParticle(0, theBattleManager.enemyList[enemyIndex]);
                        }
                        else if(theSpecial.isMental)
                        {
                            battleHud.spawnParticle(1, theBattleManager.enemyList[enemyIndex]);
                        }
                        theEnemy.stats.currenthp -= damage;
                        theEnemy.stats.currenthp += healing;
                        Debug.Log("HEALED ENEMY FOR " + healing + " DAMAGED ENEMY FOR " + damage);
                        yield return new WaitForSeconds(0.25f);
                    }
                }
                if(theSpecial.isModifier && theEnemy != null)
                {
                    for(int i = 0; i < theSpecial.modifierStats.Count; i++)
                    {
                        theEnemy.modifiers.Add(theSpecial.modifierStats[i]);
                        if(theSpecial.modifierStats[i][0] < 0)
                        {
                            battleHud.spawnParticle(3, theBattleManager.enemyList[enemyIndex]);
                        }
                        else if(theSpecial.modifierStats[i][0] > 0)
                        {
                            battleHud.spawnParticle(4, theBattleManager.enemyList[enemyIndex]);
                        }
                        yield return new WaitForSeconds(0.25f);
                    }
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
                        if(theEnemy != null)
                        {
                            if(theSpecial.isHealing)
                            {
                                battleHud.spawnParticle(2, theBattleManager.enemyList[i]);
                            }
                            else if(theSpecial.isPhysical)
                            {
                                battleHud.spawnParticle(0, theBattleManager.enemyList[i]);
                            }
                            else if(theSpecial.isMental)
                            {
                                battleHud.spawnParticle(1, theBattleManager.enemyList[i]);
                            }
                            theEnemy.stats.currenthp -= damage;
                            theEnemy.stats.currenthp += healing;
                            Debug.Log("HEALED ENEMY FOR " + healing + " DAMAGED ENEMY FOR " + damage);
                            yield return new WaitForSeconds(0.25f);
                        }
                    }
                    if(theSpecial.isModifier && theEnemy != null)
                    {
                        for(int j = 0; j < theSpecial.modifierStats.Count; j++)
                        {
                            theEnemy.modifiers.Add(theSpecial.modifierStats[j]);
                            if(theSpecial.modifierStats[j][0] < 0)
                            {
                                battleHud.spawnParticle(3, theBattleManager.enemyList[i]);
                            }
                            else if(theSpecial.modifierStats[j][0] > 0)
                            {
                                battleHud.spawnParticle(4, theBattleManager.enemyList[i]);
                            }
                            yield return new WaitForSeconds(0.25f);
                        }
                    }
                    //Killed enemy, should not shift
                    if(theEnemy == null)
                    {
                        i--;
                    }
                }
            }
            else if(theSpecial.targeting == 2)
            {
                Character theEnemy = theBattleManager.playerList[enemyIndex].GetComponent<Character>();
                damageHealingCalculation(theSpecial, thePlayer, theEnemy);
                for(int i = 0; i < theSpecial.numberOfHits; i++)
                {
                    if(theEnemy != null)
                    {
                        if(theSpecial.isHealing)
                        {
                            battleHud.spawnParticle(2, theBattleManager.playerList[enemyIndex]);
                        }
                        else if(theSpecial.isPhysical)
                        {
                            battleHud.spawnParticle(0, theBattleManager.playerList[enemyIndex]);
                        }
                        else if(theSpecial.isMental)
                        {
                            battleHud.spawnParticle(1, theBattleManager.playerList[enemyIndex]);
                        }
                        theEnemy.stats.currenthp -= damage;
                        theEnemy.stats.currenthp += healing;
                        Debug.Log("HEALED ALLY FOR " + healing + " DAMAGED ALLY FOR " + damage);
                        yield return new WaitForSeconds(0.25f);
                    }
                }
                if(theSpecial.isModifier && theEnemy != null)
                {
                    for(int i = 0; i < theSpecial.modifierStats.Count; i++)
                    {
                        theEnemy.modifiers.Add(theSpecial.modifierStats[i]);
                        if(theSpecial.modifierStats[i][0] < 0)
                        {
                            battleHud.spawnParticle(3, theBattleManager.playerList[enemyIndex]);
                        }
                        else if(theSpecial.modifierStats[i][0] > 0)
                        {
                            battleHud.spawnParticle(4, theBattleManager.playerList[enemyIndex]);
                        }
                        yield return new WaitForSeconds(0.25f);
                    }
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
                        if(theEnemy != null)
                        {
                            if(theSpecial.isHealing)
                            {
                                battleHud.spawnParticle(2, theBattleManager.playerList[i]);
                            }
                            else if(theSpecial.isPhysical)
                            {
                                battleHud.spawnParticle(0, theBattleManager.playerList[i]);
                            }
                            else if(theSpecial.isMental)
                            {
                                battleHud.spawnParticle(1, theBattleManager.playerList[i]);
                            }
                            theEnemy.stats.currenthp -= damage;
                            theEnemy.stats.currenthp += healing;
                            Debug.Log("HEALED ALLY FOR " + healing + " DAMAGED ALLY FOR " + damage);
                            yield return new WaitForSeconds(0.25f);
                        }
                    }
                    if(theSpecial.isModifier && theEnemy != null)
                    {
                        for(int j = 0; j < theSpecial.modifierStats.Count; j++)
                        {
                            theEnemy.modifiers.Add(theSpecial.modifierStats[j]);
                            if(theSpecial.modifierStats[j][0] < 0)
                            {
                                battleHud.spawnParticle(3, theBattleManager.playerList[i]);
                            }
                            else if(theSpecial.modifierStats[j][0] > 0)
                            {
                                battleHud.spawnParticle(4, theBattleManager.playerList[i]);
                            }
                            yield return new WaitForSeconds(0.25f);
                        }
                    }
                    //Killed enemy, should not shift
                    if(theEnemy == null)
                    {
                        i--;
                    }
                }    
            }
            else if(theSpecial.targeting == 4)
            {
                damageHealingCalculation(theSpecial, thePlayer, thePlayer);
                for(int j = 0; j < theSpecial.numberOfHits; j++)
                {
                    if(thePlayer != null)
                    {
                        if(theSpecial.isHealing)
                        {
                            battleHud.spawnParticle(2, theBattleManager.playerList[playerIndex]);
                        }
                        else if(theSpecial.isPhysical)
                        {
                            battleHud.spawnParticle(0, theBattleManager.playerList[playerIndex]);
                        }
                        else if(theSpecial.isMental)
                        {
                            battleHud.spawnParticle(1, theBattleManager.playerList[playerIndex]);
                        }
                        thePlayer.stats.currenthp -= damage;
                        thePlayer.stats.currenthp += healing;
                        Debug.Log("HEALED ENEMY FOR " + healing + " DAMAGED ENEMY FOR " + damage);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                if(theSpecial.isModifier && thePlayer != null)
                {
                    for(int i = 0; i < theSpecial.modifierStats.Count; i++)
                    {
                        thePlayer.modifiers.Add(theSpecial.modifierStats[i]);
                        if(theSpecial.modifierStats[i][0] < 0)
                        {
                            battleHud.spawnParticle(3, theBattleManager.playerList[playerIndex]);
                        }
                        else if(theSpecial.modifierStats[i][0] > 0)
                        {
                            battleHud.spawnParticle(4, theBattleManager.playerList[playerIndex]);
                        }
                        yield return new WaitForSeconds(0.25f);
                    }
                }
            }
            activated = false;
        }
    }
}
