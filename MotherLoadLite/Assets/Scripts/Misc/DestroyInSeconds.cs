using UnityEngine;

// this script destroys the game object it sits on after seconds delay
public class DestroyInSeconds : MonoBehaviour
{
    public float seconds;
    void Start()
    {
        Destroy(gameObject, seconds);
    }
}
