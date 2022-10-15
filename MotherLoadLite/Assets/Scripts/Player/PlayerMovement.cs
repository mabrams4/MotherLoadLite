using UnityEngine;

/* this script manages player movement */
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerController playerController;
    public float horizontalMove;
    public float verticalMove;
    public float acceleration;
    public float deceleration;
    public float velPower;
    public float moveSpeed;
    public float flySpeed;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // store player input to be used in FixedUpdate for movement
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        
        // keep player facing the right direction
        if (horizontalMove > 0 && !playerController.isDrilling) transform.rotation = new Quaternion(0, 180, 0, 0);
        if (horizontalMove < 0 && !playerController.isDrilling) transform.rotation = Quaternion.identity;
    }

    // calculate and apply forces to player rigid body
    private void FixedUpdate()
    {
        // horizontal movement
        float targetSpeed = horizontalMove * moveSpeed;
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > .01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        // vertical movement
        if (!playerController.isDrilling)
        {
            rb.AddForce(movement * Vector2.right);
            rb.AddForce(verticalMove * flySpeed * Vector2.up);
        }
    }
}
