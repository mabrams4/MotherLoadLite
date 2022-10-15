using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* this script manages all baby worm behaviors and player interactions */
public class BabyWorm : MonoBehaviour
{
    #region Singleton
    public static BabyWorm instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There can only be ONE Baby Worm :)");
            return;
        }
        instance = this;
    }
    #endregion

    // Instance objects
    PlayerController playerController;
    PlayerInfo playerInfo;
    Transform player;
    MissionControl missionControl;
    RogueColony rogueColony;
    Inventory playerInventory;
    DataMineralItem gameData;

    // Baby Worm specific
    public float radiusFromPlayer;
    public float groundedPosition;
    public float followSpeed;
    Animator animator;

    // Tunnel Speed parameters
    public float tunnelingSpeed;
    public float babyTunnelRate = 2;
    public float adultTunnelRate = .35f;

    // Tilemap things
    public Grid currentGrid;
    Tilemap dirtMap;
    Tilemap playerDirtMap;
    TileBase dirtTile;
    Tilemap mineralMap;
    GroundMapTracker groundMapTracker;

    // Tunneling info
    float startTime;
    float journeyLength;
    Vector3 startPos;
    Vector3 endPos;
    bool isTunneling;
    bool updatedTiles;
    float gridMapScale;

    int direction;
    int right = 0;
    int down = 1;
    int left = 2;
    int up = 3;

    // Dirt tile color
    public Color baseDirtTileColor;

    // Follow Player Phase
    bool onJourneyToSurface;
    public float teleportDistance;

    // Hunt Worms Phase
    public float huntRadius;
    bool isHunting = false;
    bool killedWorm = false;

    // Game Data
    public string currentEvolution;
    public bool isEvolved;

    #region Enums
    public enum Evolutions
    {
        UNDISCOVERED,
        BABY,
        ADULT
    }

    // State machine phases
    public enum Phase
    {
        IDLE,
        WANDER,
        FOLLOW_PLAYER,
        WAIT_FOR_PLAYER_COMMAND,
        HUNT_WORMS
    }
    #endregion

    public Evolutions evolution;
    public Phase phase;

    void Start()
    {
        playerInfo = PlayerInfo.instance;
        playerController = PlayerController.instance;
        missionControl = MissionControl.instance;
        rogueColony = RogueColony.instance;
        playerInventory = Inventory.instance;
        gameData = DataMineralItem.instance;

        mineralMap = playerController.GetTileMap("MineralMap");
        playerDirtMap = playerController.GetTileMap("DirtMap");
        dirtTile = playerController.dirtTile;
        gridMapScale = currentGrid.transform.localScale.x;
        groundMapTracker = GetComponentInChildren<GroundMapTracker>();

        player = playerController.GetComponent<Transform>();
        animator = GetComponent<Animator>();

        phase = Phase.IDLE;
        evolution = Evolutions.UNDISCOVERED;
        currentEvolution = "UNDISCOVERED";
        InvokeRepeating(nameof(TunnelTimer), .01f, babyTunnelRate);
    }

    void Update()
    {
        // trigger evolution to adult 
        if (isEvolved && evolution == Evolutions.BABY)
        {
            evolution = Evolutions.ADULT;

            // For Game Data storage
            currentEvolution = "ADULT";

            StartCoroutine(Evolve());
        }

        // determines baby worm behavior using a state machine
        switch (phase)
        {
            case (Phase.WAIT_FOR_PLAYER_COMMAND):
                // do nothing
                break;
            case (Phase.IDLE):
                Idle();
                break;
            case (Phase.WANDER):
                Wander();
                break;
            case (Phase.FOLLOW_PLAYER):
                FollowPlayer();
                break;
            case (Phase.HUNT_WORMS):
                if (!isHunting) StartCoroutine(HuntWorms());
                break;
        }

        // trigger dialogue transmission
        if (evolution == Evolutions.UNDISCOVERED && GetComponent<Renderer>().isVisible)
        {
            rogueColony.foundBabyWorm = true;
        }
    }

    // hunts the closest worm
    IEnumerator HuntWorms()
    {
        isHunting = true;
        killedWorm = false;
        Vector3 nullPos = new(0, 0, 0);

        // find the closest worm
        Vector3 target = FindClosestWorm();
        while (target == nullPos)
        {
            target = FindClosestWorm();
            yield return null;
        }

        // hunt the worm continuously updating its position
        while (!killedWorm)
        {
            if (!isTunneling) CalculatePathTo(target);
            Tunnel();
            target = FindClosestWorm();
            yield return null;
        }
        isHunting = false;
    }

    // returns the position of the closest worm or a zeroed vector if there are none within the hunting radius
    Vector3 FindClosestWorm()
    {
        bool foundWorm = false;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, huntRadius, ~0);
        float closestDistance = Mathf.Infinity;
        Dictionary<float, Vector3> distances = new();
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Worm"))
            {
                foundWorm = true;
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance) closestDistance = distance;
                distances.Add(distance, hit.transform.position);
            }
        }
        if (foundWorm) return distances[closestDistance];
        else { return new Vector3(0, 0, 0); }
    }

    // follows the player around, tunneling if underground and lerping if above ground or in transition to surface
    void FollowPlayer()
    {
        // teleport to player if get too far away
        if (Vector3.Distance(transform.position, player.position) > teleportDistance)
        {
            transform.position = player.position;
            return;
        }
        // lerp to player while hidden to emulate being in players inventory when first found
        if (onJourneyToSurface)
        {
            LerpToPlayer();
            return;
        }
        // if player is underground, tunnel
        if (playerController.IsUnderground())
        {
            if (!isTunneling) CalculatePathTo(player.position);
            Tunnel();
        }
        // lerp to player above ground and face the right direction
        else
        {
            CalculateFastedDirectionTo(player.transform.position);
            if (direction == right)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (direction == left)
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            LerpToPlayer();
        }
        // transition to idle if close enough to player
        if (Vector3.Distance(transform.position, player.position) < radiusFromPlayer
            && transform.position.y <= player.position.y + .5f) phase = Phase.IDLE;
    }

    // returns true if baby worm is above ground
    bool IsAboveGround()
    {
        Tilemap skyMap = playerController.GetTileMap("SkyMap");
        Vector3Int tilePos = skyMap.WorldToCell(transform.position);
        if (skyMap.GetTile(tilePos)) return true;
        return false;
    }

    // lerps to the player
    void LerpToPlayer()
    {
        animator.SetBool("isIdle", true);
        transform.position = Vector3.Lerp(transform.position, player.position, followSpeed);
        transform.position = new(
            transform.position.x,
            Mathf.Clamp(transform.position.y, Mathf.NegativeInfinity, groundedPosition),
            0);
    }

    // tunnel in random directions
    void Wander()
    {
        if (!isTunneling) CalculateRandomPath();
        Tunnel();
    }

    // wander if not yet discovered, follow player if not close enough, do nothing if close enough to player
    void Idle()
    {
        animator.SetBool("isIdle", true);

        StopCoroutine(HuntWorms());
        isHunting = false;

        if (evolution == Evolutions.UNDISCOVERED) phase = Phase.WANDER;
        else if (Vector3.Distance(transform.position, player.position) > radiusFromPlayer) phase = Phase.FOLLOW_PLAYER;
        else { GetComponent<Rigidbody2D>().Sleep(); }
    }

    #region Tunneling

    void Tunnel()
    {
        if (isTunneling)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isTunneling", true);

            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * tunnelingSpeed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
        }

        // once done tunneling
        if (transform.position == endPos && !updatedTiles)
        {
            UpdateTiles();
        }
    }

    // chooses a random direction to tunnel in and calculates start and end positions for lerping
    void CalculateRandomPath()
    {
        updatedTiles = false;
        startTime = Time.time;
        startPos = transform.position;
        direction = Random.Range(0, 4);
        if (IsAboveGround()) direction = down;

        Vector3Int tilePos = currentGrid.WorldToCell(CalculateTunnelingDestination());
        endPos = currentGrid.GetCellCenterWorld(tilePos);

        journeyLength = Vector3.Distance(startPos, endPos);
        isTunneling = true;
    }

    // calculates the correct direction to tunnel in to move towards target
    void CalculatePathTo(Vector3 target)
    {
        updatedTiles = false;
        startTime = Time.time;
        startPos = transform.position;
        CalculateFastedDirectionTo(target);

        if (IsAboveGround()) direction = down;

        Vector3Int tilePos = currentGrid.WorldToCell(CalculateTunnelingDestination());
        endPos = currentGrid.GetCellCenterWorld(tilePos);

        journeyLength = Vector3.Distance(startPos, endPos);
        isTunneling = true;
    }

    // calulates the fastest direction to target
    void CalculateFastedDirectionTo(Vector3 target)
    {
        float ydif = target.y - transform.position.y;
        float xdif = target.x - transform.position.x;
        if (Mathf.Abs(ydif) > Mathf.Abs(xdif))
        {
            if (ydif > 0) direction = up;
            else { direction = down; }
        }
        else
        {
            if (xdif > 0) direction = right;
            else { direction = left; }
        }
    }


    // calculates the position of the next tile to be tunneled into and updates rotation to face correct way
    Vector3 CalculateTunnelingDestination()
    {
        Vector3 offset = new Vector3(0, 0, 0);
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

    // updates the tile that were just just tunneled into, adding minerals to player inventory if their were any
    void UpdateTiles()
    {
        Vector3Int gridTilePos = currentGrid.WorldToCell(endPos);
        if (playerController.GetTileMap("StoreBases").GetTile(gridTilePos)) return;

        // if drilled a mineral and in adult evolution, collect the mineral for the player
        if (mineralMap.GetTile(gridTilePos) && evolution == Evolutions.ADULT)
        {
            TileBase mineralTile = mineralMap.GetTile(gridTilePos);
            mineralMap.SetTile(gridTilePos, null);
            if (mineralTile.name == "Ancient Treasures")
            {
                missionControl.foundAncientTreasure = true; // trigger mission protocol transmission
                Mineral treasure = gameData.nameToMineralDict[mineralTile.name];
                playerInfo.money += treasure.cashValue;
                MoneyText.instance.SetMoneyValue(playerInfo.money);
            }
            if (mineralTile.name == "Ancient Tech")
            {
                playerInfo.ancientTech++;
                AncientTechText.instance.SetAncientTechValue(playerInfo.ancientTech);
            }
            else
            {
                string[] tokens = mineralTile.name.Split(' ');
                string mineralName = tokens[0];
                playerInventory.AddMineral(gameData.nameToMineralDict[mineralName]);
            }
        }

        // set the tunneled tile to a baby worm colored dirt tile if in adult evolution
        if (!playerDirtMap.GetTile(gridTilePos))
        {
            if (evolution == Evolutions.ADULT)
            {
                Tilemap groundMap = groundMapTracker.currentGroundMap;
                groundMap.SetTile(gridTilePos, null);

                Color dirtTileColor = playerController.CalculateDirtTileColor(baseDirtTileColor, gridTilePos.y);
                dirtMap.SetTile(gridTilePos, dirtTile);
                dirtMap.SetTileFlags(gridTilePos, TileFlags.None);
                dirtMap.SetColor(gridTilePos, dirtTileColor);
            }
        }
        updatedTiles = true;
    }

    // resets the tunnel timer to allow baby worm to tunnel
    void TunnelTimer()
    {
        isTunneling = false;
        animator.SetBool("isTunneling", false);
    }
    #endregion

    // manages first interaction with player and collding with worms in adult phase
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && evolution == Evolutions.UNDISCOVERED)
        {
            StartCoroutine(JourneyToSurface());
        }
        if (collision.CompareTag("Worm") && phase == Phase.HUNT_WORMS)
        {
            killedWorm = true;
        }
    }

    // starts the transition from undiscovered evolution to baby evolution
    IEnumerator JourneyToSurface()
    {
        evolution = Evolutions.BABY;
        phase = Phase.WAIT_FOR_PLAYER_COMMAND;

        // For Game Data storage
        currentEvolution = "BABY";
        onJourneyToSurface = true;

        GetComponent<Renderer>().enabled = false;

        // wait for player to reach surface
        yield return new WaitUntil(() => playerInfo.altitude == 0);
        phase = Phase.IDLE;
        transform.position = player.position;
        yield return new WaitForSeconds(.25f);
        missionControl.babyWormToSurface = true;
        GetComponent<Renderer>().enabled = true;
        onJourneyToSurface = false;
        yield break;
    }

    // matures babyworm
    IEnumerator Evolve()
    {
        yield return new WaitUntil(() => !playerInfo.UIActive);
        yield return new WaitForSeconds(.5f);
        missionControl.babyWormGrowsUp = true;
        CancelInvoke(nameof(TunnelTimer));
        Mature();
    }

    // grows baby worm in size, recalculates grounded position for when player is above ground,
    // increases tunnel rate and speed adn sets up baby worm to interact with game tilemaps
    public void Mature()
    {
        transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        groundedPosition += .4f;
        transform.position = new Vector3(player.position.x, groundedPosition, 0);
        dirtMap = playerController.GetTileMap("DirtMap");
        currentGrid = playerController.grid;
        gridMapScale = currentGrid.transform.localScale.x;
        tunnelingSpeed = playerController.drillSpeed;
        animator.SetFloat("speed", 4);
        InvokeRepeating(nameof(TunnelTimer), .01f, adultTunnelRate);
    }
}
