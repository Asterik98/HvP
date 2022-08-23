using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerListing : MonoBehaviour
{

    [SerializeField]
    private Text name;
    [SerializeField]
    private Text _timeText;

    [SerializeField]
    private Text _position;

    public float finishTime;
    public int position;
    private Animator posAnim;
    public Player Player { get; private set; }
    public void SetPlayerInfo(int pos,Player player)
    {
        Player = player;
        position = pos;
        name.text = Player.NickName;
        if (position == 0)
        {
            _position.text = "C";
        }
        else
        {
            _position.text = position.ToString();
        }
    }
    public void SetPlayerTime(float time)
    {
        finishTime = time;
        string minutes = Mathf.Floor(finishTime / 60).ToString("00");
        string seconds = (finishTime % 60).ToString("00");
        string miliseconds = ((finishTime * 10) % 10).ToString("00");
        string tempTimer = string.Format("{0}:{1}:{2}", minutes, seconds, miliseconds);
        _timeText.text= tempTimer;
        GetComponent<Animator>().SetBool("Finish",true);
    }
} 
