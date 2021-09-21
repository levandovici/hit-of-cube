using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Coloristic;

public class Block : MonoBehaviour
{
    [SerializeField]
    private EColor _color;

    [SerializeField]
    private MeshRenderer _mesh_renderer;



    public EColor Color => _color;



    public void Awake()
    {
        _mesh_renderer = GetComponent<MeshRenderer>();
    }



    public void SetUp(EColor color)
    {
        _color = color;

        _mesh_renderer.material.color = Coloristic.GetColor(_color);
    }
}
