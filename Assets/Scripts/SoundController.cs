using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField]
    private AudioSource _music;

    [SerializeField]
    private AudioSource _sfx;

    [SerializeField]
    private AudioClip _collect_gem;

    [SerializeField]
    private AudioClip _change_line_1;

    [SerializeField]
    private AudioClip _change_line_2;

    [SerializeField]
    private AudioClip _correct_block;

    [SerializeField]
    private AudioClip _wrong_block;

    [SerializeField]
    private AudioClip _new_record;



    public void SetUp(float music_volume, float sfx_volume)
    {
        _music.volume = music_volume;

        _sfx.volume = sfx_volume;
    }

    public void PlaySfx(ESfx sfx)
    {
        switch (sfx)
        {
            case ESfx.collect_gem:

                _sfx.PlayOneShot(_collect_gem);

                break;

            case ESfx.change_line:

                int r = UnityEngine.Random.Range(0, 2);

                if (r == 0)
                {
                    _sfx.PlayOneShot(_change_line_1);
                }
                else
                {
                    _sfx.PlayOneShot(_change_line_2);
                }

                break;

            case ESfx.correct_block:

                _sfx.PlayOneShot(_correct_block);

                break;

            case ESfx.wrong_block:

                _sfx.PlayOneShot(_wrong_block);

                break;

            case ESfx.new_record:

                _sfx.PlayOneShot(_new_record);

                break;
        }
    }

    public enum ESfx
    {
        collect_gem, change_line, correct_block, wrong_block, new_record,
    }
}
