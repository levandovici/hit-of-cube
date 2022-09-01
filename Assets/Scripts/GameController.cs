using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

using static BlocksLine;

public class GameController : MonoBehaviour
{
    private const float Block_X_Offset = 10.62132f;

    private const float Block_Y_Offset = 3.628679f;

    private const float Block_Size = 7.257357f;
    
    private const int Counter_Limit_Reset = 8192;



    [SerializeField]
    private float _start_move_speed;

    [SerializeField]
    private float _max_move_speed;

    [SerializeField]
    private float _move_speed;

    [SerializeField]
    private float _move_left_and_right = 2f;

    [SerializeField]
    private float _increase_after_distance;

    [SerializeField]
    private float _increase_multiplier;


    [SerializeField]
    private Player _player;


    private bool _game_over = false;


    [SerializeField]
    private Block _block_prefab;

    [SerializeField]
    private BlocksLine[] _blocks_lines;

    [SerializeField]
    private int _blocks_lines_count = 8;

    [SerializeField]
    private float _blocks_line_offset = 32f;

    private float _blocks_line_position = 32f;

    [SerializeField]
    private int _score;

    [SerializeField]
    private int _score_limit_reset_counter = 0;

    private int _best_score;

    private bool _new_record = false;

    [SerializeField]
    private GameObject _new_record_star;

    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private int _gems;

    [SerializeField]
    private Text _gemsText;

    [SerializeField]
    private Gem _gem_prefab;

    [SerializeField]
    private Text _healthText;

    [SerializeField]
    private Transform _objects;

    [SerializeField]
    private bool _auto = false;

    [SerializeField]
    private GameObject _play_tab;

    [SerializeField]
    private GameObject _game_over_tab;

    [SerializeField]
    private Text _game_over_tab_best_score;

    [SerializeField]
    private Text _game_over_tab_score;

    [SerializeField]
    private Text _game_over_tab_gems;


    [SerializeField]
    private Button _main_menu;


    private Vector3 _start_mouse_position = Vector3.zero;



    public event Action OnCollectGem;

    public event Action OnChangeLine;

    public event Action OnEnterCorrectBlock;

    public event Action OnEnterWrongBlock;

    public event Action<int, bool> OnNewRecord;

    public event Action<float> OnZPositionChanged;

    public event Action OnGameOver;

    public event Action OnClick;



    private void Awake()
    {
        _blocks_lines = new BlocksLine[0];

        _blocks_line_position = _blocks_line_offset * 2f;

        OnGameOver += () =>
        {
            _game_over = true;

            _play_tab.gameObject.SetActive(false);

            _game_over_tab.gameObject.SetActive(true);

            _game_over_tab_best_score.text = $"Best score: {_best_score}";

            _game_over_tab_score.text = $"Score: {_score}";

            _game_over_tab_gems.text = $"Gems: {_gems}";
        };

        _player.OnTriggerBlock += (block) =>
        {
            if (block.Color == _player.Color)
            {
                OnEnterCorrectBlock.Invoke();

                _gems += 1;

                UpdateUI();

                OnCollectGem.Invoke();

                Gem gem = Instantiate(_gem_prefab, block.transform.position, Quaternion.identity, _objects);

                gem.SetUp(_move_speed * 2f);

                _player.SetUp(Player.EAnimation.eat);
            }
            else
            {
                OnEnterWrongBlock.Invoke();

                _player.DealDamage(UnityEngine.Random.Range(32, 34));

                _player.SetUp(Player.EAnimation.bump);
            }

            Destroy(block.gameObject);
        };

        _player.OnDie += () => OnGameOver.Invoke();

        _main_menu.onClick.AddListener(() =>
        {
            OnClick.Invoke();

            SceneManager.LoadScene(0);
        });
    }

    private void Start()
    {
        _player.SetUp(100);

        _play_tab.gameObject.SetActive(true);

        _game_over_tab.gameObject.SetActive(false);

        AddBlocksLines(_blocks_lines_count);

        FindPlayerTarget();
    }

