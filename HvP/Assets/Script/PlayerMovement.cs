
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Resources = UnityEngine.Resources;
public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    public Vector3 velocity;
    Vector3 realVelocity;
    bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 1f;
    public LayerMask groundMask;
    public FixedJoystick joystick;
    float currentStopwatch = 0f;
    public Text infoText;
    public Text playerText;
    bool playerEnterArena = false;
    public PlayerListingMenu menu;
    public Text playerUsername;
    Animator movement;
    public Camera camera;
    protected Vector3 move;
    protected Vector3 realMove;
    public GameObject finishWall;
    public GameObject finishStation;
    AudioSource audioData;
    void Start()
    {
        audioData = GetComponent<AudioSource>();
        movement = GetComponentInChildren<Animator>();
        menu =GameObject.Find("InGamePanel").GetComponent<PlayerListingMenu>();
        playerText = GameObject.Find("PlayerText").GetComponent<Text>();
        if (photonView.IsMine)
        {
            playerUsername.text = PhotonNetwork.LocalPlayer.NickName;
            infoText = GameObject.Find("InfoText").GetComponent<Text>();
            joystick = GameObject.Find("MoveJoystick").GetComponent<FixedJoystick>();
            camera.enabled=true;
        }
        else
        {
            playerUsername.text = photonView.Owner.NickName;
            camera.enabled = false;
            camera.GetComponent<AudioListener>().enabled = false;
        }
    }
    public void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        if (!photonView.IsMine)
        {
            move = realMove;
            controller.Move(move * speed * Time.fixedDeltaTime);
            velocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(velocity * Time.fixedDeltaTime);
        }
        else
        {
            move = transform.forward * joystick.Vertical + transform.right * joystick.Horizontal;
            controller.Move(move * speed * Time.fixedDeltaTime);
            velocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(velocity * Time.fixedDeltaTime);
        }

        if (move != Vector3.zero)
        {
            ChangeAnimator("Run");
            if(!audioData.isPlaying)
                audioData.Play(0);
        }
        else
        {
            ChangeAnimator("Idle");
            audioData = GetComponent<AudioSource>();
            audioData.Stop();
        }
        if (playerEnterArena == true)
        {
            PlayerStopwatch();
        }
    }

    void ChangeAnimator(string move)
    {
        movement.runtimeAnimatorController = Resources.Load("basicMotions/AnimationControllers/BasicMotions@" +move) as RuntimeAnimatorController;
    }
    void PlayerStopwatch()
    {
        currentStopwatch += Time.deltaTime;
        string minutes = Mathf.Floor(currentStopwatch / 60).ToString("00");
        string seconds = (currentStopwatch % 60).ToString("00");
        string miliseconds = ((currentStopwatch * 10) % 10).ToString("00");
        string tempTimer = string.Format("{0}:{1}:{2}", minutes, seconds, miliseconds);
        infoText.text = tempTimer;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Arena") && !infoText.text.Contains("Finished"))
        {
            finishStation = GameObject.Find("FinishStation(Clone)");
            finishWall = finishStation.transform.GetChild(5).gameObject;
            finishWall.SetActive(false);
            playerText.enabled = false;
            playerEnterArena = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Arena"))
        {
            finishWall.SetActive(true);
            playerEnterArena = false;
            menu.SetPlayerFinishTime(currentStopwatch,photonView.Owner);
            if (photonView.IsMine)
            {
                infoText.text = "Finished";
            }
            
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(move);
        }
        else
        {
            realMove = (Vector3)stream.ReceiveNext();
            //Vector3 oldMove = move;

            //float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            //realMove += ((realMove - oldMove) * lag);
        }
    }
}
