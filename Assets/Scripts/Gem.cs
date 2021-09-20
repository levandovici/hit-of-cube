using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField]
    private float _move_speed = 32f;

    [SerializeField]
    private float _destroy_after = 4f;



    public void Awake()
    {
        Destroy(gameObject, _destroy_after);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * _move_speed * Time.deltaTime, Space.World);
    }



    public void SetUp(float move_speed)
    {
        _move_speed = move_speed;
    }
}
