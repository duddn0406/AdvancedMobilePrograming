using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviourPun
{
    public Camera playerCamera;

    void Start()
    {
        if (photonView.IsMine) // 로컬 플레이어일 경우
        {
            playerCamera.enabled = true; // 자신의 카메라 활성화
        }
        else
        {
            playerCamera.enabled = false; // 다른 플레이어의 카메라 비활성화
        }
    }
}