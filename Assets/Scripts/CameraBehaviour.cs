using UnityEngine;

public class CameraBehaviour : MonoBehaviour {
    [SerializeField] private Transform mainCamera;
    public float speed;
    private Transform _rotator;
    private Vector3 cameraStartPosition = new Vector3(0, 6, -10);

    private void Start() {
        _rotator = GetComponent<Transform>();
        SetCameraPos(cameraStartPosition);
    }

    private void Update() {
        _rotator.Rotate(0, speed * Time.deltaTime, 0);
    }

    private void SetCameraPos(Vector3 pos) {
        mainCamera.position = pos;
    }

    public void ResetCameraPos() {
        SetCameraPos(cameraStartPosition);
    }

    public void ChangeCameraDistance(float step) {
        float distanceY = cameraStartPosition.y;
        float distanceZ = cameraStartPosition.z;
        SetCameraPos(new Vector3(0, distanceY, distanceZ));
    }
}
