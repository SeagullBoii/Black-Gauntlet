using UnityEngine;
using System;
using System.IO;
using UnityEngine.Profiling;

public class FileDataHandler
{
    private string dataFileName = "";

    public FileDataHandler(string dataFileName)
    {
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            loadedData = JsonUtility.FromJson<GameData>(File.ReadAllText(fullPath));
        }
        return loadedData;
    }

    public void Save(GameData data)
    {

        string fullPath = Path.Combine(Application.persistentDataPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                writer.Write(dataToStore);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file:" + fullPath + "\n" + e);
        }
    }

}
