using UnityEngine;
[System.Serializable]
//Things that are REQUIRED to be saved are in the class, everything else should be in Character.cs
public class CharacterStats
{
    //0 = warrior, 1 = archer, 2 = mage, 3 = cleric
    public int characterType;
    public float currenthp;
    public float currentmp;
    public float exp;
    public float level;
    // public GameObject[] modifiers;
    // public Move[] moveset;
}
