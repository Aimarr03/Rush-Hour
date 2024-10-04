using Data_Logic;
using UnityEngine;

public interface I_DataPersistance
{
    public void SaveData(ref GameData gameData);
    public void LoadData(GameData gameData);
}
