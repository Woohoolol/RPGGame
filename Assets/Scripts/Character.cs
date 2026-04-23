using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Character : MonoBehaviour
{
    public CharacterStats stats = new CharacterStats();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float maxhp;
    public float maxmp;
    public float physical;
    public float mental;
    public float pdefense;
    public float mdefense;
    public float expRequirement;

    //Only relevant for enemy characters
    public float enemyExp = 5;
    public float enemyMoney = 10;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }


}
