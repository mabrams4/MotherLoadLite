using UnityEngine;

// handles inventory full and purchasing item audio
public class AudioManager : MonoBehaviour
{
    public AudioSource inventoryFullSound;
    public AudioSource purchaseItemSound;

    public void PlayInventoryFullSound()
    {
        inventoryFullSound.Play();
    }

    public void PlayPurchaseItemSound()
    {
        purchaseItemSound.Play();
    }
}
