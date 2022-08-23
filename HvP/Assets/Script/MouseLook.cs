using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLook : MonoBehaviourPunCallbacks
{
    public float mouseSensitivity = 1f;
    public Transform playerBody;
    float xRotation = 0f;
    public Transform cam;
    public Vector2 LookAxis;
    public FixedJoystick joystick;
    //protected Quaternion look = Quaternion.identity;
   // protected Quaternion realLook = Quaternion.identity;
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            cam.GetComponent<AudioListener>().enabled = false;
            cam.GetComponent<Camera>().enabled = false;

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            joystick = GameObject.Find("LookJoystick").GetComponent<FixedJoystick>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Look();
        }
        else
        {
           // transform.rotation = realLook;
        }
    }
    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(look);
        }
        else
        {
            realLook = (Quaternion)stream.ReceiveNext();
        }
    }*/
    void Look()
    {
        float mouseX = joystick.Horizontal/1.75f * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); 
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
