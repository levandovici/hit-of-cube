using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BandsController : MonoBehaviour
{
    [SerializeField]
    private float _z_start_offset = 0f;

    [SerializeField]
    private float _start_scale;

    [SerializeField]
    private float _scale_multiplier;

    [SerializeField]
    private int _cubes_count;

    [SerializeField]
    private float _cubes_offset;

    [SerializeField]
    private GameObject _cube_prefab;

    [SerializeField]
    private GameObject[] _cubes;

    private Vector3[] _scale_targets;

    [SerializeField]
    private bool _use_size_band_buffer = false;

    [SerializeField]
    private bool _use_size_normalized_band = false;

    [SerializeField]
    private Color _min_emission_color = new Color();

    [SerializeField]
    private Color _max_emission_color = new Color();

    [SerializeField]
    private bool _use_min_max_no_interpolation = false;

    [SerializeField]
    private bool _use_color_band_buffer = false;

    [SerializeField]
    private Light[] _lights;

    [SerializeField]
    private float _min_intensity;

    [SerializeField]
    private float _max_intensity;

    [SerializeField]
    private bool _use_light_band_buffer = false;



    public event Func<int, float> OnGetBand;

    public event Func<int, float> OnGetBandBuffer;

    public event Func<int, float> OnGetNormalizedBand;

    public event Func<int, float> OnGetNormaliedBandBuffer;




    private void Awake()
    {
        if(_cubes != null)
        for(int i = 0; i < _cubes.Length; i++)
        {
                if (_cubes[i] != null)
                    Destroy(_cubes[i].gameObject);
        }

        _cubes = new GameObject[_cubes_count];

        for(int i = 0; i < _cubes_count; i++)
        {
            _cubes[i] = Instantiate(_cube_prefab, new Vector3(transform.position.x, 0f, i * _cubes_offset), Quaternion.identity, transform);
        }
    }



    private void Update()
    {
        for (int i = 0; i < _cubes.Length; i++)
        {
            float s = _use_size_band_buffer ?
                _use_size_normalized_band ? OnGetNormaliedBandBuffer.Invoke(i) : OnGetBandBuffer.Invoke(i) :
                _use_size_normalized_band ? OnGetNormalizedBand.Invoke(i) : OnGetNormalizedBand.Invoke(i);

            _cubes[i].transform.localScale = new Vector3(_cubes[i].transform.localScale.x,
                _start_scale + (s * _scale_multiplier), _cubes[i].transform.localScale.z);
        }

        for (int i = 0; i < _lights.Length; i++)
        {
            float l = _use_light_band_buffer ? OnGetNormaliedBandBuffer.Invoke(i) : OnGetNormalizedBand.Invoke(i);

            _lights[i].intensity = l * (_max_intensity - _min_intensity) + _min_intensity;
        }
    }


    private void LateUpdate()
    {
        for (int i = 0; i < _cubes.Length; i++)
        {
            float c = _use_color_band_buffer ? OnGetNormaliedBandBuffer.Invoke(i) : OnGetNormalizedBand.Invoke(i);

            Color color = new Color();

            if (_use_min_max_no_interpolation)
            {
                color = new Color(c * (_max_emission_color.r + _min_emission_color.r) + _min_emission_color.r,
                  c * (_max_emission_color.g + _min_emission_color.g) + _min_emission_color.g,
                  c * (_max_emission_color.b + _min_emission_color.b) + _min_emission_color.b,
                  c * (_max_emission_color.a + _min_emission_color.a) + _min_emission_color.a);
            }
            else
            {
                color = new Color(Mathf.Lerp(_min_emission_color.r, _max_emission_color.r, c),
                  Mathf.Lerp(_min_emission_color.g, _max_emission_color.g, c),
                  Mathf.Lerp(_min_emission_color.b, _max_emission_color.b, c),
                  Mathf.Lerp(_min_emission_color.a, _max_emission_color.a, c));
            }

            _cubes[i].GetComponent<MeshRenderer>().material.color = color;
        }
    }



    public void SetUpZ(float z_position)
    {
        for (int i = 0; i < _cubes.Length; i++)
        {
            _cubes[i].transform.position = new Vector3(transform.position.x,
                0f, _z_start_offset + z_position - z_position % _cubes_offset + i * _cubes_offset);
        }
    }
}
