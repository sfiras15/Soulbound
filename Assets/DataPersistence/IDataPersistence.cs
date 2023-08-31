using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Interface to be used by every class that needs saving/loading data
public interface IDataPersistence
{

    void LoadData(GameData gameData);


    // we passed it by ref so we could change the gameData when we save
    void SaveData(ref GameData gameData);
}
