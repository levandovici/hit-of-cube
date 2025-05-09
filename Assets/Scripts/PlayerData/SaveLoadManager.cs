using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public static class SaveLoadManager
{
    private const string PlayerDataPath = "data";

    private static PlayerData _player_data = null;



    public static PlayerData PlayerData
    {
        get
        {
            if (_player_data == null)
            {
                Load();
            }

            return _player_data;
        }
    }



    public static void Save()
    {
        PlayerPrefs.SetString(PlayerDataPath, Encript(PlayerData));
    }

    public static void Load()
    {
        if (PlayerPrefs.HasKey(PlayerDataPath))
        {
            _player_data = Decript(PlayerPrefs.GetString(PlayerDataPath));
        }
        else
        {
            _player_data = new PlayerData();
        }
    }



    private static string Encript(PlayerData data)
    {
        SaveData saveData = new SaveData(data);

        return JsonUtility.ToJson(saveData);
    }

    private static PlayerData Decript(string data)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(data);

        return saveData.PlayerData();
    }
}
