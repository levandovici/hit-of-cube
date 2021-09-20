using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Coloristic;

public class Block : MonoBehaviour
{
    [SerializeField]
    private EColor _color;



    public EColor Color => _color;



    public void SetUp(EColor color)
    {
        _color = color;

        this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Coloristic.GetColor(_color));
    }
}
