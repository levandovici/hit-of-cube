using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static Coloristic;

public class Player : MonoBehaviour
{
    [SerializeField]
    private EColor _color;

    [SerializeField]
    private BlocksLine _target;

    [SerializeField]
    private int _health;

    [SerializeField]
    private int _max_health;

    [SerializeField]
    private BlocksLine.ELine _line = BlocksLine.ELine.middle;

    [SerializeField]
    private MeshRenderer _mesh_renderer;

    [SerializeField]
    private Animator _animator;

    private float _target_x_position;



    public event Action<Block> OnTriggerBlock;

    public event Action OnDie;


    public EColor Color => _color;

    public BlocksLine Target => _target;

    public BlocksLine.ELine Line
    {
        get
        {
            return _line;
        }

        set
        {
            _line = value;
        }
    }

    public int Health => _health;

    public float TargetXPosition => _target_x_position;



    public void SetUp(BlocksLine target)
    {
        _target = target;

        _color = target.GetRandomBlockColor();

        if(target.Count < 3)
        {
            int r = UnityEngine.Random.Range(0, 4);

            if(r < 3)
            {
                _color = GetRandomColor();
            }
        }

       _mesh_renderer.material.color = Coloristic.GetColor(_color);
    }

    public void SetUp(int max_health)
    {
        _health = _max_health = max_health;
    }

    public void SetUp(EAnimation animation)
    {
        switch(animation)
        {
            case EAnimation.eat:

                _animator.SetTrigger("Eat");

                break;

            case EAnimation.bump:

                _animator.SetTrigger("Bump");

                break;
        }
    }

    public void SetUp(float target_x_position)
    {
        _target_x_position = target_x_position;
    }

    public void ClearTarget()
    {
        _target = null;
    }

    public void DealDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            _health = 0;

            OnDie.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Block")
        {
            OnTriggerBlock.Invoke(other.gameObject.GetComponent<Block>());
        }
    }

    public enum EAnimation
    {
        idle, eat, bump
    }

}
