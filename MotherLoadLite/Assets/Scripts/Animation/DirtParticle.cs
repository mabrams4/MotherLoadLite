using UnityEngine;

/* This script deals with destroying animated dirt particles from drilling once they are no longer
 * in the screen view 
 */

public class DirtParticle : MonoBehaviour
{
    public bool isActive;

    void Update()
    {
        if (!GetComponent<Renderer>().isVisible && isActive) Destroy(gameObject);
    }

    // applies a random force to the dirt particle 
    public void ApplyForce(Vector2 force)
    {
        GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
    }
}
