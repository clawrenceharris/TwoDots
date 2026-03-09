using UnityEngine;

public class PrefabLibrary : MonoBehaviour
{
    public static PrefabLibrary Instance { get; private set; }
    [SerializeField] private DotView NormalDot;
    [SerializeField] private DotView BlankDot;
    [SerializeField] private DotView AnchorDot;
    [SerializeField] private DotView ClockDot;
    [SerializeField] private BombView Bomb;
    [SerializeField] private DotView NestingDot;
    [SerializeField] private DotView BeetleDot;
    [SerializeField] private DotView LotusDot;



    [SerializeField] private TileView EmptyTile;
    [SerializeField] private TileView Block;
    [SerializeField] private TileView OneSidedBlock;
    [SerializeField] private TileView Ice;
    [SerializeField] private TileView Circuit;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public DotView FromDotType(DotType type)
    {
        switch (type)
        {
            case DotType.Normal: return NormalDot;
            case DotType.Blank: return BlankDot;
            case DotType.Anchor: return AnchorDot;
            case DotType.Clock: return ClockDot;
            case DotType.Bomb: return Bomb;
            case DotType.Nesting: return NestingDot;
            case DotType.Beetle: return BeetleDot;
            case DotType.Lotus: return LotusDot;
            default: return null;
        }
    }
    public TileView FromTileType(TileType type)
    {
        switch (type)
        {
            case TileType.EmptyTile: return EmptyTile;
            case TileType.Block: return Block;
            case TileType.OneSidedBlock: return OneSidedBlock;
            case TileType.Ice: return Ice;
            case TileType.Circuit: return Circuit;
            default: return null;
        }
    }
}
