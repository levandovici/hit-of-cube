using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int gems;

    public int best_score;



    public PlayerData()
    {
        gems = 0;

        best_score = 0;
    }
}
