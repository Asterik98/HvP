using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private RoomListing _roomListing;
    public Text roomText;
    public Text playerText;
    public Text mazeText;
    public List<RoomListing> _listings = new List<RoomListing>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count == 0)
        {
            roomText.text = "Sorry, No Public Room Available";
            playerText.enabled = false;
            mazeText.enabled = false;
        }
        else {
            foreach (RoomInfo info in roomList)
            {
                if (info.RemovedFromList)
                {
                    int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                    if (roomList.Count == 1)
                    {
                        roomText.text = "Sorry, No Public Room Available";
                        playerText.enabled = false;
                        mazeText.enabled = false;
                        break;
                    }
                }
                else
                {
                    roomText.text = "Room Code";
                    playerText.enabled = true;
                    mazeText.enabled = true;
                    int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                    if (index == -1)
                    {
                        RoomListing listing = Instantiate(_roomListing, _content);
                        if (listing != null)
                        {
                            listing.SetRoomInfo(info);
                            _listings.Add(listing);
                        }
                    }
                }
            }
        }
        roomText.enabled = true;
    }
}
