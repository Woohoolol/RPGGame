using UnityEngine;

public class Special
{
    public string name;
    public string description;
    //Either damage or healing amount
    public float intensity;
    //index 0 = Single target enemy, 1 = Area of effect enemy, 2 = ST ally, 3 = AOE ally
    public int targeting;
    public bool isPhysical;
    public bool isMagic;
    public bool isHealing;
    public int numberOfHits;
    public bool isBuff;
    //index 0 = Stat type, 1 = Intensity, 2 = Duration
    public float[] buffStats;
    public bool isDebuff;
    //index 0 = Stat type, 1 = Intensity, 2 = Duration
    public float[] debuffStats;
    //Stat type: 0 = attack, 1 = mental, 2 = pdefense, 3 = mdefense, 4 = 
    public Special()
    {
        buffStats = new float[3];
        debuffStats = new float[3];
    }

}
