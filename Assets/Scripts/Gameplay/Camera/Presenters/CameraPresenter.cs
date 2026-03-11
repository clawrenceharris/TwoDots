using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPresenter : MonoBehaviour
{
    private readonly float _aspectRatio = 0.625f;
    [SerializeField] private float padding = 1;
    private Camera cam;
    void Awake()
    {
        cam = GetComponent<Camera>();
    }
    

   
   


    public void SetUpCameraForLevel(LevelData level)
    {
        cam.backgroundColor = ServiceProvider.Instance.GetService<ColorSchemeService>().CurrentColorScheme.backgroundColor;
       
        float x = (level.width - 1) * BoardView.TileSize;
        float y = (level.height - 1) * BoardView.TileSize;
        Vector3 tempPosition = new(x / 2, y / 2, -1);

        transform.position = tempPosition;
        if (level.width >= level.height)
        {
            cam.orthographicSize = (x / 2 + padding) / _aspectRatio;
        }
        else
        {
            cam.orthographicSize = y / 2 + padding;
        }

    }


}
