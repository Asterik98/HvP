using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public struct Wall
{
    public WallState[,] wallState;
    public int endX;
    public int endY;
    public int startX;
    public int startY;

}
public class MazeRenderer :MonoBehaviourPunCallbacks
{
    [Range(1, 50)]
    private int width=10;
    [Range(1,50)]
    private int height = 10;

    [SerializeField]
    private float size = 1f;

    [SerializeField]
    private Transform wallPrefab=null;
    [SerializeField]
    private Transform floorPrefab = null;
    public GameObject StartStation;
    public GameObject FinishStation;
    public MenuScript menu;
    public GameObject playerPrefab = null;
    public GameObject[] spawnPosition;
    public float[] wallRotX;
    public float[] wallRotY;
    public float[] wallRotZ;
    public float[] wallPosX;
    public float[] wallPosY;
    public float[] wallPosZ;
    public float[] wallRotW;
    public float[] wallScaleX;
    public float[] wallScaleY;
    public float[] wallScaleZ;
    int spawn = 0;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            width = int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["mazeSize"].ToString().Substring(0, 2));
            height = width;
            Wall maze = MazeGenerator.Generate(10, 10);
            Draw(maze);
            GameObject finish = GameObject.Find("FinishStation(Clone)");
            int i = gameObject.transform.childCount;
            int a = 0;
            wallRotX = new float[i + 2];
            wallRotY = new float[i + 2];
            wallRotZ = new float[i + 2];
            wallRotW = new float[i + 2];
            wallPosX = new float[i + 2];
            wallPosY = new float[i + 2];
            wallPosZ = new float[i + 2];
            wallScaleX = new float[i + 2];
            wallScaleY = new float[i + 2];
            wallScaleZ = new float[i + 2];
            foreach (Transform child in transform)
            {
                wallRotX[a] = child.rotation.x;
                wallRotY[a] = child.rotation.y;
                wallRotZ[a] = child.rotation.z;
                wallRotW[a] = child.rotation.w;
                wallPosX[a] = child.position.x;
                wallPosY[a] = child.position.y;
                wallPosZ[a] = child.position.z;
                wallScaleX[a] = child.localScale.x;
                wallScaleY[a] = child.localScale.y;
                wallScaleZ[a] = child.localScale.z;
                a++;
            }
            wallRotX[a] = StartStation.transform.rotation.x;
            wallRotY[a] = StartStation.transform.rotation.y;
            wallRotZ[a] = StartStation.transform.rotation.z;
            wallRotW[a] = StartStation.transform.rotation.w;
            wallPosX[a] = StartStation.transform.position.x;
            wallPosY[a] = StartStation.transform.position.y;
            wallPosZ[a] = StartStation.transform.position.z;
            wallScaleX[a] = StartStation.transform.localScale.x;
            wallScaleY[a] = StartStation.transform.localScale.y;
            wallScaleZ[a] = StartStation.transform.localScale.z;
            a++;
            wallRotX[a] = finish.transform.rotation.x;
            wallRotY[a] = finish.transform.rotation.y;
            wallRotZ[a] = finish.transform.rotation.z;
            wallRotW[a] = finish.transform.rotation.w;
            wallPosX[a] = finish.transform.position.x;
            wallPosY[a] = finish.transform.position.y;
            wallPosZ[a] = finish.transform.position.z;
            wallScaleX[a] = finish.transform.localScale.x;
            wallScaleY[a] = finish.transform.localScale.y;
            wallScaleZ[a] = finish.transform.localScale.z;
        }
        photonView.RPC("GenerateMaze", RpcTarget.OthersBuffered, wallPosX, wallPosY, wallPosZ, wallRotX, wallRotY, wallRotZ, wallRotW, wallScaleX, wallScaleY, wallScaleZ);
        InstantiatePlayer();
    }

    private Vector3 setPos()
    {
        Vector3 pos;
        spawnPosition = GameObject.FindGameObjectsWithTag("SpawnPoint");
        spawn = Random.Range(0, spawnPosition.Length - 1);
        pos = spawnPosition[spawn].transform.position;
        spawnPosition[spawn].SetActive(false);
        return pos;
    }
    private void InstantiatePlayer()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, setPos(), Quaternion.Euler(0, 0, 0));
        menu.FindWall();
    }
    [PunRPC]
    void GenerateMaze(float[] wallPosX, float[] wallPosY, float[] wallPosZ, float[] wallRotX, float[] wallRotY, float[] wallRotZ, float[] wallRotW, float[] wallScaleX, float[] wallScaleY, float[] wallScaleZ)
    {
        for (int i=0;i<wallPosX.Length; i++) {
            Vector3 pos = new Vector3(wallPosX[i],wallPosY[i],wallPosZ[i]);
            Quaternion rot = new Quaternion(wallRotX[i],wallRotY[i],wallRotZ[i],wallRotW[i]);
            if (i == 0)
            {
                var wall = Instantiate(floorPrefab, pos, rot);
                wall.transform.parent = gameObject.transform;
                wall.transform.localScale = new Vector3(wallScaleX[i], wallScaleY[i], wallScaleZ[i]);
            }
            else if(wallPosX.Length-i>2)
            {
                var wall = Instantiate(wallPrefab, pos, rot);
                wall.transform.parent = gameObject.transform;
                wall.transform.localScale = new Vector3(wallScaleX[i], wallScaleY[i], wallScaleZ[i]);
            }
            else if (wallPosX.Length - i == 2)
            {
                StartStation.transform.rotation = new Quaternion(wallRotX[i], wallRotY[i], wallRotZ[i], wallRotW[i]);
                StartStation.transform.position = new Vector3(wallPosX[i], wallPosY[i], wallPosZ[i]);
                StartStation.transform.localScale = new Vector3(wallScaleX[i], wallScaleY[i], wallScaleZ[i]);
            }
            else
            {
                var finish = Instantiate(FinishStation, pos, rot);
                finish.transform.rotation = new Quaternion(wallRotX[i], wallRotY[i], wallRotZ[i], wallRotW[i]);
                finish.transform.position = new Vector3(wallPosX[i], wallPosY[i], wallPosZ[i]);
                finish.transform.localScale = new Vector3(wallScaleX[i], wallScaleY[i], wallScaleZ[i]);
            }
        }
    }
    private void Draw(Wall maze)
    {
        var floor = Instantiate(floorPrefab, transform);
        floor.localScale = new Vector3(width/10f, 0.1f, height/10f);
        floor.localPosition = new Vector3(0, 0.01f, 0);
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze.wallState[i, j];
                var position = new Vector3(-width / 2 + i+0.5f, 0, (-height / 2 + j) + 0.5f);
                if (i == maze.endX && j == maze.endY)
                {
                    Quaternion rot=Quaternion.identity;
                    Vector3 pos=Vector3.zero;
                    if (!cell.HasFlag(WallState.UP)&& j==height-1)
                    {
                        rot = Quaternion.Euler(0, 180, 0);
                        pos = position + new Vector3(0, 0, -size / 2 - 6.4f);
                    }

                    else if (!cell.HasFlag(WallState.LEFT)&&i==0)
                    {
                        rot = Quaternion.Euler(0, 90, 0);
                        pos = position+ new Vector3(size / 2+ 6.4f, 0, 0);
                    }

                    else if(i == width - 1)
                    {
                        if (!cell.HasFlag(WallState.RIGHT)&&i==width-1)
                        {
                            rot = Quaternion.Euler(0, -90, 0);
                            pos = position + new Vector3(-size / 2- 6.4f, 0, 0);
                        }
                    }
                    var finishStation = Instantiate(FinishStation,pos, rot)as GameObject;
                }
                else if (i == maze.startX && j == maze.startY)
                {
                    Quaternion rot = Quaternion.Euler(0, 0, 0);
                    Vector3 pos = position + new Vector3(0, 0, size/2+6.4f);
                    StartStation.transform.rotation = rot;
                    StartStation.transform.position = pos;
                }
                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform) as Transform;
                    topWall.position = position + new Vector3(0, 0.5f, size / 2);
                    topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position = position + new Vector3(-size / 2, 0.5f, 0);
                    leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector3(size / 2, 0.5f, 0);
                        rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.position = position + new Vector3(0, 0.5f, -size / 2);
                        bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
      
            }

        }
    }
    
}
