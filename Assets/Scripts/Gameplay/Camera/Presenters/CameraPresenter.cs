using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPresenter : MonoBehaviour
{
    private readonly float _aspectRatio = 0.625f;
    [SerializeField] private float padding = 1;
    private Camera cam;
    private BoardPresenter _board;
    void Start()
    {
        _board = FindFirstObjectByType<BoardPresenter>();
        cam = GetComponent<Camera>();
        cam.backgroundColor = ColorSchemeService.CurrentColorScheme.backgroundColor;
    }   
    private void Update()
    {
        RepositionCamera((_board.Width - 1) * BoardView.TileSize, (_board.Height - 1) * BoardView.TileSize);
    }


    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new(x / 2, y / 2, -1);
        transform.position = tempPosition;
        if (_board.Width >= _board.Height)
        {
            Camera.main.orthographicSize = (x / 2 + padding) / _aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = y / 2 + padding;
        }

    }


}