    private void Update()
    {
        if (!_game_over)
        {
            MoveForward();

            ManageMoveSpeed();

            OnZPositionChanged.Invoke(transform.position.z);

            ManageMovement();

            RemoveBlocks(_player.transform.position.z - _blocks_line_offset);

            AddBlocksLines(_blocks_lines_count - _blocks_lines.Length);

            if (_player.Target == null || _player.Target.ZPosition < _player.transform.position.z - Block_Size)
                FindPlayerTarget();

            if(_auto)
            {
                AutoMode();
            }

            ManageScore();

            UpdateUI();


            if (transform.position.z >= Counter_Limit_Reset)
            {
                ResetPosition();
            }
        }
    }



    public void ContinueGame()
    {
        _game_over = false;

        _player.SetUp(100);

        _play_tab.gameObject.SetActive(true);

        _game_over_tab.gameObject.SetActive(false);
    }



    private void MoveForward()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            transform.position + Vector3.forward * _move_speed, _move_speed * Time.deltaTime);
    }

    private void ResetPosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - Counter_Limit_Reset);

        _score_limit_reset_counter++;

        _blocks_line_position -= Counter_Limit_Reset;

        ResetBlocksPosition();

        _player.ClearTarget();
    }

    private void AutoMode()
    {
        if (_player.Target != null && _player.Target.ZPosition <= transform.position.z + Block_Size * 7f)
        {
            ELine line = _player.Line;

            if (_player.Target.left != null && _player.Target.left.Color == _player.Color)
            {
                line = ELine.left;
            }
            else if (_player.Target.middle != null && _player.Target.middle.Color == _player.Color)
            {
                line = ELine.middle;
            }
            else if (_player.Target.right != null && _player.Target.right.Color == _player.Color)
            {
                line = ELine.right;
            }

            int l = (int)line;

            int pl = (int)_player.Line;

            if (l > pl)
            {
                for (int i = 0; i < l - pl; i++)
                {
                    MoveRight();
                }
            }
            else if (l < pl)
            {
                for (int i = 0; i < pl - l; i++)
                {
                    MoveLeft();
                }
            }
        }
    }

    private void ManageScore()
    {
        _score = _score_limit_reset_counter * Counter_Limit_Reset + (int)_player.transform.position.z;

        if (_score > _best_score)
        {
            _new_record_star.SetActive(true);

            OnNewRecord.Invoke(_score, !_new_record);

            SetUp(_score);

            _new_record = true;
        }
    }

    private void ManageMoveSpeed()
    {
        _move_speed = _start_move_speed * Mathf.Pow(_increase_multiplier,
                (int)(_score / _increase_after_distance));

        _move_speed = Mathf.Clamp(_move_speed, _start_move_speed, _max_move_speed);
    }

    private void ManageMovement()
    {
        bool left = false, right = false;


        if (Input.GetMouseButtonDown(0))
        {
            _start_mouse_position = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            float swipe = Input.mousePosition.x - _start_mouse_position.x;

            if (swipe < 0f)
            {
                left = true;
            }
            else if (swipe > 0f)
            {
                right = true;
            }
        }


#if UNITY_EDITOR
        left = left || Input.GetKeyDown(KeyCode.A);

        right = right || Input.GetKeyDown(KeyCode.D);
#endif


        if (left && _player.Line != ELine.left)
        {
            MoveLeft();
        }
        else if (right && _player.Line != ELine.right)
        {
            MoveRight();
        }

        _player.transform.position = Vector3.MoveTowards(_player.transform.position,
            new Vector3(_player.TargetXPosition, _player.transform.position.y,
            _player.transform.position.z), _move_speed * _move_left_and_right * Time.deltaTime);
    }

    private void MoveLeft()
    {
        _player.Line--;

        SetUpPlayerPosition();

        OnChangeLine.Invoke();
    }

    private void MoveRight()
    {
        _player.Line++;

        SetUpPlayerPosition();

        OnChangeLine.Invoke();
    }


    public void SetUp(int best_score)
    {
        _best_score = best_score;
    }


    private void SetUpPlayerPosition()
    {
        _player.SetUp((int)_player.Line * Block_X_Offset);
    }

    private void AddBlocksLine()
    {
        int min_blocks = 1;

        if (_score >= 4096)
            min_blocks++;

        if (_score >= 8192)
            min_blocks++;

        EBlocksLine blocksLine = GenerateBlocksLine(min_blocks);


        Block left = null;

        Block middle = null;

        Block right = null;


        Coloristic.TriColor triColor = Coloristic.TriColor.GetRandomTriColor();


        if ((blocksLine & EBlocksLine.left) == EBlocksLine.left)
        {
            left = InstantiateBlock(ELine.left, _blocks_line_position, triColor.left);
        }

        if ((blocksLine & EBlocksLine.middle) == EBlocksLine.middle)
        {
            middle = InstantiateBlock(ELine.middle, _blocks_line_position, triColor.middle);
        }

        if ((blocksLine & EBlocksLine.right) == EBlocksLine.right)
        {
            right = InstantiateBlock(ELine.right, _blocks_line_position, triColor.right);
        }

        BlocksLine line = new BlocksLine(left, middle, right);


        BlocksLine[] array = _blocks_lines;

        _blocks_lines = new BlocksLine[_blocks_lines.Length + 1];

        for (int i = 0; i < array.Length; i++)
        {
            _blocks_lines[i] = array[i];
        }

        _blocks_lines[array.Length] = line;


        _blocks_line_position += _blocks_line_offset;
    }

    private void AddBlocksLines(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddBlocksLine();
        }
    }

    private Block InstantiateBlock(ELine line, float positionZ, Coloristic.EColor color)
    {
        Block block = Instantiate<Block>(_block_prefab,
            new Vector3((int)line * Block_X_Offset, Block_Y_Offset, positionZ),
            Quaternion.identity, _objects);


        block.SetUp(color);


        return block;
    }

    private EBlocksLine GenerateBlocksLine(int min_blocks)
    {
        int min = 0;

        if (min_blocks == 1)
        {
            min = 1;
        }
        else if (min_blocks == 2)
        {
            min = 3;
        }
        else if (min_blocks == 3)
        {
            min = 7;
        }

        int num = UnityEngine.Random.Range(min, 8);

        return (EBlocksLine)num;
    }

    private void RemoveBlocks(float minZ)
    {
        List<BlocksLine> remove_blocks = new List<BlocksLine>();

        List<BlocksLine> blocks = new List<BlocksLine>();

        for (int i = 0; i < _blocks_lines.Length; i++)
        {
            if (_blocks_lines[i].ZPosition < minZ)
            {
                remove_blocks.Add(_blocks_lines[i]);
            }
            else
            {
                blocks.Add(_blocks_lines[i]);
            }
        }

        foreach (BlocksLine b in remove_blocks)
        {
            DestroyBlocks(b.GetBlocks());
        }

        _blocks_lines = blocks.ToArray();
    }

    private void RemoveBlocks()
    {
        RemoveBlocks(Mathf.Infinity);
    }

    private void ResetBlocksPosition()
    {
        for(int i = 0; i < _blocks_lines.Length; i++)
        {
            _blocks_lines[i].SetBlocksZPosition(_blocks_lines[i].ZPosition - Counter_Limit_Reset);
        }
    }

    private void DestroyBlocks(Block[] blocks)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            Destroy(blocks[i].gameObject);
        }
    }

    private void FindPlayerTarget()
    {
        float nearest = Mathf.Infinity;

        BlocksLine blocksLine = null;

        for (int i = 0; i < _blocks_lines_count / 2; i++)
        {
            float b = _blocks_lines[i].ZPosition;
            float p = _player.transform.position.z;

            float d = Mathf.Abs(b - p);

            if (b > p && d < nearest)
            {
                nearest = d;

                blocksLine = _blocks_lines[i];

                //because the next block z position is this + diff 
                break;
            }
        }

        if(blocksLine != null)
        _player.SetUp(blocksLine);
    }



    private void UpdateUI()
    {
        _gemsText.text = _gems.ToString("# ### ### ### ##0");

        _scoreText.text = _score.ToString("# ### ### ### ##0");

        _healthText.text = _player.Health.ToString("# ### ### ### ##0");
    }
}

