using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
	public static GameObject LocalPlayerInstance;

	public Player Owner;

	[SerializeField] private GameObject explosion;

	#region Movement Variables
	[Header("Movement")] 
	[SerializeField] private float speed = .1f;
	[SerializeField] private float turnSpeed = .2f;

	private Rigidbody2D body;
	private float moveX;
	private float moveY;

	private Vector2 networkPosition;
	private Quaternion networkRotation;

	#endregion

	#region Bullets Variables
	[Header("Bullets")] 
	[SerializeField] private GameObject bullet;
	[SerializeField] private Transform bulletSpawner;
	[SerializeField] private float shootSpeed = 0.1f;
	private float shootCdTimeStamp;

	[SerializeField] private AudioSource shootingAudioSource;
	#endregion

	#region Stats Variables
	[Header("Stats")] 
	public static int MaxHealth = 20;

	public int Health;

	[SerializeField] private GameObject playerUiPrefab;
	private PlayerUI playerUi;
	#endregion

	#region Unity Methods
	private void Awake()
	{
		if (photonView.IsMine)
		{
			LocalPlayerInstance = gameObject;
		}

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		Owner = photonView.Owner;
		Debug.Log(Owner.NickName);
			
		body = GetComponent<Rigidbody2D>();
			
		Health = MaxHealth;

		var uiGo = Instantiate(playerUiPrefab);
		uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

		playerUi = uiGo.GetComponent<PlayerUI>();

		if (photonView.IsMine)
		{
			var hashState = new Hashtable {{"Health", Health}};
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashState);
		}

	}

	//Processes inputs on the local side
	private void Update()
	{
		if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

		Move();

		if (Time.time > shootCdTimeStamp && Input.GetButtonDown("Fire1"))
		{
			shootCdTimeStamp = Time.time + shootSpeed;
			photonView.RPC("Shoot", RpcTarget.All);
		}
	}
	
	private void OnDestroy()
	{
		var newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
		var explosionParticles = newExplosion.GetComponent<ParticleSystem>();
		Destroy(newExplosion, explosionParticles.main.duration + explosionParticles.main.startLifetimeMultiplier);
		Destroy(playerUi.gameObject);
	}

	//Detects if a bullet hits the player
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Bullets") || 
		    Equals(other.GetComponent<Bullet>().Owner, photonView.Owner)) return;
		
		Health--;
		Destroy(other.gameObject);
		
		if(Health <= 0) DestroySelf();
	}
	#endregion

	#region PUN Methods
	//Sends and receives position, rotation, and health to the other clients
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(Health);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			Health = (int) stream.ReceiveNext();
			UpdateHealth();
			
			networkPosition = (Vector3)stream.ReceiveNext();
			networkRotation = (Quaternion)stream.ReceiveNext();

			float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
			if(body != null) networkPosition += body.velocity * lag;
		}
	}

	//Fire a bullet and send the event to all other clients
	[PunRPC]
	private void Shoot()
	{
		shootingAudioSource.Play();
		var newBullet = Instantiate(bullet, bulletSpawner.position, transform.rotation);
		newBullet.GetComponent<Bullet>().Owner = photonView.Owner;
	}

	//Updates all the players' health bars
	private void UpdateHealth()
	{
		var hashState = new Hashtable {{"Health", Health}};
		Owner?.SetCustomProperties(hashState);

		if (playerUi != null) playerUi.UpdateHealth();
	}
	#endregion

	//Handles the movement of the player
	private void Move()
	{
		moveX = Input.GetAxis("Horizontal");
		moveY = Input.GetAxis("Vertical");
		body.velocity = new Vector2(moveX, moveY) * speed;

		if (body.velocity != Vector2.zero)
			transform.rotation =
				Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, body.velocity), turnSpeed);
		
		if (!photonView.IsMine)
		{
			transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.fixedDeltaTime);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
		}
	}

	private void DestroySelf()
	{
		UpdateHealth();
		GameManager.Instance.CheckPlayerStatus();
		if (photonView.IsMine) PhotonNetwork.Destroy(gameObject);
	}
}