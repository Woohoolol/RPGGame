using UnityEngine;
[System.Serializable]

public class CharacterStats
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float exp;
    public float level;
    public float hp;
    public float mp;
    public float physical;
    public float mental;
    public float pdefense;
    public float mdefense;
    // public GameObject[] modifiers;
    //Only relevant for enemy characters
    public float enemyExp = 5;
    public float enemyMoney = 10;
    // public Move[] moveset;
}
