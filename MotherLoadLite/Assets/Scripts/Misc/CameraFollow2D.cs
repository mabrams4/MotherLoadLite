using UnityEngine;

/* this script is responsible for making the camera smoothly track the player in game */
public class CameraFollow2D : MonoBehaviour
{
    public Transform player;
    public float timeOffset;
    public Vector2 posOffset;

    private Vector3 velocity;

    // Screen Limits
    public float rightLimit;
    public float leftLimit;
    public float topLimit;
    public float bottomLimit;
    void Update()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = player.position;
        endPos.x += posOffset.x;
        endPos.y += posOffset.y;
        endPos.z = -10;

        // smoothly follow player position
        transform.position = Vector3.SmoothDamp(startPos, endPos, ref velocity, timeOffset * Time.deltaTime);

        // clamp camera position to game bounds
        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, leftLimit, rightLimit),
            Mathf.Clamp(transform.position.y, bottomLimit, topLimit),
            transform.position.z
        );
    }
}
