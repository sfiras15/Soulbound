using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class XpToLevel
{
    public int level;
    public int xp;
}
/// <summary>
/// handles the xp/level of the player
/// </summary>
public class PlayerManager : MonoBehaviour,IDataPersistence
{

    public static PlayerManager instance;

    public int playerLevel;

    private int currentPlayerXp;
    private int nextLevelXp;

    [SerializeField] private Bar xpBar;

    [SerializeField] private XpToLevel[] xpToLevelsList;
    private Dictionary<int, int> xpToLevelDictionary = new Dictionary<int, int>();


    public void LoadData(GameData data)
    {
        this.playerLevel = data.playerLevel;
        this.currentPlayerXp = data.playerCurrentXp;
        this.nextLevelXp = data.nextLevelXp;
    }
    public void SaveData(ref GameData data)
    {
        data.playerLevel = this.playerLevel;
        data.playerCurrentXp = this.currentPlayerXp;
        data.nextLevelXp = this.nextLevelXp;
    }

    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        foreach (XpToLevel element in xpToLevelsList)
        {
            xpToLevelDictionary.Add(element.level, element.xp);
        }
    }

    public void AddXp(int value)
    {
        currentPlayerXp += value;
        UpdateLevel(currentPlayerXp);
        xpBar.UpdateBar(nextLevelXp, currentPlayerXp);
    }

    public void UpdateLevel(int xp)
    {
        for (int i = playerLevel; i < xpToLevelDictionary.Count; i++)
        {
            if (xp >= xpToLevelDictionary[i])
            {
                playerLevel = i;
                nextLevelXp = xpToLevelDictionary[i + 1];
            }
        }
    }





}
