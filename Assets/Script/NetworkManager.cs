using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // UI 요소들
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

    private string selectedPlayer; // 선택된 플레이어 닉네임
    private string privateRoomName; // 1대1 대화방 이름
    private bool isJoiningPrivateRoom = false; // 1대1 방 이동 여부

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
        // 방 입장 로직 처리
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
        chatDisplay.text += $"{PhotonNetwork.NickName}님이 방에 입장했습니다.\n";

        SpawnPlayer(); // 플레이어 스폰
    }

    private void SpawnPlayer()
    {
        // 플레이어 오브젝트 생성
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
            selectedPlayer = targetPhotonView.Owner.NickName; // 선택된 플레이어 닉네임
            confirmRequestUI.SetActive(true);
            confirmText.text = $"{selectedPlayer}님과 1대1 대화를 하시겠습니까?";
        }
    }

    public void DeclineConfirm()
    {
        confirmRequestUI.SetActive(false);
    }

    public void ConfirmPrivateChat()
    {
        confirmRequestUI.SetActive(false);

        // 방 이름 생성
        privateRoomName = $"{PhotonNetwork.NickName}-{selectedPlayer}-{System.Guid.NewGuid()}";

        Photon.Realtime.Player targetPlayer = PhotonNetwork.PlayerListOthers
            .FirstOrDefault(p => p.NickName == selectedPlayer);

        if (targetPlayer != null)
        {
            isJoiningPrivateRoom = true; // 1대1 방 이동 플래그 활성화
            photonView.RPC("SendChatRequest", targetPlayer, PhotonNetwork.NickName, selectedPlayer, privateRoomName);
            PhotonNetwork.LeaveRoom(); // 현재 방 나가기
        }
    }

    [PunRPC]
    public void SendChatRequest(string sender, string target, string roomName)
    {
        if (PhotonNetwork.NickName == target)
        {
            chatRequestUI.SetActive(true);
            requestText.text = $"{sender}님이 1대1 채팅을 요청했습니다. 수락하시겠습니까?";
            privateRoomName = roomName; // 방 이름 저장
        }
    }

    public void AcceptChatRequest()
    {
        chatRequestUI.SetActive(false);
        isJoiningPrivateRoom = true; // 1대1 방 이동 플래그 활성화
        PhotonNetwork.LeaveRoom(); // 현재 방 나가기
    }
 

    public void DeclineChatRequest()
    {
        chatRequestUI.SetActive(false);
    }

    public void LeavePrivateRoom()
    {
        Debug.Log("Leaving private room...");
        isJoiningPrivateRoom = false; // 기본 방으로 돌아갈 준비
        PhotonNetwork.LeaveRoom(); // 현재 방 나가기
    }
}