using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Character : MonoBehaviour
{
    public CharacterStats stats = new CharacterStats();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float basemaxhp;
    public float basemaxmp;
    public float basephysical;
    public float basemental;
    public float basepdefense;
    public float basemdefense;

    public float finalmaxhp;
    public float finalmaxmp;
    public float finalphysical;
    public float finalmental;
    public float finalpdefense;
    public float finalmdefense;

    public float expRequirement;
    //First int is the skill
    //Second int is the skill level requirement
    public List<(int, int)> specialList;
    public List<float[]> modifiers;
    //Only relevant for enemy characters
    public float enemyExp = 5;
    public float enemyMoney = 10;

    void Start()
    {
        specialList = new List<(int, int)>();
        modifiers = new List<float[]>();
    }

    // Update is called once per frame
    void Update()
    {
        finalmaxhp = basemaxhp;
        finalmaxmp = basemaxmp;
        finalphysical = basephysical;
        finalmental = basemental;
        finalpdefense = basepdefense;
        finalmdefense = basemdefense;
        //Modifier calculations can happen in here since they're in battle only
        for(int i = 0; i < modifiers.Count; i++)
        {
            float modifierType = modifiers[i][0];
            float modifierValue = modifiers[i][1];
            if(modifierType < 0)
            {
                modifierValue *= -1;
            }
            if(Math.Abs(modifierType) == 1)
            {
                finalphysical += modifierValue * basephysical;
            }
            else if(Math.Abs(modifierType) == 2)
            {
                finalmental += modifierValue * basemental;
            }
            else if(Math.Abs(modifierType) == 3)
            {
                finalpdefense += modifierValue * basepdefense;
            }
            else if(Math.Abs(modifierType) == 4)
            {
                finalmdefense += modifierValue * basemdefense;
            }
            else if(Math.Abs(modifierType) == 5)
            {
                finalmaxhp += modifierValue * basemaxhp;
            }
            else if(Math.Abs(modifierType) == 6)
            {
                finalmaxmp += modifierValue * basemaxmp;
            }
        }
        //Stat caps
        //Cannot exceed 1.75x base value or fall below 0.25x base value
        finalmaxhp = statCap(finalmaxhp, basemaxhp);
        finalmaxmp = statCap(finalmaxmp, basemaxmp);
        finalphysical = statCap(finalphysical, basephysical);
        finalmental = statCap(finalmental, basemental);
        finalpdefense = statCap(finalpdefense, basepdefense);
        finalmdefense = statCap(finalmdefense, basemdefense);

    }

    public float statCap(float finalStat, float baseStat)
    {
        if(finalStat > 1.75f * baseStat)
        {
            return 1.75f * baseStat;
        }
        else if(finalStat < 0.25f * baseStat)
        {
            return 0.25f * baseStat;
        }
        else
        {
            return finalStat;
        }
    }
    public void modifierDecay()
    {
        for(int i = 0; i < modifiers.Count; i++)
        {
            float modifierDuration = modifiers[i][2];
            if(modifierDuration > 1)
            {
                modifiers[i][2]--;
            }
            else
            {
                modifiers.Remove(modifiers[i]);
                i--;
            }
        }
    }
}
