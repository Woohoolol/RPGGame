using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    //Static so can be called throughout classes
    public static SaveManager instance;
    public List<GameObject> playerList;
    public GameData gameData;
    private List<SaveInterface> allSaveData;
    private string fileName = "reimu";
    private FileManager fileManager;
    void Awake()
    {
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
}
