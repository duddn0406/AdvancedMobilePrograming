using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerNickname : MonoBehaviourPun
{
    public Text nicknameText; // Canvas�� Text ������Ʈ ����

    void Start()
    {



        if (photonView.IsMine)
        {
            // �� �г����� ǥ��
            nicknameText.text = PhotonNetwork.NickName;
        }
        else
        {
            // �ٸ� �÷��̾��� �г��� ǥ��
            nicknameText.text = photonView.Owner.NickName;
        }
    }
}