using UnityEngine;

public class Special
{
    public string name;
    public string description;
    public int mpcost;
    //index 0 = Single target enemy, 1 = Area of effect enemy, 2 = ST ally, 3 = AOE ally
    public int targeting;
    //Either damage or healing amount, do a decimal amount for percentage
    public float intensity;
    public int numberOfHits;
    public bool isPhysical;
    public bool isMental;
    public bool isHealing;
    public bool isBuff;
    public bool isDebuff;
    //index 0 = Stat type, 1 = Intensity, 2 = Duration
    public float[] buffStats;
    //index 0 = Stat type, 1 = Intensity, 2 = Duration
    public float[] debuffStats;
    //Stat type: 0 = attack, 1 = mental, 2 = pdefense, 3 = mdefense
    public Special()
    {
        buffStats = new float[3];
        debuffStats = new float[3];
    }
    //Just turn on (true) whichever characteristics are true
    public Special(string name, string description, int mpcost, int targeting, float intensity = 0, int numberOfHits = 0, 
    bool isPhysical = false, bool isMental = false, 
    bool isHealing = false, bool isBuff = false, 
    bool isDebuff = false)
    {
        this.name = name;
        this.description = description;
        this.mpcost = mpcost;
        this.targeting = targeting;
        this.intensity = intensity;
        this.numberOfHits = numberOfHits;
        this.isPhysical = isPhysical;
        this.isMental = isMental;
        this.isHealing = isHealing;
        this.isBuff = isBuff;
        this.isDebuff = isDebuff;
        //Need to be of type {float, float, float}
        // this.buffStats = buffStats;
        // this.debuffStats = debuffStats;
    }

}
