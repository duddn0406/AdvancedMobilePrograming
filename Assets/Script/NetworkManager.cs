using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField nicknameInputField; // 닉네임 입력 UI
    public GameObject lobbyUI; // 닉네임 입력 UI가 포함된 로비 화면
    public GameObject chatUI; // 채팅 UI
    public InputField chatInputField; // 채팅 입력 필드
    public Text chatDisplay; // 채팅 표시 텍스트

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void OnClickConnect()
    {
        string nickname = nicknameInputField.text;

        // 닉네임이 비어있을 경우 기본 닉네임 설정
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = "Player";
        }

        PhotonNetwork.NickName = nickname; // 닉네임 설정
        PhotonNetwork.ConnectUsingSettings(); // Photon 서버에 연결

        // 닉네임 입력 UI 비활성화, 채팅 UI 활성화
        lobbyUI.SetActive(false);
        chatUI.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null); // 방에 입장
    }

    public override void OnJoinedRoom()
    {
        Spawn();
    }

    public void Spawn()
    {
        // 플레이어 스폰
        PhotonNetwork.Instantiate("Player", new Vector3(-0.8f, -2f, 0), Quaternion.identity);
    }

    public void SendChatMessage()
    {
        if (!string.IsNullOrEmpty(chatInputField.text))
        {
            photonView.RPC("DisplayChatMessage", RpcTarget.All, PhotonNetwork.NickName + ": " + chatInputField.text);
            chatInputField.text = "";
        }
    }

    [PunRPC]
    public void DisplayChatMessage(string message)
    {
        chatDisplay.text += message + "\n";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && chatInputField.text != "")
        {
            SendChatMessage();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
}