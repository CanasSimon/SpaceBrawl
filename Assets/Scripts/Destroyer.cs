using Photon.Pun;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    //Destroys the bullet if it goes outside of the play area
    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Bullets")) return;
        Destroy(other.gameObject);
    }
}
