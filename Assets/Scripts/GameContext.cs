using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameContext : MonoBehaviour
{
    public int gridColumns = 3;
    public int gridRows = 3;
    public int bricksInitCount = 3;
    public float movementSpeed = 0.25f;

    private DiContainer _diContainer;
    private IBrickColorsService _brickColorsService;
    private ScoreService _scoreService;

    [SerializeField] private Cell _cellBrefab;
    [SerializeField] private Brick _brickBrefab;
    [SerializeField] private SpriteRenderer _board;

    private int _cellSize = 2;
    private bool _canMove = true;
    private List<Cell> cells = new List<Cell>();

    [Inject]
    public void Construct(DiContainer diContainer,
        IBrickColorsService brickColorsService,
        ScoreService scoreService)
    {
        _diContainer = diContainer;
        _brickColorsService = brickColorsService;
        _scoreService = scoreService;
        SwipeHandler.onSwipe += OnSwipeGesture;
        InitGrid();
    }

    private void OnDestroy() => SwipeHandler.onSwipe -= OnSwipeGesture;


    private void Update()
    {
        //guys it is just for testing :)
        if (_canMove && !Application.isMobilePlatform)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                OnSwipeGesture(Vector2.left);
            else if (Input.GetKey(KeyCode.RightArrow))
                OnSwipeGesture(Vector2.right);
            else if (Input.GetKey(KeyCode.UpArrow))
                OnSwipeGesture(Vector2.up);
            else if (Input.GetKey(KeyCode.DownArrow))
                OnSwipeGesture(Vector2.down);
        }
    }

    private void OnSwipeGesture(Vector2 direction)
    {
        _canMove = false;
        List<Brick> orderedBySwipeDirection = cells
            .Select(c => c.GetBrick())
            .Where(b => b is not null)
            .OrderBy(c => c.Position.x)
            .ThenBy(c => c.Position.y)
            .ToList();

        if (direction == Vector2.up || direction == Vector2.right)
            orderedBySwipeDirection.Reverse();

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        foreach (Brick brick in orderedBySwipeDirection)
        {
            //in first iteration it is equals to currentBrickCell
            Cell next = GetCellForBrick(brick);

            HandeBrickMoveAbility(brick, next, direction, sequence);
        }

        sequence.OnComplete(() =>
        {
            foreach (Brick brick in orderedBySwipeDirection)
            {
                Brick mergeBrick = brick.GetMergeBlock();
                if (mergeBrick is not null)
                {
                    Brick newbrick = _diContainer.InstantiatePrefab(_brickBrefab, mergeBrick.Position,
                    Quaternion.identity, _board.transform).GetComponent<Brick>();

                    newbrick.SetPosition(mergeBrick.Position);
                    newbrick.Init(mergeBrick.Value * 2);
                    Destroy(mergeBrick);
                    Destroy(brick);

                    Cell cell = GetCellForBrick(newbrick);
                    cell.PutBrick(newbrick);

                    _scoreService.AddScore();
                }
            }
            _canMove = true;
            GenerateBrick(1);

        });

    }

    private void Destroy(Brick brick)
    {
        Destroy(brick.gameObject);
        Cell brickCell = GetCellForBrick(brick);
        brickCell.RemoveBrick();
    }


    //Recursive method
    private void HandeBrickMoveAbility(Brick brick, Cell next, Vector2 direction, DG.Tweening.Sequence sequence)
    {
        Cell currentBrickCell = GetCellForBrick(brick);

        if (currentBrickCell != next)
            currentBrickCell.RemoveBrick();

        next.PutBrick(brick);
        var a = direction * _cellSize;
        Cell targetCell = cells.FirstOrDefault(c => c.Position == next.Position + direction * _cellSize);

        if (targetCell is not null)
        {
            Brick targetCellBrick = targetCell.GetBrick();
            if (brick.GetMergeBlock() is null && targetCellBrick is not null && targetCellBrick.Value == brick.Value && targetCellBrick.GetMergeBlock() is null)
            {
                brick.Merge(targetCellBrick);
                next = targetCell;
            }
            else if (!targetCell.HasBrick() && brick.GetMergeBlock() is null)
                next = targetCell;
        }
        if (next.Position != brick.Position)
            HandeBrickMoveAbility(brick, next, direction, sequence);
        else
            sequence.Insert(0, brick.transform.DOMove(brick.Position, movementSpeed));
    }

    public void Reset()
    {
        _brickColorsService.Reset();
        _scoreService.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void InitGrid()
    {
        for (int r = 0; r < gridRows; r++)
            for (int c = 0; c < gridColumns; c++)
            {
                Vector2 cellPosition = new Vector2(CalculateOffset(c, gridColumns), CalculateOffset(r, gridRows));
                Cell cell = _diContainer.InstantiatePrefab(_cellBrefab,
                   cellPosition,
                    Quaternion.identity, _board.transform).GetComponent<Cell>();
                cell.SetPosition(cellPosition);
                cells.Add(cell);
            }

        _board.size = new Vector2(gridColumns * _cellSize, gridRows * _cellSize);

        Camera.main.transform.position = new Vector3(_board.transform.position.x,
            _board.transform.position.y,
            Camera.main.transform.position.z);

        Camera.main.orthographicSize = math.max(gridColumns, gridRows) * 2;


        GenerateBrick(bricksInitCount);
    }

    private void GenerateBrick(int count = 1)
    {
        IEnumerable<Cell> availableCells = GetAvailableCells();

        bool ifBoardIsFull = availableCells.Count() <= 1;
        if (ifBoardIsFull)
        {
            _canMove = false;
            _scoreService.GameOverPopup();
        }

        List<Cell> freeCells = availableCells.OrderBy(c => UnityEngine.Random.value).Take(count).ToList();

        foreach (Cell freeCell in freeCells)
        {

            Brick brick = _diContainer.InstantiatePrefab(_brickBrefab,
                      freeCell.Position,
                       Quaternion.identity, _board.transform).GetComponent<Brick>();

            freeCell.PutBrick(brick);
        }

    }

    private int CalculateOffset(int value, int rowOrColumnCount) => value * _cellSize - rowOrColumnCount + 1;

    private IEnumerable<Cell> GetAvailableCells() => cells.Where(c => !c.HasBrick());

    private Cell GetCellForBrick(Brick brick) => cells.FirstOrDefault(c => c.Position == brick.Position);

}
