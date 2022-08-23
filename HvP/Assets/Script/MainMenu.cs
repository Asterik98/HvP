using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public GameObject buttons;
    public Slider slideLobby;
    public Text progressText;
    void Start()
    {
        slideLobby.gameObject.SetActive(false);

    }
    public void EnterLobby()
    {
        StartCoroutine(loadingLobby());
        slideLobby.gameObject.SetActive(true);
        buttons.SetActive(false);
    }
    IEnumerator loadingLobby()
    {
        PhotonNetwork.LoadLevel("ListLobby");
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            slideLobby.value = -PhotonNetwork.LevelLoadingProgress;
            progressText.text = (int)(PhotonNetwork.LevelLoadingProgress * 100)+11 + "%";
            yield return null;
        }
    }
    public void EnterSettings()
    {

    }
    public void Quit()
    {

    }
}
