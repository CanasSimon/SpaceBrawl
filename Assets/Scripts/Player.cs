using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private float shootSpeed;
    private float shootCdTimeStamp;
    
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;

    private float moveX;
    private float moveY;

    // Update is called once per frame
    private void Update()
    {
        Move();

        if (Time.time > shootCdTimeStamp && Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Move()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        var moveVec = new Vector3(moveX, moveY) * speed;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + moveVec, 1);
        
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
