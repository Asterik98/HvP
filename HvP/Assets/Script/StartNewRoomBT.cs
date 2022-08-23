using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class StartNewRoomBT : MonoBehaviourPunCallbacks
{
    const string allchar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public Text titlePanel;
    public Button JoinButton;
    public Button EnterButton;
    public Button CreateButton;
    public Button GenerateCodeRoomButton;
    public GameObject JoinPanel;
    public GameObject EnterPanel;
    public GameObject CreatePanel;
    public GameObject loadingIcon;
    public InputField username;
    public InputField CreateRoomCode;
    public Slider MaxPlayerRoom;
    public InputField EnterRoomCode;
    public Dropdown MazeSize;
    public Text usernameAlert;
    public Text CodeRoomAlert;
    public Text EnterRoomAlert;
    public Text roomText;
    public Text playerText;
    public Text mazeText;
    public Text progressText;
    public Text MaxPlayerText;
    public Slider slider;
    public GameObject loadPanel;
    bool duplicate;
    public Text loadText;
    public RoomListingMenu listRoom;
    public Toggle privateCheckbox;
    void Start()
    {
        roomText.enabled = false;
        playerText.enabled = false;
        mazeText.enabled = false;
        JoinPanel.SetActive(true);
        JoinButton.interactable = false;
        titlePanel.text = "JOIN ROOM";
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Not Connected");
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
        if (!MasterManager.GameSettings.NickName.Equals(""))
        {
            username.text = MasterManager.GameSettings.NickName;
        }
        if (PhotonNetwork.InLobby == true)
        {
            loadingIcon.SetActive(false);
        }
        CodeRoomAlert.enabled = false;
        EnterRoomAlert.enabled = false;
        usernameAlert.enabled = false;
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        loadingIcon.SetActive(false);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.LeaveLobby();
    }

    public void CreateRoomChecker()
    {
        if (username.text.Equals(""))
        {
            usernameAlert.text = "Fill the Username";
            usernameAlert.enabled = true;
            CreateButton.interactable = true;
        }
        else
        {
            usernameAlert.enabled = false;
        }

        if (CreateRoomCode.text.Equals(""))
        {
            CodeRoomAlert.text = "Room Code Cannot Blank";
            CodeRoomAlert.enabled = true;
            CreateButton.interactable = true;
        }
        else
        {
            CodeRoomAlert.enabled = false;
        }

        if (usernameAlert.enabled == false && CodeRoomAlert.enabled == false)
        {
            duplicate = CheckDuplicateRoomCode(CreateRoomCode.text);
            if (duplicate)
            {
                CodeRoomAlert.text = "Room Code has been used, please generate / use another code";
                CodeRoomAlert.enabled = true;
                CreateButton.interactable = true;
            }
            else
            {
                CodeRoomAlert.enabled = false;
                CreateRoom((int)MaxPlayerRoom.value);
            }
        }
    }
    public void JoinRoomChecker()
    {
        if (username.text.Equals(""))
        {
            usernameAlert.enabled = true;
        }
        else
        {
            usernameAlert.enabled = false;
        }
        if (EnterRoomCode.text.Equals(""))
        {
            EnterRoomAlert.enabled = true;
            EnterRoomAlert.text = "Room Code Cannot Blank";
        }
        else
        {
            EnterRoomAlert.enabled = false;
        }
        if (usernameAlert.enabled == false && EnterRoomAlert.enabled == false)
        {
            JoinRoom(EnterRoomCode.text);
        }
    }
    public void CreateRoom(int maxPlayer)
    {
        bool privateRoom= privateCheckbox.isOn;
        var roomProperties = new Hashtable();
        var lobbyproperties = new string[1];
        roomProperties.Add("mazeSize", MazeSize.options[MazeSize.value].text);
        lobbyproperties[0] = "mazeSize";
        PhotonNetwork.LeaveLobby();
        loadPanel.SetActive(true);
        loadText.text = "Creating Room";
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayer,
            CustomRoomProperties = roomProperties,
            CustomRoomPropertiesForLobby = lobbyproperties,
            IsVisible = !privateRoom
        };
        PhotonNetwork.CreateRoom(CreateRoomCode.text, options, TypedLobby.Default);
        PhotonNetwork.LocalPlayer.NickName = username.text;
    }
    IEnumerator moveScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.LoadLevel("MainGame");
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            slider.value = -PhotonNetwork.LevelLoadingProgress;
            progressText.text = (int)(PhotonNetwork.LevelLoadingProgress * 100) + 11 + "%";
            yield return null;
        }
        PhotonNetwork.IsMessageQueueRunning = true;
    }
    public void JoinRoom(string roomcode)
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomcode);
        PhotonNetwork.LocalPlayer.NickName = username.text;
        loadPanel.SetActive(true);
        loadText.text = "Joining Room...";
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(moveScene());
        }
        // PhotonNetwork.IsMessageQueueRunning = false;
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        EnterRoomAlert.enabled = true;
        EnterRoomAlert.text = "Room Code Not Found";
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CodeRoomAlert.enabled = true;
        CodeRoomAlert.text = "Room has been created";
    }
    public void JoinPanelBtn()
    {
        titlePanel.text = "JOIN ROOM";
        JoinButton.interactable = false;
        CreateButton.interactable = true;
        EnterButton.interactable = true;
        JoinPanel.SetActive(true);
        CreatePanel.SetActive(false);
        EnterPanel.SetActive(false);
    }
    public void CreatePanelBtn()
    {
        GenerateCodeRoom();
        titlePanel.text = "CREATE ROOM";
        JoinButton.interactable = true;
        CreateButton.interactable = false;
        EnterButton.interactable = true;
        JoinPanel.SetActive(false);
        CreatePanel.SetActive(true);
        EnterPanel.SetActive(false);
    }
    public void EnterPanelBtn()
    {
        titlePanel.text = "ENTER ROOM";
        JoinButton.interactable = true;
        CreateButton.interactable = true;
        EnterButton.interactable = false;
        JoinPanel.SetActive(false);
        CreatePanel.SetActive(false);
        EnterPanel.SetActive(true);
    }
    public void SaveUsername()
    {
        MasterManager.GameSettings.NickName = username.text;
        usernameAlert.text = "Saved";
        usernameAlert.enabled = true;
    }
    public void GenerateCodeRoom()
    {
        string code = "";
        duplicate = true;
        while (duplicate)
        {
            for (int i = 0; i < 6; i++)
            {
                code += allchar[Random.Range(0, allchar.Length)];
            }
            duplicate=CheckDuplicateRoomCode(code);
        }
        CreateRoomCode.text = code;
    }
   public bool CheckDuplicateRoomCode(string code)
    {
        if (listRoom._listings.Count != 0)
        {
            foreach (RoomListing room in listRoom._listings)
            {
                if (!room._roomCodeText.text.Equals(code))
                {
                    duplicate = false;
                    break;
                }
                else
                {
                    duplicate = true;
                }
            }
        }
        else
        {
            duplicate = false;
        }
        return duplicate;
    }
    public void MaxPlayerinfo()
    {
        MaxPlayerText.text=MaxPlayerRoom.value.ToString();
    }
}
