using UnityEngine;

/* this script manages damaging the player should they be in contact with worm acid */
public class WormAcid : MonoBehaviour
{
    public float acidDamage;
    PlayerInfo playerInfo;
    bool playerInAcid;

    void Start()
    {
        playerInfo = PlayerInfo.instance;
        InvokeRepeating(nameof(DoAcidDamage), .1f, .1f);
    }

    // toggle boolean for whether or not player is in acid
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInAcid = true;
    }

    // toggle boolean for whether or not player is in acid
    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInAcid = false;
    }

    // applies acid damage to player
    void DoAcidDamage()
    {
        if (playerInAcid) playerInfo.ApplyAcidDamage(acidDamage);
    }
}