[System.Serializable]
public class BlocksLine
{
    public Block left;

    public Block middle;

    public Block right;

    private float _z_position;



    public float ZPosition => _z_position;

    public int Count
    {
        get
        {
            int count = 0;


            if (left != null)
                count++;

            if (middle != null)
                count++;

            if (right != null)
                count++;


            return count;
        }
    }



    public BlocksLine(Block left, Block middle, Block right)
    {
        this.left = left;

        this.middle = middle;

        this.right = right;


        if (left != null)
        {
            _z_position = left.transform.position.z;
        }
        else if (middle != null)
        {
            _z_position = middle.transform.position.z;
        }
        else if (right != null)
        {
            _z_position = right.transform.position.z;
        }
    }



    public Block[] GetBlocks()
    {
        List<Block> blocks = new List<Block>();


        if (left != null)
            blocks.Add(left);

        if (middle != null)
            blocks.Add(middle);

        if (right != null)
            blocks.Add(right);


        return blocks.ToArray();
    }


    public void SetBlocksZPosition(float z_position)
    {
        Block[] blocks = GetBlocks();

        for (int j = 0; j < blocks.Length; j++)
        {
            Vector3 position = blocks[j].transform.position;

            position.z = z_position;

            blocks[j].transform.position = position;
        }

        _z_position = z_position;
    }



