using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MenuScript : MonoBehaviourPunCallbacks
{
    public GameObject menuPanel;
    public GameObject move;
    public GameObject look;
    public GameObject leaveButton;
    public GameObject PosPanel;
    public GameObject loadPanel;
    public GameObject menu;
    public Text totalPlayer;
    public Text currentPlayer;
    public Text progressText;
    public Text loadText;
    public Text infoText;
    public TMP_Text wallText;
    public Slider slider;
    public GameObject startWall;
    public Animator startWallAnim;
    public GameObject StartBt;
    float currentTimer;
    bool startBtActive = false;
    private void Start()
    {
        if (!FindObjectOfType<AudioManager>().CheckSound("Ambient"))
            FindObjectOfType<AudioManager>().Play("Ambient");
        currentTimer = 4f;
        StartBt.SetActive(false);
        leaveButton.SetActive(true);
        menuPanel.SetActive(true);
        move.SetActive(true);
        look.SetActive(true);
        totalPlayer.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        currentPlayer.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        CheckTotalPlayer();
    }
    private void Update()
    {
        PhotonView photonView = PhotonView.Get(this);
        if (startBtActive == true)
        {
            CountdownTimer();
        }
    }
    void CountdownTimer()
    {
        StartBt.SetActive(false);
        currentTimer -= Time.deltaTime;
        if (currentTimer >= 2f)
        {
            string tempTimer = string.Format("{0:00}", currentTimer - 1);
            infoText.text = tempTimer;
        }
        if (currentTimer < 2f && currentTimer > 0f)
        {
            infoText.text = "READY";
        }
        if (currentTimer < 1f)
        {
            infoText.text = "GO";
            startBtActive = false;
            infoText.text = "Finish The Maze";
            startWallAnim.SetBool("Start", true);
            PosPanel.SetActive(false);
        }
        wallText.text = infoText.text;
    }
   
    [PunRPC]
    void SetTimer(float timer)
    {
        currentTimer = timer;
    }
    [PunRPC]
    void SetStart(bool start)
    {
        startBtActive = start;
    }
    public void FindWall()
    {
        startWallAnim = GameObject.Find("StartWall").GetComponent<Animator>();
        startWall = GameObject.Find("StartWall");
        wallText = startWall.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
        if (PhotonNetwork.IsMasterClient)
        {
            CheckTotalPlayer();
        }
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        loadPanel.SetActive(true);
        menu.SetActive(false);
    }
    public override void OnJoinedLobby()
    {
        StartCoroutine(Leave());
    }
    IEnumerator Leave()
    {
        PhotonNetwork.LoadLevel("ListLobby");
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            slider.value = -PhotonNetwork.LevelLoadingProgress;
            progressText.text = (int)(PhotonNetwork.LevelLoadingProgress * 100) + 11 + "%";
            yield return null;
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        StartCoroutine(MainReconnect());
    }
    private IEnumerator MainReconnect()
    {
        while (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
        {
            Debug.Log("Waiting for client to be fully disconnected..", this);

            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("Client is disconnected!", this);

        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            if (PhotonNetwork.Reconnect())
            {
                Debug.Log("Successful reconnected!", this);
            }
        }
        else
        {
            Debug.Log("Successful reconnected and joined!", this);
        }
    }
    public void StartButtonCommand()
    {
        startBtActive = true;
        photonView.RPC("SetStart", RpcTarget.Others, startBtActive);
    }
    void ResetTimer()
    {
        currentTimer = 4f;
        startBtActive = false;
        infoText.text = "Waiting For Players";
        wallText.text = infoText.text;
    }
    void CheckTotalPlayer()
    {
        if (PhotonNetwork.LocalPlayer.IsLocal && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartBt.SetActive(true);
                ResetTimer();
                photonView.RPC("SetTimer", RpcTarget.Others, currentTimer);
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        currentPlayer.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        CheckTotalPlayer();
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        currentPlayer.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        if (PhotonNetwork.IsMasterClient)
        {
            ResetTimer();
        }
    }
}