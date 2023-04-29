using DG.Tweening;
using UnityEngine;


public class Camera4ProjController : MonoBehaviour
{
    public Transform Pivot;
    public float RotationDuration;
    public Camera[] Cameras;
    public GameObject[] Axis;


    private Tweener _rotationTweener;
    private Quaternion _rotation;
    private int AxisMode = 0;


    void Awake()
    {
        _rotation = Pivot.rotation;
    }

#if UNITY_EDITOR
    void Update()
    {
        if(IsStillRotating())
            return;

        // Rotation
        {
            var prevRotation = _rotation;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Y))
                _rotation *= Quaternion.Euler(0, 90, 0);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                _rotation *= Quaternion.Euler(0, -90, 0);
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.X))
                _rotation *= Quaternion.Euler(90, 0, 0);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                _rotation *= Quaternion.Euler(-90, 0, 0);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                _rotation *= Quaternion.Euler(-90, 0, 0);
            else if (Input.GetKeyDown(KeyCode.Z))
                _rotation *= Quaternion.Euler(0, 0, 90);
            if (!_rotation.Equals(prevRotation))
                RotateCamera(_rotation, RotationDuration);
        }

        // Zoom
        {
            if (Input.GetKeyDown(KeyCode.Equals))
                foreach (var cam in Cameras)
                    cam.orthographicSize *= 2f;

            if (Input.GetKeyDown(KeyCode.Minus))
                foreach (var cam in Cameras)
                    cam.orthographicSize *= 0.5f;
        }

        // Hide axis
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                AxisMode = (AxisMode + 1) % 4;
                if (AxisMode == 0) // Enable all
                    foreach (var axis in Axis)
                        axis.SetActive(true);
                else if (AxisMode == 1) // Enable only first
                {
                    Axis[0].SetActive(true);
                    Axis[1].SetActive(false);
                }
                else if (AxisMode == 2) // Enable only second
                {
                    Axis[0].SetActive(false);
                    Axis[1].SetActive(true);
                }
                if (AxisMode == 3) // Disable all
                    foreach (var axis in Axis)
                        axis.SetActive(false);
            }
        }
    }
#endif

    public void RotateCamera(Quaternion newAngles, float rotationTime, Ease easeRotation = Ease.InOutQuad)
    {
        _rotationTweener = Pivot.DORotateQuaternion(newAngles, rotationTime).SetEase(easeRotation);
    }

    public bool IsStillRotating()
    {
        if (_rotationTweener == null)
            return false;
        if (!_rotationTweener.IsActive())
            return false;
        return _rotationTweener.IsPlaying();
    }

    public void StopRotation()
    {
        _rotationTweener.Kill();
        _rotationTweener = null;
    }
}
