using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // UI ��ҵ�
    public InputField nicknameInputField;
    public GameObject lobbyUI;
    public GameObject chatUI;
    public GameObject chatRequestUI;
    public GameObject confirmRequestUI;
    public InputField chatInputField;
    public GameObject privateUI;
    public Text chatDisplay;
    public Text requestText;
    public Text confirmText;

    private string selectedPlayer; // ���õ� �÷��̾� �г���
    private string privateRoomName; // 1��1 ��ȭ�� �̸�
    private bool isJoiningPrivateRoom = false; // 1��1 �� �̵� ����

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void OnClickConnect()
    {
        string nickname = nicknameInputField.text;

        if (string.IsNullOrEmpty(nickname))
        {
            nickname = "Player";
        }

        PhotonNetwork.NickName = nickname;
        PhotonNetwork.ConnectUsingSettings();

        lobbyUI.SetActive(false);
        chatUI.SetActive(true);
    }



    public override void OnConnectedToMaster()
    {
        // �� ���� ���� ó��
        if (isJoiningPrivateRoom)
        {
            Debug.Log($"Joining private room: {privateRoomName}");
            privateUI.SetActive(true);
            chatUI.SetActive(true);
            PhotonNetwork.JoinOrCreateRoom(privateRoomName, new RoomOptions { MaxPlayers = 2 }, null);
        }
        else
        {
            Debug.Log("Joining default room...");
            privateUI.SetActive(false);
            chatUI.SetActive(true);
            PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 10 }, null);
        }
    }

    public override void OnJoinedRoom()
    {
        chatDisplay.text = "";
        chatDisplay.text += $"{PhotonNetwork.NickName}���� �濡 �����߽��ϴ�.\n";

        SpawnPlayer(); // �÷��̾� ����
    }

    private void SpawnPlayer()
    {
        // �÷��̾� ������Ʈ ����
        if (isJoiningPrivateRoom)
        {
            PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-5f, 5f), Random.Range(-2f, 5f), 0), Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-32f, -25f), Random.Range(-5f, 5f), 0), Quaternion.identity);
           
        }
       
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

    public void OnPlayerTouched(GameObject targetPlayer)
    {
        if (targetPlayer.TryGetComponent<PhotonView>(out PhotonView targetPhotonView))
        {
            selectedPlayer = targetPhotonView.Owner.NickName; // ���õ� �÷��̾� �г���
            confirmRequestUI.SetActive(true);
            confirmText.text = $"{selectedPlayer}�԰� 1��1 ��ȭ�� �Ͻðڽ��ϱ�?";
        }
    }

    public void DeclineConfirm()
    {
        confirmRequestUI.SetActive(false);
    }

    public void ConfirmPrivateChat()
    {
        confirmRequestUI.SetActive(false);

        // �� �̸� ����
        privateRoomName = $"{PhotonNetwork.NickName}-{selectedPlayer}-{System.Guid.NewGuid()}";

        Photon.Realtime.Player targetPlayer = PhotonNetwork.PlayerListOthers
            .FirstOrDefault(p => p.NickName == selectedPlayer);

        if (targetPlayer != null)
        {
            isJoiningPrivateRoom = true; // 1��1 �� �̵� �÷��� Ȱ��ȭ
            photonView.RPC("SendChatRequest", targetPlayer, PhotonNetwork.NickName, selectedPlayer, privateRoomName);
            PhotonNetwork.LeaveRoom(); // ���� �� ������
        }
    }

    [PunRPC]
    public void SendChatRequest(string sender, string target, string roomName)
    {
        if (PhotonNetwork.NickName == target)
        {
            chatRequestUI.SetActive(true);
            requestText.text = $"{sender}���� 1��1 ä���� ��û�߽��ϴ�. �����Ͻðڽ��ϱ�?";
            privateRoomName = roomName; // �� �̸� ����
        }
    }

    public void AcceptChatRequest()
    {
        chatRequestUI.SetActive(false);
        isJoiningPrivateRoom = true; // 1��1 �� �̵� �÷��� Ȱ��ȭ
        PhotonNetwork.LeaveRoom(); // ���� �� ������
    }
 

    public void DeclineChatRequest()
    {
        chatRequestUI.SetActive(false);
    }

    public void LeavePrivateRoom()
    {
        Debug.Log("Leaving private room...");
        isJoiningPrivateRoom = false; // �⺻ ������ ���ư� �غ�
        PhotonNetwork.LeaveRoom(); // ���� �� ������
    }
}