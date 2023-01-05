using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

/* this script is attatched to all worms and controls all of their behaviors */
public class WormController : MonoBehaviour
{
    // Tilemap things
    Grid grid;
    Tilemap skyMap;
    Tilemap mineralMap;
    Tilemap dirtMap;
    TileBase dirtTile;
    GroundMapTracker groundMapTracker;

    // acid ability
    public TileBase acidTile;
    public Tilemap acidMap;
    public bool hasAcidAbility;
    Queue<Vector3Int> acidTilePositions = new();
    WaitForSeconds waitSeconds;
    WaitUntil waitUntilNoAcidTilesLeft;
    bool hasDied;

    // Player interaction 
    RogueColony rogueColony;
    PlayerController playerController;
    PlayerInfo playerInfo;
    Transform player;

    // Worm stats
    public float health;
    public float searchRadius;
    public float damage;
    bool detectedPlayer;

    // Tunneling info
    float startTime;
    float journeyLength;
    Vector3 startPos;
    Vector3 endPos;
    bool isTunneling;
    bool updatedTiles;
    public Color baseDirtTileColor;
    float gridMapScale;

    int direction;
    int right = 0;
    int down = 1;
    int left = 2;
    int up = 3;

    // Tunneling algorithm parameters
    public float functionCallTime;
    public float tunnelingSpeed;
    public int numTriesToDrillNewGround;
    public int numUpTunnelsToDo;
    private int numUpTunnelsDone = 0;
    public float upDirectionChance;

    // Audio
    public AudioSource wormDeathSound;

    private void Start()
    {
        playerInfo = PlayerInfo.instance;
        playerController = PlayerController.instance;
        rogueColony = RogueColony.instance;
        player = playerInfo.GetComponent<Transform>();
        waitSeconds = new WaitForSeconds(functionCallTime * 1.5f);
        waitUntilNoAcidTilesLeft = new WaitUntil(() => acidTilePositions.Count == 0);

        groundMapTracker = GetComponent<GroundMapTracker>();
        grid = playerController.grid;
        gridMapScale = grid.transform.localScale.x;
        dirtMap = playerController.GetTileMap("DirtMap");
        skyMap = playerController.GetTileMap("SkyMap");
        mineralMap = playerController.GetTileMap("MineralMap");
        dirtTile = playerController.dirtTile;   // maybe change color for worm tiles to make


        InvokeRepeating(nameof(Tunnel), functionCallTime / 10, functionCallTime);
        if (hasAcidAbility) StartCoroutine(ClearAcidTiles());
        GetComponent<Animator>().speed = functionCallTime;      // makes animations match tunneling rate
    }
    void Update()
    {
        if (health <= 0) StartCoroutine(Die());
        if (PlayerInRange()) detectedPlayer = true;
        else { detectedPlayer = false; }

        if (isTunneling)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * tunnelingSpeed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
        }

        // once done tunneling
        if (transform.position == endPos && !updatedTiles) UpdateTiles();

