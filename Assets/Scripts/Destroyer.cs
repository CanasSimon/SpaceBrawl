using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        Destroy(other.gameObject);
    }
}
