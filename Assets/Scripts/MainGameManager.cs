using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    [SerializeField]
    private AudioPeer _audio_peer;

    [SerializeField]
    private BandsController _bands_controller;

    [SerializeField]
    private BandsController _bands_controller_2;

    [SerializeField]
    private GameController _game_controller;

    [SerializeField]
    SoundController _sound_controller;

    [SerializeField]
    private AdsManager _ads_manager;



    private void Awake()
    {
        SaveLoadManager.Load();


        _bands_controller.OnGetBand += _audio_peer.GetBand;

        _bands_controller.OnGetBandBuffer += _audio_peer.GetBandBuffer;

        _bands_controller.OnGetNormalizedBand += _audio_peer.GetNormalizedBand;

        _bands_controller.OnGetNormaliedBandBuffer += _audio_peer.GetNormalizedBandBuffer;


        _bands_controller_2.OnGetBand += _audio_peer.GetBand;

        _bands_controller_2.OnGetBandBuffer += _audio_peer.GetBandBuffer;

        _bands_controller_2.OnGetNormalizedBand += _audio_peer.GetNormalizedBand;

        _bands_controller_2.OnGetNormaliedBandBuffer += _audio_peer.GetNormalizedBandBuffer;


        _game_controller.OnZPositionChanged += _bands_controller.SetUpZ;

        _game_controller.OnZPositionChanged += _bands_controller_2.SetUpZ;

        _game_controller.OnCollectGem += () =>
        {
            _sound_controller.PlaySfx(SoundController.ESfx.collect_gem);

            SaveLoadManager.PlayerData.gems++;
        };

        _game_controller.OnChangeLine += () => _sound_controller.PlaySfx(SoundController.ESfx.change_line);

        _game_controller.OnEnterCorrectBlock += () => _sound_controller.PlaySfx(SoundController.ESfx.correct_block);

        _game_controller.OnEnterWrongBlock += () => _sound_controller.PlaySfx(SoundController.ESfx.wrong_block);

        _game_controller.OnNewRecord += (new_record, music) =>
        {
            if (music)
                _sound_controller.PlaySfx(SoundController.ESfx.new_record);

            SaveLoadManager.PlayerData.best_score = new_record;
        };

        _game_controller.OnGameOver += () =>
        {
            SaveLoadManager.Save();
        };

        _game_controller.OnShowAd += () =>
        {
            _ads_manager.ShowAd(AdsManager.EReward.continue_game);
        };

        _game_controller.OnClick += () =>
        {
            _sound_controller.PlaySfx(SoundController.ESfx.click);
        };


        _ads_manager.OnCanShowAdChanged += (b) =>
        {
            _game_controller.SetUp(b);
        };

        _ads_manager.OnReward += (reward) =>
        {
            if (reward == AdsManager.EReward.continue_game)
            {
                _game_controller.ContinueGame();
            }
        };


        _ads_manager.IsAdsEnabled = true;

        _ads_manager.InitializeAds();
    }

    private void Start()
    {
        _game_controller.SetUp(SaveLoadManager.PlayerData.best_score);
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
