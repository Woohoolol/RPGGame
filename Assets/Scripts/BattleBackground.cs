using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleBackground : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<Sprite> backgrounds;
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = backgrounds[SaveManager.instance.biome - 1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
