using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// Saving happens during closing down the game , loading happens at the start of the game , change those later 
public class DataPersistenceManger : MonoBehaviour
{
    [Header("File Storage Config")]

    [SerializeField] private string fileName;

    private GameData gameData;

    private FileDataHandler dataHandler;
    public static DataPersistenceManger Instance { get; private set; }


    //Lists of classes using IdataPersistance interface
    private List<IDataPersistence> dataPersistenceObjects;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Data Persistance Manager in the scene. ");
        }
        Instance = this;
    }

    private void Start()
    {
        //Application.persistentDataPath gives operating system standard directory for persisting data in a unity project , we can change the directory if we need
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {

        //Load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(); // if no file exists the load will return null 

        // if no data can be loaded initialize a new game
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults. ");
            NewGame();
        }
        // Push The loaded data to all other scripts that need it

        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        //Debug.Log("Loaded data : level = " + gameData.playerLevel + " currentXp = " + gameData.playerCurrentXp + " xpForNextLevel =  " + gameData.nextLevelXp);

    }
    public void SaveGame()
    {

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        //Debug.Log("Saved data : level = " + gameData.playerLevel + " currentXp = " + gameData.playerCurrentXp + " xpForNextLevel =  " + gameData.nextLevelXp);

        // save that data to a file using the data handler

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // find all objects that uses the IdataPersistance interface
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
