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
    public GameObject transitionEffectPrefab;
    private GameObject transitionEffect;
    public GameObject dialogueCanvas;
    public int numberOfEnemies;
    //biome 0 = home, 1 = sky, 2 = space
    public int biome;
    public List<GameObject> playerList;
    public Dictionary<int, int> inventory;
    public Vector3 lastSavedLocation;
    public int checkpoint;
    public float money;
    public GameData gameData;
    private List<SaveInterface> allSaveData;
    private string fileName = "reimu";
    private FileManager fileManager;
    public List<Enemies> enemies;
    public int blah;
    private String theSceneName;
    public bool dialogueActive;
    public List<GameObject> dialogueList;
    //Should be initalized in inspector with every class wanted
    //These two should be synchronized with each other
    public GameObject[] classes;
    public GameObject[] portraits;
    void Awake()
    {
        biome = 1;
        theSceneName = SceneManager.GetActiveScene().name;
        transitionEffect = Instantiate(transitionEffectPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        playerList = new List<GameObject>();
        dialogueList = new List<GameObject>();
        inventory = new Dictionary<int, int>(); 
        //Setting file save/load to default directory
        fileManager = new FileManager(Application.persistentDataPath, fileName);
        Debug.Log("Saved location is" + Application.persistentDataPath);
        allSaveData = findAllSaveData();
        StartCoroutine(levelUp());
        StartCoroutine(processDialogue());
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

    public void switchScene(String sceneName)
    {
        StartCoroutine(switchToScene(sceneName));
    }

    public IEnumerator switchToScene(String sceneName)
    {
        transitionEffect.GetComponent<Animator>().Play("StartTransition");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName); 
        yield return new WaitForSeconds(0.5f);
        // GameObject copyTransitionEffect = transitionEffect;
        // Destroy(copyTransitionEffect);
    }

    public void newGame()
    {
        Debug.Log("Initializing");
        gameData = new GameData();
        for(int i = 0; i < gameData.playerStats.Count; i++)
        {
            int characterType = gameData.playerStats[i].characterType;
            GameObject baseCharacter = classes[characterType];
            baseCharacter.GetComponent<Character>().stats = gameData.playerStats[i];
            playerList.Add(baseCharacter);
        }
        for(int i = 0; i < gameData.inventoryID.Count; i++)
        {
            inventory.Add(gameData.inventoryID[i], gameData.inventoryQuantity[i]);
        }
        lastSavedLocation = gameData.lastSavedLocation;
        checkpoint = gameData.checkpoint;
        money = gameData.money;
    }

    public void exit()
    {
        Application.Quit();
    }
    public void loadGame()
    {
        playerList = new List<GameObject>();
        dialogueList = new List<GameObject>();
        inventory = new Dictionary<int, int>(); 
        gameData = fileManager.Load();
        //After loading, need to look at gamedata character type to instantiate correct prefab
        if(gameData == null)
        {
            Debug.Log("No save data found, initializing");
            newGame();
        }
        for(int i = 0; i < gameData.playerStats.Count; i++)
        {
            int characterType = gameData.playerStats[i].characterType;
            GameObject baseCharacter = classes[characterType];
            baseCharacter.GetComponent<Character>().stats = gameData.playerStats[i];
            playerList.Add(baseCharacter);
        }
        for(int i = 0; i < gameData.inventoryID.Count; i++)
        {
            inventory.Add(gameData.inventoryID[i], gameData.inventoryQuantity[i]);
        }
        lastSavedLocation = gameData.lastSavedLocation;
        checkpoint = gameData.checkpoint;
        money = gameData.money;
    }

    public void saveGame()
    {
        gameData.playerStats.Clear();
        gameData.inventoryID.Clear();
        gameData.inventoryQuantity.Clear();
        //Before saving, need to toss character data into gamedata
        for(int i = 0; i < playerList.Count; i++)
        {
            gameData.playerStats.Add(playerList[i].GetComponent<Character>().stats);
        }
        foreach(var item in inventory)
        {       
            gameData.inventoryID.Add(item.Key);
            gameData.inventoryQuantity.Add(item.Value);
        }
        gameData.lastSavedLocation = lastSavedLocation;
        gameData.checkpoint = checkpoint;
        gameData.money = money;
        fileManager.Save(gameData);
    }

    // void OnApplicationQuit()
    // {
    //     saveGame();
    // }

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
            if(SceneManager.GetActiveScene().name != "MainMenuScene")
            {
                for(int i = 0; i < playerList.Count; i++)
                {
                    Character thePlayer = playerList[i].GetComponent<Character>();
                    bool leveledUp = false;
                    while(thePlayer.stats.exp >= thePlayer.expRequirement)
                    {
                        thePlayer.stats.level++;
                        thePlayer.stats.exp -= thePlayer.expRequirement;
                        leveledUp = true;
                    }
                    if(leveledUp)
                    {
                        List<string> levelUpDialogue = new List<string>{thePlayer.stats.characterType + " Leveled up to level " + thePlayer.stats.level + "!"};
                        SaveManager.instance.spawnDialogue(levelUpDialogue, characterDialogue: false);
                    }
                }
            }
            yield return null;
        }
    }

    //Dialogue character referencing charactertypes from Character
    public GameObject spawnDialogue(List<string> dialogue, bool characterDialogue = false, List<int> dialogueCharacters = null)
    {
        //Z values are all glitching with each other, will spawn at dfferent z values
        float zCoordinate = 0;
        if(dialogueList.Count > 1)
        {
            zCoordinate = dialogueList[dialogueList.Count-1].transform.position.z - 2;
        }
        GameObject theDialogueCanvas = Instantiate(dialogueCanvas, new Vector3(0, 0, zCoordinate), Quaternion.Euler(0, 0, 0));
        theDialogueCanvas.SetActive(false);
        theDialogueCanvas.transform.GetChild(0).GetComponent<Dialogue>().texts = dialogue;
        theDialogueCanvas.transform.GetChild(0).GetComponent<Dialogue>().characterType = dialogueCharacters;
        theDialogueCanvas.transform.GetChild(0).GetComponent<Dialogue>().characterDialogue = characterDialogue;
        dialogueList.Add(theDialogueCanvas);
        dialogueActive = true;
        return theDialogueCanvas;
    }

    public IEnumerator processDialogue()
    {
        while(true)
        {  
            if(dialogueList.Count > 0)
            {
                GameObject theDialogueCanvas = dialogueList[0];
                dialogueList.RemoveAt(0);
                //Dialogue box may disappear in the middle of scene transitions
                if(theDialogueCanvas != null)
                {
                    theDialogueCanvas.SetActive(true);
                }
                yield return new WaitUntil(() => theDialogueCanvas == null);
            }
            yield return null;
        }
    }
    void Update()
    {
        if(theSceneName != SceneManager.GetActiveScene().name)
        {
            Debug.Log("CHANGED SCENE");
            theSceneName = SceneManager.GetActiveScene().name;
            transitionEffect = Instantiate(transitionEffectPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        }
        for(int i = 0; i < playerList.Count; i++)
        {
            Character thePlayer = playerList[i].GetComponent<Character>();
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
                thePlayer.specialList = new List<(int, int)>{(3, 1), (6, 5)};

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
            if((thePlayer.stats.currenthp) > thePlayer.basemaxhp)
            {
                thePlayer.stats.currenthp = thePlayer.basemaxhp;
            }
            if((thePlayer.stats.currentmp) > thePlayer.basemaxmp)
            {
                thePlayer.stats.currentmp = thePlayer.basemaxmp;
            }
            if((thePlayer.stats.currenthp) < 0)
            {
                thePlayer.stats.currenthp = 0;
            }
            if((thePlayer.stats.currentmp) < 0)
            {
                thePlayer.stats.currentmp = 0;
            }
        }

    }
}
