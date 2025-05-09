using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private Text _score;

    [SerializeField]
    private Text _gems;

    [SerializeField]
    private Button _play;

    [SerializeField]
    private Button _privacy_policy;

    [SerializeField]
    private SoundController _sound_controller;

    [SerializeField]
    private SwipeBlocks _swipe_blocks;



    private void Awake()
    {
        SaveLoadManager.Load();

        _play.onClick.AddListener(() =>
        {
            _sound_controller.PlaySfx(SoundController.ESfx.click);

            SceneManager.LoadScene(_swipe_blocks.Selected + 1);
        });

        _privacy_policy.onClick.AddListener(() =>
        {
            _sound_controller.PlaySfx(SoundController.ESfx.click);

            Application.OpenURL("https://games.limonadoent.com/privacy-policy.html");
        });
    }

    private void Start()
    {
        _score.text = SaveLoadManager.PlayerData.best_score.ToString();

        _gems.text = SaveLoadManager.PlayerData.gems.ToString();
    }



    private void OnApplicationPause(bool pause)
    {
        if(pause)
        SaveLoadManager.Save();
    }

    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        SaveLoadManager.Save();
    }

    private void OnApplicationQuit()
    {
        SaveLoadManager.Save();
    }
}
