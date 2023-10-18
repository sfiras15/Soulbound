using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;



// Class for storing all the gameData when saving/loading
[System.Serializable]
public class GameData
{
    //From PlayerManager
    public int playerLevel;
    public int playerCurrentXp;
    public int nextLevelXp;

    //From InputPlayer
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public int currentHealth;
    public int maxHealth;
    public int currentStamina;
    public int maxStamina;

    //From Enemy

    public SerializableDictionary<int, EnemyData> enemyDictionary;

    //From inventory
    public SerializableDictionary<int, SerializableItem> inventoryDictionary;

    //From itemPickup

    public SerializableDictionary<int, ItemData> itemDictionary;
    public SerializableDictionary<int, ChestData> chestDictionary;

    public SerializableWeapon_SO currentWeapon;
    //TO DO ,enemySpawner Position,...


    // The values defined in this constructor will be the default values
    // the game starts with when there's no data to load !!
    public GameData()
    {

        playerLevel = 1;
        playerCurrentXp = 0;
        nextLevelXp = 100;
        PlayerData playerData = new PlayerData();
        if (DataInitializerManager.instance.GetPlayerInTheWorld != null)
        {
            playerData = DataInitializerManager.instance.GetPlayerInTheWorld.playerInfo();
        }
        playerPosition = playerData.playerPosition;
        playerRotation = playerData.playerRotation;

        currentHealth = playerData.currentHealth;
        maxHealth = playerData.maxHealth;
        currentStamina = playerData.currentStamina;
        maxStamina = playerData.currentStamina;

        inventoryDictionary = new SerializableDictionary<int, SerializableItem>();

        currentWeapon = null;

        enemyDictionary = new SerializableDictionary<int, EnemyData>();


        //Placeholder until I implement enemySpawner from dead enemies
        List<Enemy> enemies = new List<Enemy>();
        if (DataInitializerManager.instance.GetEnemiesInTheWorld != null)
        {
            enemies = DataInitializerManager.instance.GetEnemiesInTheWorld;
        }
        foreach (var enemy in enemies)
        {
            enemyDictionary.Add(enemy.GetEnemyID, enemy.EnemyInfo());
        }



        itemDictionary = new SerializableDictionary<int, ItemData>();

        List<ItemPickup> items = new List<ItemPickup>();

        if (DataInitializerManager.instance.GetEnemiesInTheWorld != null)
        {
            items = DataInitializerManager.instance.GetItemInTheWorld;
        }

        foreach (var item in items)
        {
            itemDictionary.Add(item.GetItemId, item.ItemInfo());
        }

        chestDictionary = new SerializableDictionary<int, ChestData>();
        List<Chest> chests = new List<Chest>();
        if (DataInitializerManager.instance.GetChestsInTheWorld != null)
        {
            chests = DataInitializerManager.instance.GetChestsInTheWorld;
        }
        foreach(var chest in chests )
        {
            chestDictionary.Add(chest.GetChestID, chest.ChestInfo());
        }



    }   
}
