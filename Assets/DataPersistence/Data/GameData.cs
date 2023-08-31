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

    //TO DO add currentWeapon ,enemySpawner Position,...


    // The values defined in this constructor will be the default values
    // the game starts with when there's no data to load !!
    public GameData()
    {
        playerLevel = 1;
        playerCurrentXp = 0;
        nextLevelXp = 100;

        playerPosition = new Vector3(-8.64999962f, 0, -10.1409092f);
        playerRotation = new Vector3(359.24f, 330.78f, 0.42f);

        currentHealth = 100;
        maxHealth = 100;
        currentStamina = 100;
        maxStamina = 100;



        enemyDictionary = new SerializableDictionary<int, EnemyData>();


        //Placeholder until I implement enemySpawner from dead enemies
        EnemyData enemy1 = new EnemyData();
        enemy1.enemyPosition = new Vector3(-6.59000015f, -1.29999995f, -3.3599999f);
        enemy1.enemyRotation = new Vector3(0, 0, 0);
        enemy1.killed = false;

        EnemyData enemy2 = new EnemyData();
        enemy2.enemyPosition = new Vector3(-6.11000013f, -1.21200001f, -3.97000003f);
        enemy2.enemyRotation = new Vector3(0, 209.439987f, 0);
        enemy2.killed = false;

        enemyDictionary.Add(1, enemy1);
        enemyDictionary.Add(2, enemy2);

        inventoryDictionary = new SerializableDictionary<int, SerializableItem>();

        itemDictionary = new SerializableDictionary<int, ItemData>();


        //Placeholder until i implement itemDrop from dead enemies
        ItemData item1 = new ItemData();
        item1.itemPosition = new Vector3(0, 0, 0);
        item1.itemRotation = new Vector3(0, 0, 0);
        item1.collected = false;

        ItemData item2 = new ItemData();
        item2.itemPosition = new Vector3(1, 0, 0);
        item2.itemRotation = new Vector3(0, 0, 0);
        item2.collected = false;

        ItemData item3 = new ItemData();
        item3.itemPosition = new Vector3(2, 0, 0);
        item3.itemRotation = new Vector3(0, 0, 0);
        item3.collected = false;

        ItemData item4 = new ItemData();
        item4.itemPosition = new Vector3(-1, 0, 0);
        item4.itemRotation = new Vector3(0, 0, 0);
        item4.collected = false;

        ItemData item5 = new ItemData();
        item5.itemPosition = new Vector3(-2, 0, 0);
        item5.itemRotation = new Vector3(0, 0, 0);
        item5.collected = false;

        ItemData item6 = new ItemData();
        item6.itemPosition = new Vector3(-3, 0, 0);
        item6.itemRotation = new Vector3(0, 0, 0);
        item6.collected = false;

        ItemData item7 = new ItemData();
        item7.itemPosition = new Vector3(0, 0, -1);
        item7.itemRotation = new Vector3(0, 0, 0);
        item7.collected = false;

        itemDictionary.Add(1, item1);
        itemDictionary.Add(2, item2);
        itemDictionary.Add(3, item3);
        itemDictionary.Add(4, item4);
        itemDictionary.Add(5, item5);
        itemDictionary.Add(6, item6);
        itemDictionary.Add(7, item7);

    }   
}