        // trigger rogue colony transmission
        if (GetComponent<Renderer>().isVisible && !rogueColony.encounteredWorm)
        {
            rogueColony.encounteredWorm = true;
        }
    }

    // clears acid tiles left by the worm at a slightly slower rate than their tunneling rate
    IEnumerator ClearAcidTiles()
    {
        while (true)
        {
            yield return waitSeconds;
            Vector3Int tilePos = acidTilePositions.Dequeue();
            acidMap.SetTile(tilePos, null);
            if (acidTilePositions.Count == 0 && hasDied) yield break;
        }
    }

    // applies damage to the worm
    public void ApplyDamage(float damage)
    {
        health -= damage;
    }

    // kills the worm. If worm has the acid ability,
    // hides worm game object keeping it enabled until all acid tiles are cleared
    public IEnumerator Die()
    {
        if (GetComponent<Renderer>().isVisible) wormDeathSound.Play();
        hasDied = true;
        if (hasAcidAbility)
        {
            grid = null;
            CancelInvoke(nameof(Tunnel));
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            yield return waitUntilNoAcidTilesLeft;
        }
        Destroy(gameObject);
    }

    // draws out the worm search radius in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.exposure = Texture2D.normalTexture;
        Gizmos.DrawWireSphere(transform.position, searchRadius);

    }

    // returns true if the player is in a certain radius of the worm
    bool PlayerInRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < searchRadius) return true;
        return false;
    }

    // calculates the position of the next tile to be tunneled into
    Vector3 CalculateTunnelingDestination()
    {
        Vector3 offset = new Vector3(0, 0, 0);

        // all worms start by tunneling up NumUpTunnelsToDo times
        if (numUpTunnelsDone < numUpTunnelsToDo)
        {
            direction = 3;
            numUpTunnelsDone++;
        }

        // randomly select a new direction to tunnel
        else { direction = DecideDirection(); }

        // keep the worm facing the right way
        if (direction == right)
        {
            transform.localEulerAngles = new Vector3(0, 0, 270);
            offset.x += gridMapScale;
        }
        else if (direction == left)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
            offset.x -= gridMapScale;
        }
        else if (direction == down)
        {
            transform.localEulerAngles = new Vector3(0, 0, 180);
            offset.y -= gridMapScale;
        }
        else if (direction == up)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
            offset.y += gridMapScale;
        }
        return transform.position + offset;
    }

    // returns true if the passed in position is a dirt tile in the grid
    bool IsDirtTile(Vector3 endPos)
    {
        return (dirtMap.GetTile(grid.WorldToCell(endPos)));
    }

    // returns true if the passed in position is a sky tile in the grid
    bool IsSkyTile(Vector3 endPos)
    {
        return (skyMap.GetTile(grid.WorldToCell(endPos)));
    }

    // resets the tunneling timer to allow the worm to tunnel again
    void Tunnel()
    {
        isTunneling = false;
    }

    // Calculated start and end positions for the Lerp function called in Update for drilling
    void CalculateTunnelingParameters()
    {
        isTunneling = true;
        startTime = Time.time;
        startPos = transform.position;

        Vector3Int tilePos = grid.WorldToCell(CalculateTunnelingDestination());
        endPos = grid.GetCellCenterWorld(tilePos);

        if (IsSkyTile(endPos))
        {
            StartCoroutine(Die());   // if next tile is a sky tile worm is at surface so dies
            //return;
        }

        if (numUpTunnelsDone == numUpTunnelsToDo)
        {
            // if the next tile to tunnel to is already a dirt tile, try numTriesToDrillNewGround times to pick a
            // non-dirt tile
            for (int i = 0; i < numTriesToDrillNewGround; i++)
            {
                if (IsDirtTile(endPos))
                {
                    tilePos = grid.WorldToCell(CalculateTunnelingDestination());
                    endPos = grid.GetCellCenterWorld(tilePos);
                }
                else { break; }
            }
        }
        journeyLength = Vector3.Distance(startPos, endPos);
    }

    // updates the tile that the worm just tunneled into, removing minerals if their are any
    void UpdateTiles()
    {
        if (grid == null) return;   // if worm is dead but still enabled to clear acid tiles stop updating tiles

        Vector3Int gridTilePos = grid.WorldToCell(endPos);
        if (IsSkyTile(gridTilePos)) return; // dont update sky tiles
        Vector3Int groundTilePos = groundMapTracker.currentGroundMap.WorldToCell(endPos);

        // if tunneled into a mineral, delete the mineral
        if (mineralMap.GetTile(gridTilePos)) mineralMap.SetTile(gridTilePos, null);

        // remove ground tile to get rid of collider, then replace with dirt tile
        groundMapTracker.currentGroundMap.SetTile(groundTilePos, null);

        if (hasAcidAbility)
        {
            acidMap.SetTile(gridTilePos, acidTile);
            acidTilePositions.Enqueue(gridTilePos);
        }

        Color dirtTileColor = playerController.CalculateDirtTileColor(baseDirtTileColor, gridTilePos.y);
        dirtMap.SetTile(gridTilePos, dirtTile);
        dirtMap.SetTileFlags(gridTilePos, TileFlags.None);
        dirtMap.SetColor(gridTilePos, dirtTileColor);
        updatedTiles = true;
    }

    // if not already tunneling and isTunneling is false start tunneling
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isTunneling)
        {
            CalculateTunnelingParameters();
            updatedTiles = false;
        }
    }

    // manages colliding with an evolved baby worm
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collided with player
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInfo.ApplyDamage(damage);
            StartCoroutine(Die());
        }
        
        // collider with babyWorm or Bounds
        if ((collision.CompareTag("BabyWorm") && BabyWorm.instance.evolution == BabyWorm.Evolutions.ADULT)
            || collision.CompareTag("Bounds"))
        {
            StartCoroutine(Die());
        }
    }

    // returns an int representing the next direction to tunnel in
    int DecideDirection()
    {
        // if player in range tunnel towards the player
        if (detectedPlayer)
        {
            float ydif = player.position.y - transform.position.y;
            float xdif = player.position.x - transform.position.x;
            if (Mathf.Abs(ydif) > Mathf.Abs(xdif))
            {
                if (ydif > 0) return up;
                else { return down; }
            }
            else
            {
                if (xdif > 0) return right;
                else { return left; }
            }
        }

        // chooses a random direction but has a set probability to choose up
        float p = Random.Range(0f, 1f);
        if (p < upDirectionChance) direction = up;
        else { return Random.Range(0, 4); }
        return direction;
    }
}
