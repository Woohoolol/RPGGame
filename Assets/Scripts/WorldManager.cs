using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class WorldManager : MonoBehaviour
{
    public GameObject player;
    public GameObject background;
    public float encounterRate;
    private float encounterModifier;
    private float encounterRequirement;
    public GameObject[] actions;
    public GameObject actionMenu;
    public GameObject playerStats;
    //0 = Overworld mode, 1 = stats menu, 2 = detailed stats, 3 = equipment menu, 4 = inventory menu
    public int mode;
    public int focusedIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        encounterModifier = 1;
        encounterRequirement = Random.Range(1, 2);
        Instantiate(background, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        Instantiate(background, new Vector3(-19.20f, 0, 0), Quaternion.Euler(0, 0, 0));
        Instantiate(background, new Vector3(19.20f, 0, 0), Quaternion.Euler(0, 0, 0));
        mode = 0;
        focusedIndex = 0;
        StartCoroutine(showMenu());
    }

    // Update is called once per frame
    void Update()
    {

        if(mode == 0)
        {
            actionMenu.SetActive(false);
            player.GetComponent<PlayerMovement>().canMove = true;
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                mode = 1;
            }
            if(player.GetComponent<PlayerMovement>().xDirection != 0 || player.GetComponent<PlayerMovement>().yDirection != 0)
            {
                encounterRate += Time.deltaTime * encounterModifier;
            }
            if(encounterRate >= encounterRequirement)
            {
                encounterRate = 0;
                encounterRequirement = Random.Range(10, 20);
                //SaveManager.instance is the static thing we can call
                SaveManager.instance.gameData.battleSpawn = 1;
                SaveManager.instance.switchToBattleScene();
            }
        }
        else if(mode == 1)
        {
            actionMenu.SetActive(true);
            player.GetComponent<PlayerMovement>().canMove = false;
            if(Keyboard.current.xKey.wasPressedThisFrame)
            {
                mode = 0;
            }
           if(Keyboard.current.upArrowKey.wasPressedThisFrame && focusedIndex > 0)
            {
                //Unhighlight option before moving
                actions[focusedIndex].GetComponent<SpriteRenderer>().color = new Color(0.3f, 0, 1);
                focusedIndex--;
            }
            if(Keyboard.current.downArrowKey.wasPressedThisFrame && focusedIndex < actions.Length - 1)
            {
                actions[focusedIndex].GetComponent<SpriteRenderer>().color = new Color(0.3f, 0, 1);
                focusedIndex++;
            }
            //Highlight current option
            actions[focusedIndex].GetComponent<SpriteRenderer>().color = Color.blue;
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if(focusedIndex == 0)
                {
                    Debug.Log("DETAILED CHARACTER STATS");
                    mode = 2;
                }
                else if(focusedIndex == 1)
                {
                    Debug.Log("EQUIPMENT MENU");
                }
                else if(focusedIndex == 2)
                {
                    Debug.Log("INVENTORY MENU");
                }
                else if(focusedIndex == 3)
                {
                    Application.Quit();
                }
            }

            // for(int i = 0; i < SaveManager.instance.playerList.Count; i++)
            // {
            //     string playerInfo = "hp: " + SaveManager.instance.playerList[i].GetComponent<Character>().currenthp + " mp: " + battleManager.playerList[i].GetComponent<Character>().currentmp;
            //     playerStats.transform.GetChild(i).gameObject.GetComponent<TextMeshProUGUI>().SetText(playerInfo);
            // }
        }

    }
    public IEnumerator showMenu()
    {
        while(true)
        {
            if(mode == 1)
            {
                List<GameObject> portraits = new List<GameObject>();
                for(int i = 0; i < SaveManager.instance.playerList.Count; i++)
                {
                    playerStats.transform.GetChild(i).gameObject.SetActive(true);
                    GameObject portrait = Instantiate(SaveManager.instance.portraits[SaveManager.instance.playerList[i].GetComponent<Character>().stats.characterType], 
                    playerStats.transform.GetChild(i).position + new Vector3(-6.5f, 0f, -1), Quaternion.Euler(0, 0, 0));
                    portraits.Add(portrait);
                    portrait.transform.localScale = new Vector3(0.08f, 0.08f, 1);
                    portrait.transform.parent = actionMenu.transform;
                }
                yield return new WaitUntil(() => mode != 1);
                for(int i = 0; i < portraits.Count; i++)
                {
                    Destroy(portraits[i]);
                }
            }
            yield return null;
        }
    }
}
