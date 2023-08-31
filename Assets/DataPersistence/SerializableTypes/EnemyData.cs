using UnityEngine;


//Class for serializing information about the enemies in the game
[System.Serializable]
public class EnemyData
{
    public Vector3 enemyPosition;
    public Vector3 enemyRotation;
    public bool killed;

}
