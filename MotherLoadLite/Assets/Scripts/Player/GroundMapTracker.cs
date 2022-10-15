using UnityEngine;
using UnityEngine.Tilemaps;

/* This script keeps track of the current and previous groundMaps being used by the player or worm 
 * that this script is attached to. Necessary for various tilemap calculations
 */
public class GroundMapTracker : MonoBehaviour
{
    public Tilemap currentGroundMap;
    public Tilemap previousGroundMap;
    Tilemap[] groundMaps;

    private void Start()
    {
        groundMaps = PlayerController.instance.groundMaps;
    }

    // changes the current groundMap to the groundMap associated with the collision
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentGroundMap = collision.gameObject.GetComponentInChildren<Tilemap>();
            for (int i = 0; i < groundMaps.Length; i++)
            {
                if (groundMaps[i] == currentGroundMap && i > 0) previousGroundMap = groundMaps[i - 1];
            }
        }
    }
}
