using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public Tile flashTile;
    public float flashDelay = 0.05f;
    public int flashCount = 3;
    public NextPreviewPrefab nextPreviewPrefab;

    private TetrominoData nextTetromino;
    private bool isClearingLines;
    private bool isProcessingLines = false;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        GenerateNext();
        SpawnPiece();
    }

    private void GenerateNext()
    {
        int random = Random.Range(0, tetrominoes.Length);
        nextTetromino = tetrominoes[random];

        nextPreviewPrefab.SetNext(nextTetromino);
    }

    public void SpawnPiece()
    {
        if (isProcessingLines) return;

        TetrominoData data = nextTetromino;

        this.activePiece.Initialize(this, this.spawnPosition, data);

        if (IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
            GenerateNext();
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        StopAllCoroutines();

        this.tilemap.ClearAllTiles();

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetAll();
        }

        activePiece.ResetState();

        GenerateNext();
        SpawnPiece();
    }


    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        if (isClearingLines || isProcessingLines) return;
        StartCoroutine(ClearLinesRoutine());
    }

    private IEnumerator ClearLinesRoutine()
    {
        isClearingLines = true;
        isProcessingLines = true;

        RectInt bounds = this.Bounds;
        int linesCleared = 0;

        for (int row = bounds.yMin; row < bounds.yMax;)
        {
            if (IsLineFull(row))
            {
                yield return StartCoroutine(FlashLine(row));
                LineClear(row);
                linesCleared++;
            }
            else
            {
                row++;
            }
        }

        if (linesCleared > 0 && ScoreManager.Instance != null)
        {
            SoundManager.Instance?.PlayLineClear();
            ScoreManager.Instance.OnLinesCleared(linesCleared);
        }

        isProcessingLines = false;
        isClearingLines = false;

        SpawnPiece();
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            if (!this.tilemap.HasTile(new Vector3Int(col, row, 0)))
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator FlashLine(int row)
    {
        RectInt bounds = this.Bounds;

        for (int i = 0; i < flashCount; i++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                tilemap.SetTile(new Vector3Int(col, row, 0), flashTile);
            }

            yield return new WaitForSeconds(flashDelay);

            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                tilemap.SetTile(new Vector3Int(col, row, 0), null);
            }

            yield return new WaitForSeconds(flashDelay);
        }
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            tilemap.SetTile(new Vector3Int(col, row, 0), null);
        }

        for (int y = row + 1; y < bounds.yMax; y++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int from = new Vector3Int(col, y, 0);
                Vector3Int to = new Vector3Int(col, y - 1, 0);

                TileBase tile = tilemap.GetTile(from);
                tilemap.SetTile(to, tile);
            }
        }

        int topRow = bounds.yMax - 1;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            tilemap.SetTile(new Vector3Int(col, topRow, 0), null);
        }
    }
}
