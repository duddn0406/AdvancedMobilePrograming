using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerNickname : MonoBehaviourPun
{
    public Text nicknameText; // Canvas의 Text 컴포넌트 연결

    void Start()
    {



        if (photonView.IsMine)
        {
            // 내 닉네임을 표시
            nicknameText.text = PhotonNetwork.NickName;
        }
        else
        {
            // 다른 플레이어의 닉네임 표시
            nicknameText.text = photonView.Owner.NickName;
        }
    }
}