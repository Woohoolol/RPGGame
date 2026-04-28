using UnityEngine;
using System;
using System.IO;
public class FileManager
{
    private string path = "";
    public string fileName = "";

    public FileManager(string path, string fileName)
    {
        this.path = path;
        this.fileName = fileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(path, fileName);
        GameData loadedData = null;
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    //Reads cuz we're loading
                    using(StreamReader reader = new StreamReader(stream))
                    {
                       //Read the file!
                       dataToLoad = reader.ReadToEnd();
                    }
                }
                Debug.Log("data is" + dataToLoad);
                //Converting from json back to gamedata
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error could not read data" + e.ToString());
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        //Accounts for different OS shenanigans
        string fullPath = Path.Combine(path, fileName);
        try
        {
            //If directory doesn't exist to save in, create it
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //Serializing gamedata, the true makes it readable
            string dataStored = JsonUtility.ToJson(data, true);
            //Using makes sure it open and closes without leaks etc etc
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                //Writes cuz we're saving
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    //Write it to the file!
                    writer.Write(dataStored);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error could not save data" + e.ToString());
        }
    }
}
