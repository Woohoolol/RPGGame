using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
public class SaveManager : MonoBehaviour
{
    //Static so can be called throughout classes
    public static SaveManager instance;
    public int battleSpawn;
    private GameData gameData;
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

        foreach(SaveInterface aSaveData in allSaveData)
        {
            //Talking to each file and calling load data
            aSaveData.loadData(gameData);
        }
        Debug.Log("Loaded exp is " + gameData.exp);
    }

    public void saveGame()
    {
        foreach(SaveInterface aSaveData in allSaveData)
        {
            //Talking to each file and calling save data
            aSaveData.saveData(ref gameData);
        }
        Debug.Log("Saved exp is " + gameData.exp + ", saving");
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
