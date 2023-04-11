using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {
    [SerializeField] CameraBehaviour CameraBehaviourScript;
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f;
    public Transform cubeToPlace;
    public GameObject cubeToCreate, allCubes;
    public GameObject restartButton;
    public GameObject[] uiElements;
    public Rigidbody allCubesRb;
    private List<Vector3> allCubesPositions = new List<Vector3> {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0), // main cube
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(-1, 0, -1),
    };
    private bool IsLose, firstCube;
    private Coroutine showCubePlace;

    // Start is called before the first frame update
    private void Start() {
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update() {
        if (!IsLose) {
            if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null) {
                // click on UI element
                if (EventSystem.current.IsPointerOverGameObject()) {
                    return;
                }
                if (!firstCube) {
                    firstCube = true;
                    foreach (GameObject uiGameObject in uiElements) {
                        Destroy(uiGameObject);
                    }
                }
                InsertNewCube();
                SpawnPositions();
                CheckCameraDistance();
            }

            // tower start moving
            if (allCubesRb.velocity.magnitude > 0.1f) {
                GoToLose();
            }
        }
    }

    IEnumerator ShowCubePlace() {
        while (true) {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }

    private void SpawnPositions() {
        List<Vector3> availablePositions = new List<Vector3>();
        List<Vector3> positionsToCheck = new List<Vector3>();
        positionsToCheck.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        positionsToCheck.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        positionsToCheck.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        positionsToCheck.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        positionsToCheck.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        positionsToCheck.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));

        foreach (Vector3 position in positionsToCheck) {
            if (IsPositionEmpty(position) && !IsPositionTheSame(cubeToPlace.position, position)) {
                availablePositions.Add(position);
            }
        }

        cubeToPlace.position = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];
    }

    private void InsertNewCube() {
        #if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began) {
                return;
            }
        #endif

        GameObject newCube = Instantiate(cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
        newCube.transform.SetParent(allCubes.transform);
        nowCube.setVector(newCube.transform.position);
        allCubesPositions.Add(nowCube.getVector());

        // refresh physics
        allCubesRb.isKinematic = true;
        allCubesRb.isKinematic = false;
    }

    private void CheckCameraDistance() {
        float maxX = 0, maxY = 0, maxZ = 0;

        foreach (Vector3 cubePos in allCubesPositions) {
            maxX = Math.Max(maxX, cubePos.x);
            maxY = Math.Max(maxY, cubePos.y);
            maxZ = Math.Max(maxZ, cubePos.z);
        }

        if (maxX > 3 || maxZ > 3) {
            CameraBehaviourScript.ChangeCameraDistance(Math.Max(maxX, maxZ));
        }

        Debug.Log("maxX: " + maxX + ", maxY: " + maxY+ ", maxZ: " + maxZ);
    }

    private void GoToLose() {
        Destroy(cubeToPlace.gameObject);
        IsLose = true;
        StopCoroutine(showCubePlace);
        restartButton.SetActive(true);
        CameraBehaviourScript.ResetCameraPos();
    }

    private bool IsPositionEmpty(Vector3 position) {
        if (position.y == 0) { // ground position
            return false;
        }

        foreach (Vector3 occupiedPos in allCubesPositions) {
            if (IsPositionTheSame(occupiedPos, position)) {
                return false;
            }
        }

        return true;
    }

    private bool IsPositionTheSame(Vector3 position1, Vector3 position2) {
        return (
            position1.x == position2.x &&
            position1.y == position2.y &&
            position1.z == position2.z
        );
    }
}

struct CubePos {
    public int x, y, z;

    public CubePos(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector() {
        return new Vector3(x, y, z);
    }

    public void setVector(Vector3 pos) {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}