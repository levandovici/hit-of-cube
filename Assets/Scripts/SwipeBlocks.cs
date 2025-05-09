using UnityEngine;

public class SwipeBlocks : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _blocks;

    [SerializeField]
    private float _offset = 2.5f;

    [SerializeField]
    private int _selected = 0;

    [SerializeField]
    private bool _move = false;

    [SerializeField]
    private float _moveSpeed = 5f;

    [SerializeField]
    private Vector3 _lastMousePosition = Vector3.zero;



    public int Selected
    {
        get
        {
            return _selected;
        }
    }



    private void Awake()
    {
        
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;

            Debug.Log($"Start mouse position: {Input.mousePosition}");
        }
        else if(Input.GetMouseButtonUp(0))
        {
            Vector3 dir = Input.mousePosition - _lastMousePosition;

            dir = new Vector3(dir.x / Screen.width * 2f - 1f, dir.y / Screen.height * 2f - 1f, dir.z);

            if(dir.x > 0.25f)
            {
                Left();
            }
            else if(dir.x < 0.25f)
            {
                Right();
            }

            Debug.Log($"End mouse position: {Input.mousePosition}");
        }

        if (_move)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector3 target = Vector3.zero;

        for(int i = 0; i < _blocks.Length; i++)
        {
            target = new Vector3(i * _offset - _selected * _offset, _blocks[i].transform.position.y, _blocks[i].transform.position.z);

            _blocks[i].transform.position = Vector3.MoveTowards(_blocks[i].transform.position, target, _moveSpeed * Time.deltaTime);

        }

        if (Vector3.Distance(_blocks[_blocks.Length - 1].transform.position, target) < 0.001f)
        {
            _move = false;
        }
    }

    private void Left()
    {
        _selected--;

        if (_selected < 0)
        {
            _selected = 0;
        }
        else
        {
            _move = true;
        }
    }

    private void Right()
    {
        _selected++;

        if (_selected >= _blocks.Length)
        {
            _selected = _blocks.Length - 1;
        }
        else
        {
            _move = true;
        }
    }
}
