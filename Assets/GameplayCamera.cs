using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    void Start()
    {
        _mainCamera.orthographic = true;
    }
}
