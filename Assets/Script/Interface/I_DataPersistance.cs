using Data_Logic;
using UnityEngine;

public interface I_DataPersistance
{
    public void SaveGame(ref GameData gameData);
    public void LoadGame(GameData gameData);
}
