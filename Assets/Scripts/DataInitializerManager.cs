using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Class that stores the initial info of the objects that will be saved in the game
/// </summary>
public class DataInitializerManager : MonoBehaviour
{
    public static DataInitializerManager instance;

    [SerializeField] private List<ItemPickup> itemsInWorld;
    [SerializeField] private List<Enemy> enemiesInWorld;

    [SerializeField] private List<Chest> chestInWorld;

    [SerializeField] private InputPlayer player;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        ItemPickup[] items = FindObjectsOfType<ItemPickup>();
        itemsInWorld.AddRange(items);

        Enemy[] enemies = FindObjectsOfType<Enemy>();
        enemiesInWorld.AddRange(enemies);

        Chest[] chests = FindObjectsOfType<Chest>();
        chestInWorld.AddRange(chests);

        player = FindObjectOfType<InputPlayer>();
    }

    public List<ItemPickup> GetItemInTheWorld
    {
        get { return itemsInWorld; }
    }
    public List<Enemy> GetEnemiesInTheWorld
    {
        get { return enemiesInWorld; }
    }

    public List<Chest> GetChestsInTheWorld
    {
        get { return chestInWorld; }
    }
    public InputPlayer GetPlayerInTheWorld
    {
        get { return player; }
    }
}
