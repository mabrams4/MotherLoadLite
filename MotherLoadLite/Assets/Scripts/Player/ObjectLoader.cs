using UnityEngine;
using UnityEngine.Tilemaps;

/* this script handles loading in tilemap chunks when the player or a worm gets close enough so that
 * only necessary tile data be loaded at any given time
 */
public class ObjectLoader : MonoBehaviour
{
    // if colliding with a groundMap chunk that is not enabled, enable it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!collision.gameObject.GetComponentInChildren<TilemapRenderer>().enabled)
            {
                EdgeCollider2D[] bounds = collision.gameObject.GetComponentsInChildren<EdgeCollider2D>();
                foreach (EdgeCollider2D bound in bounds) bound.enabled = true;
                collision.gameObject.GetComponentInChildren<TilemapRenderer>().enabled = true;
                collision.gameObject.GetComponentInChildren<TilemapCollider2D>().enabled = true;
            }
        }
    }

    // disable the groundMap chunk when it is no longer needed
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (collision.gameObject.GetComponentInChildren<TilemapRenderer>().enabled
                && gameObject.tag == "PlayerObjectLoader")
            {
                EdgeCollider2D[] bounds = collision.gameObject.GetComponentsInChildren<EdgeCollider2D>();
                foreach (EdgeCollider2D bound in bounds) bound.enabled = false;
                collision.gameObject.GetComponentInChildren<TilemapRenderer>().enabled = false;
                collision.gameObject.GetComponentInChildren<TilemapCollider2D>().enabled = false;
            }
        }
    }
}
