using System;
using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public static GameObject localPlayerInstance;
    
    #region Movement
    [Header("Movement")]
    [SerializeField] private float speed = .1f;
    [SerializeField] private float turnSpeed = .2f;
    
    private float moveX;
    private float moveY;
    #endregion
    
    #region Bullets
    [Header("Bullets")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private float shootSpeed = 0.1f;
    private float shootCdTimeStamp;
    #endregion

    #region Stats
    [Header("Stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int attack;
    public int Health;
    
    [SerializeField] private GameObject playerUiPrefab;
    #endregion

    private void Awake()
    {
        if (photonView.IsMine)
        {
            localPlayerInstance = gameObject;
        }
        
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.GetComponentInChildren<CinemachineVirtualCamera>().m_Follow = transform;
        }
        else
        {
            Debug.Log("Error: No Camera in scene");
        }
        
        var uiGo = Instantiate(playerUiPrefab);
        uiGo.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    private void Update()
    {
        if(!photonView.IsMine && PhotonNetwork.IsConnected) return;
        
        Move();

        if (Time.time > shootCdTimeStamp && Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Health);
        }
        else
        {
            Health = (int) stream.ReceiveNext();
        }
    }
    
    private void Move()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        var moveVec = new Vector3(moveX, moveY) * speed;

        var position = transform.position;
        position = Vector3.MoveTowards(position, position + moveVec, 1);
        transform.position = position;

        if (moveVec != Vector3.zero)
            transform.rotation = 
                Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, moveVec), turnSpeed);
    }

    private void Shoot()
    {
        shootCdTimeStamp = Time.time + shootSpeed;

        Instantiate(bullet, bulletSpawner.position, transform.rotation);
    }
}
