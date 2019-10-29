using Photon.Realtime;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float bulletSpeed;
    
    //To prevent a player from shooting themselves
    public Player Owner { get; set; }
    
    //Launches the bullet forward
    private void Start()
    {
        body.velocity = bulletSpeed * Time.deltaTime * transform.up;
    }
}
