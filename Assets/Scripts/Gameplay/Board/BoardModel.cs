
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardModel : IBoardModel
{
   

    
    // Board State
    public Dot[,] DotGrid { get; private set; }
    public TileModel[,] TileGrid { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int DotCount => _dotsById.Count;

    private readonly Dictionary<string, Dot> _dotsById = new();
    private readonly Dictionary<string, TileModel> _tilesById = new();

    // Board Queries
    public List<Dot> GetAllDots() => _dotsById.Values.ToList();
    public List<TileModel> GetAllTiles() => _tilesById.Values.ToList();
    
    
    // Events for the Presenter to subscribe to

    public event Action<Dot> OnDotCleared;
    public event Action<Dot> OnDotSpawned;
    public event Action<TileModel> OnTileRemoved;
    public event Action<TileModel> OnTileSpawned;
    public BoardModel(LevelData level)
    {
        Width = level.width;
        Height = level.height;
        DotGrid = new Dot[Width, Height];
        TileGrid = new TileModel[Width, Height];
       
    }

    public List<Dot> InitDots(LevelData level)
    {
        var dots = new List<Dot>();

        if (level.dotsOnBoard != null)
        {
            foreach (var spawn in level.dotsOnBoard)
            {
                var dot = DotFactory.CreateDot(spawn);
                if (dot != null)
                    dots.Add(dot);
            }
        }

        return dots;
    }
    public List<TileModel> InitTiles(LevelData level)
    {
        var tiles = new List<TileModel>();
        foreach (var spawn in level.tilesOnBoard)
        {
            var tile = TileFactory.CreateTileModel(spawn);
            if (tile != null)
                tiles.Add(tile);
            
        }
        return tiles;
    }

   

    
   
    public void SpawnTile(TileModel tile)
    {
        if (!IsValidPosition(tile.GridPosition))
        {
            Debug.LogError($"Attempted to place tile outside board bounds: {tile.GridPosition}");
            return;
        }
        if (TileGrid[tile.GridPosition.x, tile.GridPosition.y] != null)
        {
            throw new ArgumentException("A tile already exists at this position: " + tile.GridPosition);
        }

        _tilesById.Add(tile.ID, tile);
        TileGrid[tile.GridPosition.x, tile.GridPosition.y] = tile; // Place in grid
        OnTileSpawned?.Invoke(tile);
    }
    public void SpawnDot(Dot dot)
    {

        if (TryPlaceDot(dot))
        {
            OnDotSpawned?.Invoke(dot);
        }
    }
   
    public bool TryPlaceDot(Dot dot)
    {
        if (!IsValidPosition(dot.GridPosition))
        {
            Debug.LogError($"Attempted to place dot outside board bounds: {dot.GridPosition}");
            return false;
        }
        else if (_dotsById.TryAdd(dot.ID, dot))
        {
            DotGrid[dot.GridPosition.x, dot.GridPosition.y] = dot;
            return true;
        }
        return false;
    }

    public void RemoveDot(string id)
    {
        if (_dotsById.TryGetValue(id, out Dot dotToRemove))
        {
            _dotsById.Remove(id);
            DotGrid[dotToRemove.GridPosition.x, dotToRemove.GridPosition.y] = null;
            OnDotCleared?.Invoke(dotToRemove);
            
        }
    }
    public void RemoveTile(string id)
    {
        if (_tilesById.TryGetValue(id, out TileModel tileToRemove))
        {
            _tilesById.Remove(id);
            TileGrid[tileToRemove.GridPosition.x, tileToRemove.GridPosition.y] = null;
            OnTileRemoved?.Invoke(tileToRemove);

        }
    }
    public void MoveDot(string id, Vector2Int toPosition)
    {
        if (_dotsById.TryGetValue(id, out Dot dotToMove))
        {
            Vector2Int fromPosition = dotToMove.GridPosition;

            DotGrid[fromPosition.x, fromPosition.y] = null;
            DotGrid[toPosition.x, toPosition.y] = dotToMove;
            dotToMove.GridPosition = toPosition;
        }
    }
   

    public Dot GetDotAt(Vector2Int position)
    {
        if (position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height)
        {
            return DotGrid[position.x, position.y];
        }
        return null;
    }

    public T GetDotAt<T>(Vector2Int position)
    {
        if (position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height)
        {
            if (DotGrid[position.x, position.y] is T t)
                return t;
            else
                return default;
        }
        return default;
    }

    public T GetTileAt<T>(Vector2Int position)
    {
        if (position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height)
        {
            if (TileGrid[position.x, position.y] is T t)
                return t;
            else
                return default;
        }
        return default;
    }
    public List<T> FindObjectsInRow<T>(int row)
    {
        List<T> objects = new();
        if (row < 0 || row >= Height)
        {
            return new();


        }
        for (int col = 0; col < TileGrid.GetLength(0); col++)
        {
            if (TileGrid[col, row] is T tile)
            {
                objects.Add(tile);
            }
            if (DotGrid[col, row] is T dot)
            {
                objects.Add(dot);
            }

        }
        return objects;

    }
    public List<T> FindObjectsInColumn<T>(int col)
    {
        List<T> objects = new();
        if (col < 0 || col >= Width)
        {
            return new();


        }
        for (int row = 0; row < TileGrid.GetLength(1); row++)

        {
            if (TileGrid[col, row] is T tile)
            {
                objects.Add(tile);
            }
            if (DotGrid[col, row] is T dot)
            {
                objects.Add(dot);
            }

        }
        return objects;

    }
    public Dot GetDotAt(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return DotGrid[x, y];
        }
        return null;
    }
    public TileModel GetTileAt(Vector2Int position)
    {
        if (position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height)
        {
            return TileGrid[position.x, position.y];
        }
        return null;
    }
    public TileModel GetTileAt(int x, int y)
    {
        if (x >= 0 && y < Width && y >= 0 && y < Height)
        {
            return TileGrid[x, y];
        }
        return null;
    }






    public override string ToString()
    {
        string str = "";
        foreach (Dot dot in DotGrid)
        {
            if (dot == null) continue;
            str += dot + " \n";

        }
        return str;
    }
    public bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height;
    }

    public Dot GetDot(string id) => _dotsById.TryGetValue(id, out var dot) ? dot : null;
       
    public TileModel GetTile(string id) => _tilesById.TryGetValue(id, out var tile) ? tile : null;

    public void ClearBoard()
    {
        _dotsById?.Clear();
        _tilesById?.Clear();
        DotGrid = new Dot[Width, Height];
        TileGrid = new TileModel[Width, Height];
    }
}

