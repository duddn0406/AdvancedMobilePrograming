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
        // ���� ���� �÷��̾��� �������� ����
        if (!photonView.IsMine)
        {
            // �ٸ� �÷��̾��� ���, ��ġ ����ȭ�� ���� ī�޶� ��Ȱ��ȭ�ϰų� ������ �� ����
            Destroy(GetComponent<csh_Player>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)  // ���� �÷��̾��� ���� �Է��� �޵���
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            // isHorizonMove ���� �����ϴ� ���� ����ȭ
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Vertical"))
                isHorizonMove = true;
            else if (Input.GetButtonDown("Vertical") || Input.GetButtonUp("Horizontal"))
                isHorizonMove = false;

            // �ִϸ��̼� �� ���游 �� �� ��������� ���� ��쿡�� ȣ��
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

    // FixedUpdate�� ���� ��꿡�� ���
    private void FixedUpdate()
    {
        if (photonView.IsMine)  // ���� �÷��̾��� ���� ���� ���
        {
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            rigid.velocity = moveVec * speed;
        }
    }

    // PhotonView�� ���� ��ġ ����ȭ
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

