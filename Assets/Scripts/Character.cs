using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float hp;
    public float physical;
    public float mental;
    public float pdefense;
    public float mdefense;
    public GameObject[] modifiers;
    public float exp;
    public float money;
    // public Move[] moveset;
    void Start()
    {
        hp = 10;
        physical = 3;
        mental = 1;
        pdefense = 1;
        mdefense = 1;
        exp = 5;
        money = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
