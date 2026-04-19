using UnityEngine;
using System.Collections;
public class TurnBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int turnCounter;
    public GameObject[] players;
    public GameObject[] enemies;
    void Start()
    {
        turnCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(turnCounter % 2 == 0)
        {
            for(int i = 0; i < players.Length; i++)
            {
                StartCoroutine(playerTurn(players[i]));
            }
        }
        else
        {
            for(int i = 0; i < enemies.Length; i++)
            {
                StartCoroutine(enemyTurn(enemies[i]));
            }
        }
        StartCoroutine(turnAdvance());
    }

    public IEnumerator playerTurn(GameObject player)
    {
        yield return 0;
    }

    public IEnumerator enemyTurn(GameObject enemy)
    {
        yield return 0;
    }

    public IEnumerator turnAdvance()
    {
        turnCounter++;
        yield return 0;

    }
}
