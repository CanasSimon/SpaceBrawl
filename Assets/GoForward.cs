using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoForward : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float bulletSpeed;
    
    private void Start()
    {
        body.velocity = transform.up * bulletSpeed * Time.deltaTime;
    }
}
