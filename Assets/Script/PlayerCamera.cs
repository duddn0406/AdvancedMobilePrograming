using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviourPun
{
    public Camera playerCamera;

    void Start()
    {
        if (photonView.IsMine) // ���� �÷��̾��� ���
        {
            playerCamera.enabled = true; // �ڽ��� ī�޶� Ȱ��ȭ
        }
        else
        {
            playerCamera.enabled = false; // �ٸ� �÷��̾��� ī�޶� ��Ȱ��ȭ
        }
    }
}