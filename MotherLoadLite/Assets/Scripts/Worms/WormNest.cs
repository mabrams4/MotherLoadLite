using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

/* this script is attatched to each worm nest in the game and manages the spawning of worms based on the 
 * inherent enable depth, spawn rate, and worm stats
 */
public class WormNest : MonoBehaviour
{
    PlayerInfo playerInfo;
    public float health = 50f;

    // Worm stats
    public GameObject worm;
    public float spawnRate;
    public float wormSearchRadius;
    public float wormDamage;
    public float wormHealth;

    // Worm Tunneling info
    public float wormFunctionCallTime;
    public float wormTunnelingSpeed;
    public int wormNumTriesToDrillNewGround;
    public int wormNumUpTunnelsToDo = 50;
    public float wormUpDirectionChance;

    public int maxNumSpawns;
    public float enableDepth;

    // Rogue colony transmission
    RogueColony rogueColony;

    public Tilemap acidMap;
    public bool hasAcidAbility;
    public TileBase acidTile;
    public Color baseDirtTileColor;

    // Audio
    public AudioSource wormDeathSound;

    void Start()
    {
        rogueColony = RogueColony.instance;
        playerInfo = PlayerInfo.instance;
        StartCoroutine(SpawnWorms());
    }

    void Update()
    {
        // trigger rogue colony transmission
        if (GetComponent<Renderer>().isVisible && !rogueColony.encounteredNest)
        {
            rogueColony.encounteredNest = true;
        }
        if (health <= 0)
        {
            wormDeathSound.Play();
            Destroy(gameObject);
        }
    } 

    // applies damage to the worm nest
    public void ApplyDamage(float damage)
    {
        health -= damage;
    }

    // spawns worms based on the spawn rate once the player reaches enableDepth and initializes values for the
    // Worm Controller script attatched to each worm
    IEnumerator SpawnWorms()
    {
        yield return new WaitUntil(() => playerInfo.altitude <= -enableDepth);

        for (int i = 0; i < maxNumSpawns; i++)
        {
            yield return new WaitForSeconds(spawnRate);

            Vector3 spawnPos = transform.position;
            int offset = Random.Range(-20, 20);
            spawnPos.x += offset;
            GameObject wormClone = Instantiate(worm, spawnPos, Quaternion.identity);
            WormController wc = wormClone.AddComponent<WormController>();
            wc.searchRadius = wormSearchRadius;
            wc.damage = wormDamage;
            wc.functionCallTime = wormFunctionCallTime;
            wc.tunnelingSpeed = wormTunnelingSpeed;
            wc.numTriesToDrillNewGround = wormNumTriesToDrillNewGround;
            wc.numUpTunnelsToDo = wormNumUpTunnelsToDo;
            wc.upDirectionChance = wormUpDirectionChance;
            wc.health = wormHealth;
            wc.hasAcidAbility = hasAcidAbility;
            wc.acidTile = acidTile;
            wc.acidMap = acidMap;
            wc.baseDirtTileColor = baseDirtTileColor;
            wc.wormDeathSound = wormDeathSound;
        }
    }
}