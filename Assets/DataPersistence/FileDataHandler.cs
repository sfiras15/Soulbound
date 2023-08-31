using UnityEngine;
using System;
using System.IO;


// Handle saving /loading game data from the files and handle the path/directory
public class FileDataHandler
{

    private string dataDirPath = "";
    private string dataFileName = "";


    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        // use Path.Combine to account for diffent Os's having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                //Load the serialized data from the file
                string dataToload = "";

                using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToload = reader.ReadToEnd();
                    }
                }

                //Deserialize the data from Json back into the c# object

                loadedData = JsonUtility.FromJson<GameData>(dataToload);

            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file : " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        // use Path.Combine to account for diffent Os's having different path separators
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        try
        {
            // Create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize the C# game data object into Json
            string dataToStore = JsonUtility.ToJson(data,true); // true parameter to format the data inside the file

            //Write the serialized data to the file

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch(Exception e)
        {
            Debug.LogError("Error Occured when trying to save data to file : " + fullPath + "\n" + e);
        }
    }
}
