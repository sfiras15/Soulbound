using System;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;

    public InputPlayer inputPlayer;

    public int playerLevel;

    public static event Action<int> onHittingEnemy;

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
        inputPlayer = FindObjectOfType<InputPlayer>();
    }

    public void HitEnemy(int id)
    {
        if (onHittingEnemy != null)
        {
            onHittingEnemy(id);
        }
    }

}
