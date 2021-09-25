using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class SaveData
{
    public int version = 1;
    public string data = null;



    public SaveData(PlayerData data)
    {
        this.version = 1;
        this.data = Encrypt(data, this.version);
    }



    public PlayerData PlayerData()
    {
        return Decrypt(data, version);
    }



    private string Encrypt(PlayerData data, int version)
    {
        string dat = JsonUtility.ToJson(data);
        byte[] bytes = Encoding.UTF8.GetBytes(dat);

        int code = GetCode(version);

        for(int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = bytes[i] = (byte)((int)bytes[i] ^ code);
        }


        return Encoding.UTF8.GetString(bytes);
    }

    private PlayerData Decrypt(string data, int version)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);

        int code = GetCode(version);

        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = bytes[i] = (byte)((int)bytes[i] ^ code);
        }


        return JsonUtility.FromJson<PlayerData>(Encoding.UTF8.GetString(bytes));
    }

    private int GetCode(int version)
    {
        switch(version)
        {
            case 1:
                return 25;

            default:
                return 0;
        }
    }
}
