using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float bulletSpeed;
    
    //To prevent a player from shooting themselves
    public Player Owner { get; set; }
    
    //Initializes the bullet and sets its owner
    //Uses shotTime to compensate lag
    public void Initialize(Player owner, double shotTime)
    {
        body.velocity = bulletSpeed * Time.deltaTime * transform.up;
        Owner = owner;
        var nextPos = (Vector3) body.velocity * (float) (PhotonNetwork.Time - shotTime);
        transform.position += nextPos;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Destructor")) Destroy(gameObject);
    }
}