    public Coloristic.EColor GetRandomBlockColor()
    {
        List<Block> blocks = new List<Block>();


        if (left != null)
        {
            blocks.Add(left);
        }

        if (middle != null)
        {
            blocks.Add(middle);
        }

        if (right != null)
        {
            blocks.Add(right);
        }


        Block[] array = blocks.ToArray();


        if(array.Length > 0)
        return array[UnityEngine.Random.Range(0, array.Length)].Color;


        return Coloristic.EColor.black;
    }



    [Flags]
    public enum EBlocksLine
    {
        none = 0, left = 1, middle = 2, right = 4,
    }

    public enum ELine
    {
        left = -1, middle = 0, right = 1,
    }
}


public class Coloristic
{
    public static Color GetColor(EColor color)
    {
        switch (color)
        {
            case EColor.red:
                return new Color(1f, 0f, 0f);

            case EColor.green:
                return new Color(0f, 1f, 0f);

            case EColor.blue:
                return new Color(0f, 0f, 1f);


            case EColor.yellow:
                return new Color(1f, 1f, 0f);

            case EColor.cyan:
                return new Color(0f, 1f, 1f);

            case EColor.magenta:
                return new Color(1f, 0f, 1f);


            case EColor.orange:
                return new Color(1f, 0.5f, 0f);

            case EColor.chartreuse_green:
                return new Color(0.5f, 1f, 0f);

            case EColor.spring_green:
                return new Color(0f, 1f, 0.5f);

            case EColor.azure:
                return new Color(0f, 0.5f, 1f);

            case EColor.violet:
                return new Color(0.5f, 0f, 1f);

            case EColor.rose:
                return new Color(1f, 0f, 0.5f);


            case EColor.white:
                return new Color(1f, 1f, 1f);
        }

        return new Color(0f, 0f, 0f);
    }

    public static EColor GetRandomColor()
    {
        return (Coloristic.EColor)UnityEngine.Random.Range(2, 14);
    }



    public class TriColor
    {
        public EColor left;

        public EColor middle;

        public EColor right;



        public TriColor(EColor left, EColor middle, EColor right)
        {
            this.left = left;

            this.middle = middle;

            this.right = right;
        }

        public TriColor()
        {

        }



        public static TriColor GetRandomTriColor()
        {
            TriColor triColor = new TriColor();

            triColor.left = GetRandomColor();

            do
            {
                triColor.middle = GetRandomColor();
            }
            while (triColor.left == triColor.middle);

            do
            {
                triColor.right = GetRandomColor();
            }
            while (triColor.right == triColor.left || triColor.right == triColor.middle);


            return triColor;
        }
    }



    public enum EColor
    {
        black, white,
        red, green, blue,
        yellow, cyan, magenta,

        orange, chartreuse_green, spring_green, azure, violet, rose,
    }
}
