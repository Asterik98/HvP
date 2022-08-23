
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private PlayerListing _localPlayer;
    [SerializeField]
    private PlayerListing _playerListing;

    private List<PlayerListing> _listings = new List<PlayerListing>();
    private bool playerFinished = false;
    public void Start()
    {
        GetCurrentRoomPlayers();
        _localPlayer.SetPlayerInfo(0, PhotonNetwork.LocalPlayer);
    }

    void GetCurrentRoomPlayers()
    {
        foreach (KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            if (playerInfo.Value != PhotonNetwork.LocalPlayer)
            {
                AddPlayerListing(playerInfo.Value);
            }
        }
    }
    void AddPlayerListing(Player player)
    {
        PlayerListing listing = Instantiate(_playerListing, _content);
        if(listing != null)
        {
            listing.SetPlayerInfo(_listings.Count+1,player);
            _listings.Add(listing);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player==otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }
    public void SetPlayerFinishTime(float time, Player p) {
        if (PhotonNetwork.LocalPlayer.NickName.Equals(p.NickName))
        {
            _localPlayer.SetPlayerTime(time);
        }
        else
        {
            Debug.Log("b "+_listings.Count);
            int index = _listings.FindIndex(c => c.Player == p);
            Debug.Log("b " + index +" "+ p.NickName);
            _listings[index].SetPlayerTime(time);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.Player.NickName == info.Name);
                if (index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
            }
        }
    }
}
