using Photon.Pun;
using UnityEngine;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public void OnCreateRoom_UnityEditor()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions{MaxPlayers = 2});
    }

    public void OnJoinRoom_UnityEditor()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnQuitRoom_UnityEditor()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE && !UNITY_EDITOR
        Application.Quit();
#endif
    }


    void Start()
    {
#if UNITY_EDITOR
        PhotonNetwork.NickName = "1";
#elif UNITY_STANDALONE && !UNITY_EDITOR
        PhotonNetwork.NickName = "2";
#endif
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debugger.Log("Ready for connecting!");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("NetGameScene");
        Debugger.Log("Connected !");
    }
}