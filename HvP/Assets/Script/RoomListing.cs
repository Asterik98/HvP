
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomListing : MonoBehaviour
{
    public InputField username;
    public Text usernameAlert;
    public StartNewRoomBT mainPanel;
    public Text _roomCodeText;
    public Text _playerText;
    public GameObject alertPanel;
    [SerializeField]
    private Text _mazeSizeText;
    public RoomInfo RoomInfo { get; private set; }
    public Button bt;
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _roomCodeText.text = roomInfo.Name;
        if (roomInfo.PlayerCount == roomInfo.MaxPlayers)
        {
            _playerText.text = "FULL";
            bt.interactable = false;
        }
        else
        {
            _playerText.text = roomInfo.PlayerCount.ToString() + "/" + roomInfo.MaxPlayers.ToString();
            bt.interactable = true;
        }
        _mazeSizeText.text = roomInfo.CustomProperties["mazeSize"].ToString();
    }
    public void OnClick_Button()
    {
        JoinRoomChecker();
    }
    public void JoinRoomChecker()
    {
        username=GameObject.Find("UsernameInputField").GetComponent<InputField>();
        usernameAlert = GameObject.Find("UsernameAlert").GetComponent<Text>();
        mainPanel = gameObject.transform.root.Find("MainPanel").GetComponent<StartNewRoomBT>();
        if (username.text.Equals(""))
        {
            usernameAlert.enabled = true;
        }
        else
        {
            mainPanel.GetComponent<StartNewRoomBT>().JoinRoom(RoomInfo.Name);
        }
    }
}
