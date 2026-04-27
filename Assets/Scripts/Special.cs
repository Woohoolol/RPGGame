using UnityEngine;
using System.Collections.Generic;
public class Special
{
    public string name;
    public string description;
    public int mpcost;
    //index 0 = Single target enemy, 1 = Area of effect enemy, 2 = ST ally, 3 = AOE ally, 4 = Self Target
    public int targeting;
    //Either damage or healing amount, do a decimal amount for percentage
    public float intensity;
    public int numberOfHits;
    public bool isPhysical;
    public bool isMental;
    public bool isHealing;
    public bool isModifier;
    //index 0 = Stat type, 1 = Intensity, 2 = Duration
    //Positive stat type = buff, Negative = debuff
    public List<float[]> modifierStats;
    //Stat type: 1 = physical, 2 = mental, 3 = pdefense, 4 = mdefense
    public Special()
    {
    }
    //Just turn on (true) whichever characteristics are true
    public Special(string name, string description, int mpcost, int targeting, float intensity = 0, int numberOfHits = 0, 
    bool isPhysical = false, bool isMental = false, 
    bool isHealing = false, bool isModifier = false, List<float[]> modifierStats = null)
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
        this.isModifier = isModifier;
        this.modifierStats = modifierStats;
        //Need to be of type {float, float, float}
        // this.buffStats = buffStats;
        // this.debuffStats = debuffStats;
    }

}
