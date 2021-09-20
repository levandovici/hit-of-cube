using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Instancer : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private GameObject[] _instances = new GameObject[512];



    public event Func<int, float> OnGetSample;



    void Start()
    {
        for(int i = 0; i < _instances.Length; i++)
        {
            GameObject obj = Instantiate(_prefab);

            obj.transform.position = this.transform.position;

            obj.transform.parent = this.transform;

            obj.name = $"Cube-{i+1}";

            this.transform.eulerAngles = new Vector3(0f, -0.703125f * i, 0f);

            obj.transform.position = Vector3.forward * 1000f;

            _instances[i] = obj;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < _instances.Length; i++)
        {
            _instances[i].transform.localScale = new Vector3(10f, (100000f * OnGetSample(i)) + 2f, 10f);
        }
    }
}
