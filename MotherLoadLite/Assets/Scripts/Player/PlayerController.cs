using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

/* this script manages player drilling and animations */
public class PlayerController : MonoBehaviour
{
    #region Singleton
    public static PlayerController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one PlayerController?");
            return;
        }
        instance = this;
    }
    #endregion

    // Instance objects
    MissionControl missionControl;
    PlayerInfo playerInfo;
    DataMineralItem gameData;
    Inventory inventory;
    MoneyText moneyText;
    AncientTechText ancientTechText;

    // Movement
    Rigidbody2D rb;
    public float horizontalMove;
    public float verticalMove;
    public float gravityScale;

    // Drilling parameters
    public float drillSpeed;
    public float minDrillSpeed;
    public bool isDrilling = false;
    bool hasStartedDrilling = false;
    
    public float scalingFactor;     // controls when fall off will start to happen
    public float power;   // controls how sharply drill speed falls off

    float startTime;
    float journeyLength;
    Vector3 drillingStartPos;
    Vector3 drillingEndPos;

    public float colorModifier;
    public Color baseDirtTileColor;
    public TileBase dirtTile;

    // Tilemaps and related things
    public Tilemap surfaceGroundMap;
    public Tilemap[] tilemaps;
    public Tilemap[] groundMaps;
    GroundMapTracker groundMapTracker;  // was public 
    public Grid grid;
    float gridMapScale;

    // Animation
    Animator animator;
    public Sprite[] dirtParticles;
    public GameObject dirtParticle;
    public GameObject topSpawnPoint;
    public GameObject bottomSpawnPoint;
    public GameObject leftSpawnPoint;
    private Vector3 dirtParticleSpawnPoint;
    public float maxDirtLaunchForce;
    public float dirtParticleSpawnRate;

    public GameObject collectMineralText;

    // Other
    public BMInventory BMinventory;
    public Item antiGravityDrill;
    PlayerAudio playerAudio;

    enum Direction
    {
        RIGHT,
        LEFT,
        UP,
        DOWN
    }
    Direction direction;

    void Start()
    {
        inventory = Inventory.instance;
        gameData = DataMineralItem.instance;
        playerInfo = PlayerInfo.instance;
        moneyText = MoneyText.instance;
        ancientTechText = AncientTechText.instance;
        missionControl = MissionControl.instance;
        playerAudio = GetComponent<PlayerAudio>();
        groundMapTracker = GetComponentInChildren<GroundMapTracker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gridMapScale = grid.transform.localScale.x;
        drillSpeed = inventory.currentDrill.drillSpeed;
    }

    void Update()
    {
        // need this info to determine drilling direction
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        if (!IsGrounded()) animator.SetBool("isFlying", true);
        else { animator.SetBool("isFlying", false); }

        if (isDrilling)
        {
            // set movement to 0 so that CanDrill() returns false and does not stop animating dirt coroutine
            horizontalMove = 0;
            verticalMove = 0;

            rb.gravityScale = 0;
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * CalculateDrillSpeed();
            UpdateDirtParticleSpawnPoint(distCovered);

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(drillingStartPos, drillingEndPos, fractionOfJourney);
        }

        // once finished drilling
        if (transform.position == drillingEndPos)
        {
            StopAllCoroutines();    // stop animating dirt particles
            UpdateTiles();
            
            isDrilling = false;
            hasStartedDrilling = false;
            rb.gravityScale = gravityScale;
            animator.SetBool("isDrilling", false);
            animator.SetBool("drillingSideways", false);
            animator.SetBool("drillingUp", false);
            animator.SetBool("drillingDown", false);
            playerAudio.StopDrillSound();
        }

        // Open Inventory shortcut
        if (Input.GetKeyDown(KeyCode.I)) BMinventory.OpenInventory();
    }

    // If in contact with the ground and can drill, start drilling
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bounds")) return;
        if (CanDrill() && !hasStartedDrilling)
        {
            hasStartedDrilling = true;
            CalculateDrillParameters();
        }
    }

    // stops the explosion animation from repeating
    public void SetExplosionOver()
    {
        animator.SetBool("explosionOver", true);
        animator.SetBool("died", false);
    }

    // keeps the dirt particles spawning at the tip of the drill as the player moves
    void UpdateDirtParticleSpawnPoint(float distCovered)
    {
        switch (direction)
        {
            case (Direction.RIGHT):
                dirtParticleSpawnPoint = new (leftSpawnPoint.transform.position.x + distCovered / 2,
                                              leftSpawnPoint.transform.position.y,
                                              leftSpawnPoint.transform.position.z);
                break;
            case (Direction.LEFT):
                dirtParticleSpawnPoint = new(leftSpawnPoint.transform.position.x - distCovered / 2,
                                              leftSpawnPoint.transform.position.y,
                                              leftSpawnPoint.transform.position.z);
                break;
            case (Direction.DOWN):
                dirtParticleSpawnPoint = new(bottomSpawnPoint.transform.position.x,
                                              bottomSpawnPoint.transform.position.y - distCovered / 2,
                                              bottomSpawnPoint.transform.position.z);
                break;
            case (Direction.UP):
                dirtParticleSpawnPoint = new(topSpawnPoint.transform.position.x,
                                              topSpawnPoint.transform.position.y + distCovered / 2,
                                              topSpawnPoint.transform.position.z);
                break;
        }
    }

    // animates dirt particles from the drill point while the player is drilling
    IEnumerator AnimateDirt()
    {
        playerInfo.UseFuel();
        while (true)
        {
            int spriteIndex = Random.Range(0, dirtParticles.Length);
            Sprite sprite = dirtParticles[spriteIndex];
            GameObject dp = Instantiate(dirtParticle, dirtParticleSpawnPoint,
                                        new Quaternion(0, 0, Random.Range(0, 360), 0));

            dp.GetComponent<SpriteRenderer>().sprite = sprite;
            Vector2 dirtLaunchForce = new(Random.Range(-maxDirtLaunchForce, maxDirtLaunchForce),
                    Random.Range(-maxDirtLaunchForce, maxDirtLaunchForce));
            dp.GetComponent<DirtParticle>().ApplyForce(dirtLaunchForce);
            dp.GetComponent<DirtParticle>().isActive = true;
            yield return new WaitForSeconds(dirtParticleSpawnRate);
        }

    }

    // calculates the drilling speed based on the players current drill speed and the current depth
    float CalculateDrillSpeed()
    {
        float speed = drillSpeed - Mathf.Pow(((Mathf.Abs(transform.position.y) / scalingFactor)), power);
        return Mathf.Max(speed, minDrillSpeed);
    }

    // Calculated start and end positions for the Lerp function called in Update for drilling
    void CalculateDrillParameters()
    {
        startTime = Time.time;
        drillingStartPos = transform.position;
        Vector3Int tilePos = grid.WorldToCell(CalculateDrillingDestinationWorldCoordinates());
        drillingEndPos = grid.GetCellCenterWorld(tilePos);

        journeyLength = Vector3.Distance(drillingStartPos, drillingEndPos);
        isDrilling = true;
        animator.SetBool("isDrilling", true);
    }

    // calculates the center of the current tile being drilled and returns its position
    Vector3 CalculateDrillingDestinationWorldCoordinates()
    {
        Vector3 offset = new Vector3(0, 0, 0);

        if (horizontalMove > 0)
        {
            offset.x += gridMapScale;
            dirtParticleSpawnPoint = leftSpawnPoint.transform.position;
        }
        else if (horizontalMove < 0)
        {
            offset.x -= gridMapScale;
            dirtParticleSpawnPoint = leftSpawnPoint.transform.position;
        }
        else if (verticalMove < 0)
        {
            offset.y -= gridMapScale;
            dirtParticleSpawnPoint = bottomSpawnPoint.transform.position;
        }
        else if (verticalMove > 0)
        {
            offset.y += gridMapScale;
            dirtParticleSpawnPoint = topSpawnPoint.transform.position;
        }
        return transform.position + offset;
    }

    // checks series of conditions to see if player can drill in their current state
    bool CanDrill()
    {
        if (horizontalMove == 0 && verticalMove == 0) return false;
        
        // cant drill directly under where the stores/shops are
        Tilemap storeBases = GetTileMap("StoreBases");
        if (storeBases.GetTile(grid.WorldToCell(CalculateDrillingDestinationWorldCoordinates()))) return false;

        Tilemap currentGroundMap = groundMapTracker.currentGroundMap;
        Tilemap previousGroundMap = groundMapTracker.previousGroundMap;

        Vector3Int currentGroundMapTilePos = currentGroundMap.WorldToCell(CalculateDrillingDestinationWorldCoordinates());
        Vector3Int previousGroundMapTilePos = previousGroundMap.WorldToCell(CalculateDrillingDestinationWorldCoordinates());

        StopAllCoroutines();    // stop animating dirt particles 

        // conditions to drill down
        if (IsGrounded() && verticalMove < 0 && horizontalMove == 0)
        {
            direction = Direction.DOWN;
            animator.SetBool("drillingDown", true);
            StartCoroutine(AnimateDirt());
            return true;
        }

        // conditions to drill sideways
        if ((horizontalMove > 0 || horizontalMove < 0) && IsGrounded()
            && IsUnderground() && verticalMove == 0
            && (currentGroundMap.GetTile(currentGroundMapTilePos)
                || previousGroundMap.GetTile(previousGroundMapTilePos)))
        {
            if (horizontalMove > 0) direction = Direction.RIGHT;
            else { direction = Direction.LEFT; }
            animator.SetBool("drillingSideways", true);
            StartCoroutine(AnimateDirt());
            return true;
        }

        // can drill in any direction with anti-gravity drill
        if (inventory.specialUpgrades.Contains(antiGravityDrill) && IsUnderground()
            && (currentGroundMap.GetTile(currentGroundMapTilePos)
                || previousGroundMap.GetTile(previousGroundMapTilePos)))
        {
            if (rb.velocity.y > .01) return false; // buggy if not here

            if (horizontalMove > 0 || horizontalMove < 0)
            {
                if (horizontalMove > 0) direction = Direction.RIGHT;
                else { direction = Direction.LEFT; }
                animator.SetBool("drillingSideways", true);

            }
            if (verticalMove > 0)
            {
                direction = Direction.UP;
                animator.SetBool("drillingUp", true);
            }
            StartCoroutine(AnimateDirt());
            return true;

        }
        return false;
    }

    // update the tiles for where player just drilled
    void UpdateTiles()
    {
        Tilemap currentGroundMap = groundMapTracker.currentGroundMap;
        Tilemap previousGroundMap = groundMapTracker.previousGroundMap;

        Tilemap dirtMap = GetTileMap("DirtMap");
        Tilemap mineralMap = GetTileMap("MineralMap");

        Vector3Int currentGroundMapTilePos = currentGroundMap.WorldToCell(drillingEndPos);
        Vector3Int previousGroundMapTilePos = previousGroundMap.WorldToCell(drillingEndPos);

        Vector3Int gridTilePos = grid.WorldToCell(drillingEndPos);

        // if drilled a mineral, collect the mineral
        if (mineralMap.GetTile(gridTilePos))
        {
            TileBase mineralTile = mineralMap.GetTile(gridTilePos);
            mineralMap.SetTile(gridTilePos, null);

            // ancient treasures are immediate cash value
            if (mineralTile.name == "Ancient Treasure")
            {
                missionControl.foundAncientTreasure = true; // trigger mission protocol transmission if first time
                Mineral treasure = gameData.nameToMineralDict[mineralTile.name];
                playerInfo.money += treasure.cashValue;
                moneyText.SetMoneyValue(playerInfo.money);
                DoCollectMineralAnimation(mineralTile.name);
            }

            // Ancient Tech is a seperate resource 
            else if (mineralTile.name == "Ancient Tech")
            {
                playerInfo.ancientTech++;
                ancientTechText.SetAncientTechValue(playerInfo.ancientTech);
                DoCollectMineralAnimation(mineralTile.name);
            }

            // drilled a standard mineral
            else
            {
                // most minerals have 4 names for each orientaion they can be found in i.e "Gold 1" "Gold 2" etc.
                // however they are stored in the name->mineral dictionary as just the mineral name i.e "Gold"
                string mineralName = mineralTile.name;
                string[] tokens = mineralName.Split(' ');   
                mineralName = tokens[0];
                inventory.AddMineral(gameData.nameToMineralDict[mineralName]);
                DoCollectMineralAnimation(mineralName);
            }
        }

        // remove ground tile to get rid of collider, then replace with dirt tile
        // have to check both ground Maps for case of drilling at intersection of two ground map chunks
        if (currentGroundMap.GetTile(currentGroundMapTilePos))
            currentGroundMap.SetTile(currentGroundMapTilePos, null);

        else if (previousGroundMap.GetTile(previousGroundMapTilePos))
            previousGroundMap.SetTile(previousGroundMapTilePos, null);

        Color dirtTileColor = CalculateDirtTileColor(baseDirtTileColor, gridTilePos.y);
        dirtMap.SetTile(gridTilePos, dirtTile);
        dirtMap.SetTileFlags(gridTilePos, TileFlags.None);
        dirtMap.SetColor(gridTilePos, dirtTileColor);
    }

    // animates text for collecting minerals or inventory being full
    void DoCollectMineralAnimation(string mineralName)
    {
        GameObject text = Instantiate(collectMineralText, transform.position, Quaternion.identity);
        if (inventory.currentCapacity < inventory.maxCapacity)
        {
            text.GetComponentInChildren<TextMesh>().text = "+1 " + mineralName;
            playerAudio.PlayCollectMineralSound();
        }
        else 
        {
            text.GetComponentInChildren<TextMesh>().text = "Inventory full";
        }
    }

    // calculates the correct dirt tile based on a base color and the depth of the tile
    public Color CalculateDirtTileColor(Color baseDirtTileColor, float yPos)
    {
        yPos -= (1 + grid.transform.localScale.y);
        yPos *= 3;
        float redColorOffset = -yPos / colorModifier;
        float greenColorOffset = -(yPos + 500) / colorModifier;
        float blueColorOffset = -(yPos + 1000) / colorModifier;
        float secondRedColorOffset = -(yPos + 1500) / colorModifier;

        Color dirtTileColor = new();
        switch (yPos)
        {
            case (< -1500):
                dirtTileColor = new(1 - secondRedColorOffset, baseDirtTileColor.g - greenColorOffset,
                             baseDirtTileColor.b + blueColorOffset, baseDirtTileColor.a);
                break;
            case (< -1000):
                dirtTileColor = new(baseDirtTileColor.r + redColorOffset, baseDirtTileColor.g - greenColorOffset,
                             baseDirtTileColor.b + blueColorOffset, baseDirtTileColor.a);
                break;
            case (< -500):
                dirtTileColor = new(baseDirtTileColor.r + redColorOffset, baseDirtTileColor.g - greenColorOffset,
                             baseDirtTileColor.b, baseDirtTileColor.a);
                break;
            default:    // first 500 ft
                dirtTileColor = new(baseDirtTileColor.r + redColorOffset, baseDirtTileColor.g,
                             baseDirtTileColor.b, baseDirtTileColor.a);
                break;
        }
        return dirtTileColor;
    }

    // returns the tilemap associated with the passed in key name
    public Tilemap GetTileMap(string name)
    {
        foreach (Tilemap map in tilemaps)
        {
            if (map.name == name) return map;
        }
        return null;
    }

    // checks if player is currently underground
    public bool IsUnderground()
    {
        Tilemap skyMap = GetTileMap("SkyMap");
        Vector3Int tilePos = skyMap.WorldToCell(transform.position);
        if (skyMap.GetTile(tilePos) == null) return true;
        return false;
    }

    // checks if player is in contact with the ground
    public bool IsGrounded()
    {
        Tilemap currentGroundMap = groundMapTracker.currentGroundMap;
        Tilemap previousGroundMap = groundMapTracker.previousGroundMap;

        Vector3 playerPos = transform.position;
        playerPos.y -= gridMapScale;
        Vector3Int currentGroundMapTilePos = currentGroundMap.WorldToCell(playerPos);
        Vector3Int previousGroundMapTilePos = previousGroundMap.WorldToCell(playerPos);

        if (currentGroundMap.GetTile(currentGroundMapTilePos) ||
            previousGroundMap.GetTile(previousGroundMapTilePos)) return true;
        return false;
    }
}
