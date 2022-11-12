using UnityEngine;

// handles all player related audio
public class PlayerAudio : MonoBehaviour
{
    public AudioSource drillSound;
    public AudioSource deathExplosionSound;
    public AudioSource explosiveSound;
    public AudioSource collectMineralSound;
    public AudioSource teleportSound;
    public AudioSource fallingDamageSound;

    public void PlayerFallingDamageSound()
    {
        fallingDamageSound.Play();
    }
    public void PlayDrillSound()
    {
        if (!drillSound.isPlaying) drillSound.Play();
    }

    public void StopDrillSound()
    {
        drillSound.Stop();
    }

    public void PlayDeathExplosionSound()
    {
        deathExplosionSound.Play();
    }

    public void PlayExplosiveSound()
    {
        explosiveSound.Play();
    }

    public void PlayCollectMineralSound()
    {
        collectMineralSound.Play();
    }

    public void PlayTeleportSound()
    {
        teleportSound.Play();
    }

}
