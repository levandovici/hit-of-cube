using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private float[] _samples = new float[512];

    [SerializeField]
    private float[] _bands = new float[8];

    [SerializeField]
    private float[] _bands_buffer = new float[8];

    private float[] _buffer_decrease = new float[8];

    [SerializeField]
    private float _decrease_value = 0.005f;

    [SerializeField]
    private float _decrease_value_multiplier = 1.2f;

    [SerializeField]
    private float[] _bands_higests = new float[8];

    [SerializeField]
    private float[] _normalized_bands = new float[8];

    [SerializeField]
    private float[] _normalized_bands_buffer = new float[8];



    public float[] GetSamples()
    {
        return _samples;
    }

    public float GetSample(int sample)
    {
        while(sample >= _samples.Length)
        {
            sample -= _samples.Length;
        }

        return _samples[sample];
    }

    public float[] GetBands()
    {
        return _bands;
    }

    public float GetBand(int band)
    {
        while (band >= _bands.Length)
        {
            band -= _bands.Length;
        }

        return _bands[band];
    }

    public float[] GetBandsBuffer()
    {
        return _bands_buffer;
    }

    public float GetBandBuffer(int band_buffer)
    {
        while (band_buffer >= _bands_buffer.Length)
        {
            band_buffer -= _bands_buffer.Length;
        }

        return _bands_buffer[band_buffer];
    }

    public float[] GetNormalizedBands()
    {
        return _normalized_bands;
    }

    public float GetNormalizedBand(int band)
    {
        while (band >= _normalized_bands.Length)
        {
            band -= _normalized_bands.Length;
        }

        return _normalized_bands[band];
    }

    public float[] GetNormalizedBandsBuffer()
    {
        return _normalized_bands_buffer;
    }

    public float GetNormalizedBandBuffer(int band_buffer)
    {
        while (band_buffer >= _normalized_bands_buffer.Length)
        {
            band_buffer -= _normalized_bands_buffer.Length;
        }

        return _normalized_bands_buffer[band_buffer];
    }



    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }



    private void Update()
    {
        GetSpectrumAudioSource();

        CreateBands();

        CreateBandsBuffer();

        CreateNormalizedBands();
    }

    private void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    private void CreateBands()
    {
        int count = 0;

        for(int i = 0; i < _bands.Length; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if(i == 7)
            {
                sampleCount += 2;
            }

            float average = 0f;

            for(int j = 0; j < sampleCount; j++)
            {
                average += _samples[count++] * count;
            }

            average /= count;

            _bands[i] = average * 10f;
        }
    }

    private void CreateBandsBuffer()
    {
        for(int i = 0; i < _bands_buffer.Length; i++)
        {
            if(_bands[i] > _bands_buffer[i])
            {
                _bands_buffer[i] = _bands[i];

                _buffer_decrease[i] = _decrease_value;
            }
            else if(_bands[i] < _bands_buffer[i])
            {
                _bands_buffer[i] -= _buffer_decrease[i];

                _buffer_decrease[i] *= _decrease_value_multiplier;
            }
        }
    }

    private void CreateNormalizedBands()
    {
        for(int i = 0; i < _normalized_bands.Length; i++)
        {
            if(_bands[i] > _bands_higests[i])
            {
                _bands_higests[i] = _bands[i];
            }

            _normalized_bands[i] = _bands[i] / _bands_higests[i];
            _normalized_bands_buffer[i] = _bands_buffer[i] / _bands_higests[i];
        }
    }
}
