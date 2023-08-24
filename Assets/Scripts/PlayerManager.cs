using System;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;

    public InputPlayer inputPlayer;

    public int playerLevel;

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



}
