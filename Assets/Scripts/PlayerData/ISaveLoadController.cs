using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveLoadController
{
    public void Save(PlayerData player_data, string path);

    public PlayerData Load(string path);
}
