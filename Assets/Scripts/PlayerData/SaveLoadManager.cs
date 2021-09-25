using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public static class SaveLoadManager
{
    private const string PlayerDataPath = "hit-of-cube.data";

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

    private static string GetPath
    {
        get
        {
#if UNITY_EDITOR

            return Path.Combine(Application.dataPath, PlayerDataPath);

#else

            return Path.Combine(Application.persistentDataPath, PlayerDataPath);

#endif
        }
    }



    public static void Save()
    {
        File.WriteAllText(GetPath, Encript(PlayerData));
    }

    public async static void SaveAsync()
    {
        using (StreamWriter writer = File.CreateText(GetPath))
        {
            await writer.WriteAsync(Encript(PlayerData));
        }
    }

    public static void Load()
    {
        if (File.Exists(GetPath))
        {
            _player_data = Decript(File.ReadAllText(GetPath));
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
