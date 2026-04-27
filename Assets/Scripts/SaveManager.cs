using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class SaveManager : MonoBehaviour
{
    //Static so can be called throughout classes
    public static SaveManager instance;
    public List<GameObject> playerList;
    public GameData gameData;
    private List<SaveInterface> allSaveData;
    private string fileName = "reimu";
    private FileManager fileManager;
    //Should be initalized in inspector with every class wanted
    //These two should be synchronized with each other
    public GameObject[] classes;
    public GameObject[] portraits;
    void Awake()
    {
        playerList = new List<GameObject>();
        //First time generation
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            //Destroy duplicates
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        //Setting file save/load to default directory
        fileManager = new FileManager(Application.persistentDataPath, fileName);
        Debug.Log("Saved location is" + Application.persistentDataPath);
        allSaveData = findAllSaveData();
        loadGame();
        StartCoroutine(levelUp());
    }

    public void switchToWorldScene()
    {
        SceneManager.LoadScene("WorldScene");      
    }

    public void switchToBattleScene()
    {
        SceneManager.LoadScene("BattleScene");      
    }

    public void newGame()
    {
        if(gameData == null)
        {
            Debug.Log("Initializing");
            gameData = new GameData();
        }
    }

    public void loadGame()
    {
        gameData = fileManager.Load();
        //After loading, need to look at gamedata character type to instantiate correct prefab
        for(int i = 0; i < gameData.playerStats.Count; i++)
        {
            int characterType = gameData.playerStats[i].characterType;
            GameObject baseCharacter = classes[characterType];
            baseCharacter.GetComponent<Character>().stats = gameData.playerStats[i];
            playerList.Add(baseCharacter);
        }
        if(gameData == null)
        {
            Debug.Log("No save data found, initializing");
            newGame();
        }
    }

    public void saveGame()
    {
        gameData.playerStats.Clear();
        //Before saving, need to toss character data into gamedata
        for(int i = 0; i < playerList.Count; i++)
        {
            gameData.playerStats.Add(playerList[i].GetComponent<Character>().stats);
        }
        fileManager.Save(gameData);
    }

    void OnApplicationQuit()
    {
        saveGame();
    }

    private List<SaveInterface> findAllSaveData()
    {
        //Not a list yet
        IEnumerable<SaveInterface> allSaveData = FindObjectsOfType<MonoBehaviour>().OfType<SaveInterface>();
        return new List<SaveInterface>(allSaveData);
    }

    //Have to update character and stats here since in the playerlist, update functions do not run unless instantiated
    //AKA we cannot put them in character.cs
    public IEnumerator levelUp()
    {
        while(true)
        {
            for(int i = 0; i < playerList.Count; i++)
            {
                Character thePlayer = playerList[i].GetComponent<Character>();
                while(thePlayer.stats.exp >= thePlayer.expRequirement)
                {
                    thePlayer.stats.level++;
                    Debug.Log("LEVEL UP");
                    thePlayer.stats.exp -= thePlayer.expRequirement;
                    yield return new WaitForSeconds(0.5f);
                }
            }
            yield return null;
        }
    }

    void Update()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            Character thePlayer = playerList[i].GetComponent<Character>();
            if((thePlayer.stats.currenthp) > thePlayer.basemaxhp)
            {
                thePlayer.stats.currenthp = thePlayer.basemaxhp;
            }
            if((thePlayer.stats.currentmp) > thePlayer.basemaxmp)
            {
                thePlayer.stats.currentmp = thePlayer.basemaxmp;
            }
            if(thePlayer.stats.characterType == 0)
            {
                thePlayer.expRequirement = (float)Math.Ceiling(Math.Pow(thePlayer.stats.level, 1.55)) + 6;
                thePlayer.basemaxhp = (10 + 5 * thePlayer.stats.level);
                thePlayer.basemaxmp = (1 + 1 * thePlayer.stats.level);
                thePlayer.basephysical = (4 + 2 * thePlayer.stats.level);
                thePlayer.basemental = (0 + 0.5f * thePlayer.stats.level);
                thePlayer.basepdefense = (5 + 3.5f * thePlayer.stats.level);
                thePlayer.basemdefense = (3 + 3.5f * thePlayer.stats.level);
                thePlayer.specialList = new List<(int, int)>{(0, 1), (7, 5)};
            }
            else if(thePlayer.stats.characterType == 1)
            {
                thePlayer.expRequirement = (float)Math.Ceiling(Math.Pow(thePlayer.stats.level, 1.49)) + 4;
                thePlayer.basemaxhp = (7 + 2 * thePlayer.stats.level);
                thePlayer.basemaxmp = (3 + 1.5f * thePlayer.stats.level);
                thePlayer.basephysical = (5 + 3.5f * thePlayer.stats.level);
                thePlayer.basemental = (2 + 1 * thePlayer.stats.level);
                thePlayer.basepdefense = (2 + 1 * thePlayer.stats.level);
                thePlayer.basemdefense = (2 + 1 * thePlayer.stats.level);
                thePlayer.specialList = new List<(int, int)>{(2, 1), (5, 5)};

            }
            else if(thePlayer.stats.characterType == 2)
            {
                thePlayer.expRequirement = (float)Math.Ceiling(Math.Pow(thePlayer.stats.level, 1.47)) + 4;
                thePlayer.basemaxhp = (7 + 1.5f * thePlayer.stats.level);
                thePlayer.basemaxmp = (3 + 3f * thePlayer.stats.level);
                thePlayer.basephysical = (2 + 1f * thePlayer.stats.level);
                thePlayer.basemental = (3 + 3.5f * thePlayer.stats.level);
                thePlayer.basepdefense = (2 + 1 * thePlayer.stats.level);
                thePlayer.basemdefense = (3 + 2 * thePlayer.stats.level);
                thePlayer.specialList = new List<(int, int)>{(3, 1), (6, 5), (6, 5)};

            }
            else if(thePlayer.stats.characterType == 3)
            {
                thePlayer.expRequirement = (float)Math.Ceiling(Math.Pow(thePlayer.stats.level, 1.52)) + 5;
                thePlayer.basemaxhp = (10 + 3.5f * thePlayer.stats.level);
                thePlayer.basemaxmp = (3 + 2f * thePlayer.stats.level);
                thePlayer.basephysical = (3 + 1f * thePlayer.stats.level);
                thePlayer.basemental = (2 + 2f * thePlayer.stats.level);
                thePlayer.basepdefense = (3 + 2 * thePlayer.stats.level);
                thePlayer.basemdefense = (3 + 2 * thePlayer.stats.level);
                thePlayer.specialList = new List<(int, int)>{(1, 1), (4, 3), (8, 9)};
            }
        }
    }
}
