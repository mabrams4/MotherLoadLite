using UnityEngine;

/* this script triggers the final dialogue when the player blows up the queen worm nest and beats the game */
public class QueenNest : MonoBehaviour
{
    RogueColony rogueColony;
    void Start()
    {
        rogueColony = RogueColony.instance;
    }

    private void OnDisable()
    {
        rogueColony.blownUpQueenNest = true;
    }
}
