using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class csh_Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed;
    private Rigidbody2D rigid;
    private Animator anim;
    public PhotonView PV;
    private float h;
    private float v;
    private bool isHorizonMove;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rigid == null)
        {
            Debug.LogError("Rigidbody2D is missing from this GameObject.");
        }
        if (anim == null)
        {
            Debug.LogError("Animator is missing from this GameObject.");
        }
        if (photonView == null)
        {
            Debug.LogError("PhotonView is missing from this GameObject.");
        }
        // 만약 로컬 플레이어라면 움직임을 제어
        if (!photonView.IsMine)
        {
            // 다른 플레이어의 경우, 위치 동기화를 위해 카메라를 비활성화하거나 제어할 수 있음
            Destroy(GetComponent<csh_Player>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)  // 로컬 플레이어일 때만 입력을 받도록
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            // isHorizonMove 값을 설정하는 로직 간소화
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Vertical"))
                isHorizonMove = true;
            else if (Input.GetButtonDown("Vertical") || Input.GetButtonUp("Horizontal"))
                isHorizonMove = false;

            // 애니메이션 값 변경만 할 때 변경사항이 있을 경우에만 호출
            if (anim.GetInteger("hAxisRaw") != h)
            {
                anim.SetBool("isChanged", true);
                anim.SetInteger("hAxisRaw", (int)h);
            }
            else if (anim.GetInteger("vAxisRaw") != v)
            {
                anim.SetBool("isChanged", true);
                anim.SetInteger("vAxisRaw", (int)v);
            }
            else
            {
                anim.SetBool("isChanged", false);
            }
        }
    }

    // FixedUpdate는 물리 계산에서 사용
    private void FixedUpdate()
    {
        if (photonView.IsMine)  // 로컬 플레이어일 때만 물리 계산
        {
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            rigid.velocity = moveVec * speed;
        }
    }

    // PhotonView를 통해 위치 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
           
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
           
        }
    }
}

