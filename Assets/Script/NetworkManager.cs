using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField nicknameInputField; // �г��� �Է� UI
    public GameObject lobbyUI; // �г��� �Է� UI�� ���Ե� �κ� ȭ��
    public GameObject chatUI; // ä�� UI
    public InputField chatInputField; // ä�� �Է� �ʵ�
    public Text chatDisplay; // ä�� ǥ�� �ؽ�Ʈ

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void OnClickConnect()
    {
        string nickname = nicknameInputField.text;

        // �г����� ������� ��� �⺻ �г��� ����
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = "Player";
        }

        PhotonNetwork.NickName = nickname; // �г��� ����
        PhotonNetwork.ConnectUsingSettings(); // Photon ������ ����

        // �г��� �Է� UI ��Ȱ��ȭ, ä�� UI Ȱ��ȭ
        lobbyUI.SetActive(false);
        chatUI.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null); // �濡 ����
    }

    public override void OnJoinedRoom()
    {
        Spawn();
    }

    public void Spawn()
    {
        // �÷��̾� ����
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