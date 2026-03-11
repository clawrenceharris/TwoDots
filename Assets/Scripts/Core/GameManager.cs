
using DG.Tweening;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    [SerializeField] private World[] worlds;
    public static GameManager Instance { get; private set; }
    
    public World[] Worlds
    {
        get
        {
            return worlds;
        }
    }

    [SerializeField] private int worldIndex;
    public int WorldIndex { get { return worldIndex; }}
    public static int TotalAmountOfLevels { get; private set; }

    private void Awake()
    {

        if (Instance == null)
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Destroy(gameObject);
        }
        SetTotalAmountOfLevels();
        DOTween.SetTweensCapacity(900, 50);
       
        
        
    }

    private void SetTotalAmountOfLevels()
    {
        int total = 0;
        foreach (World world in worlds)
        {
            total += world.levels.Length;
        }
        TotalAmountOfLevels = total;
    }

   








}
