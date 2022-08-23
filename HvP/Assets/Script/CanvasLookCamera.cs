using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CanvasLookCamera : MonoBehaviourPun
{
    public GameObject obj;
    string currentPlayerNickname;
    private void Start()
    {
        this.transform.Rotate(0, 180, 0);
        foreach (GameObject player in FindObjectsOfType<GameObject>().Where(obj => obj.name == "Player(Clone)")) {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                obj = player;
            }
        }
    }
    private void Update()
    {
        if (obj != null&& !photonView.IsMine)
        {
            Vector3 v = obj.GetComponentInChildren<Camera>().transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(obj.GetComponentInChildren<Camera>().transform.position - v);
            transform.Rotate(0, 180, 0);
            transform.RotateAround(obj.transform.position, v, 0);
        }
    }
}
