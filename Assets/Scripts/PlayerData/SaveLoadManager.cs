using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public static class SaveLoadManager
{
    private const string PlayerDataPath = "player.data";

    private static ISaveLoadController _save_load_controller = new SimpleSaveLoadController();

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
#if UNITY_ANDROID

            return Path.Combine(Application.persistentDataPath, PlayerDataPath);

#else
            return Path.Combine(Application.dataPath, PlayerDataPath);
#endif
        }
    }



    public static void Save()
    {
        _save_load_controller.Save(PlayerData, GetPath);
    }

    public static void Load()
    {
        _player_data = _save_load_controller.Load(GetPath);
    }
}
