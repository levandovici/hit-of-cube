using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class SimpleSaveLoadController : ISaveLoadController
{
    public PlayerData Load(string path)
    {
        if(File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);

                return JsonUtility.FromJson<PlayerData>(json);
            }
            catch
            {
                return new PlayerData();
            }
        }
        else
        {
            return new PlayerData();
        }
    }

    public void Save(PlayerData player_data, string path)
    {
        string json = JsonUtility.ToJson(player_data);

        File.WriteAllText(path, json);
    }
}
