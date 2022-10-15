using UnityEngine;

/* this script manages worms running into game bounds, destroying them when they do */
public class WormBoundsDetector : MonoBehaviour
{
    // destroys the worm if they collide with a game "Bounds" object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bounds")) Destroy(gameObject);
    }
}
