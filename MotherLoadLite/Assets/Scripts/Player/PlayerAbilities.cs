using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

/* this script manages all player abilities */
public class PlayerAbilities : MonoBehaviour
{
    DataMineralItem gameData;
    Inventory inventory;
    PlayerInfo playerInfo;
    PlayerController playerController;

    GroundMapTracker groundMapTracker;
    public Tilemap surfaceGroundMap;
    public GameObject blackScreen;

    public GameObject minePrefab;
    public Item teleporter;
    public float dynamiteExplosionRadius;
    public float dynamiteDamage;
    public float AGBExplosionRadius;
    public float AGBDamage;
    public float repairKitHealAmount;

    PlayerAudio playerAudio;

    void Start()
    {
        gameData = DataMineralItem.instance;
        inventory = Inventory.instance;
        playerInfo = GetComponent<PlayerInfo>();
        playerController = GetComponent<PlayerController>();
        groundMapTracker = GetComponentInChildren<GroundMapTracker>();
        playerAudio = GetComponent<PlayerAudio>();
    }

    void Update()
    {
        // Mine ability
        if (Input.GetButtonDown("LayMine"))
        {
            Item mine = gameData.nameToItemDict["Mine"];
            if (inventory.items.ContainsKey(mine) && inventory.items[mine] > 0)
            {
                Vector3 minePos = transform.position;
                minePos.y -= transform.localScale.y / 2;
                Instantiate(minePrefab, minePos, Quaternion.identity);
                inventory.UseItem(mine);
            }
        }

        // Dynamite ability
        if (Input.GetButtonDown("UseDynamite"))
        {
            Item dynamite = gameData.nameToItemDict["Dynamite"];
            if (inventory.items.ContainsKey(dynamite) && inventory.items[dynamite] > 0)
            {
                UseExplosive(dynamiteExplosionRadius, dynamiteDamage);
                inventory.UseItem(dynamite);
            }
        }

        // Anti-Matter Bomb ability
        if (Input.GetButtonDown("UseAMB"))
        {
            Item antiGravityBomb = gameData.nameToItemDict["Anti-Matter Bomb"];
            if (inventory.items.ContainsKey(antiGravityBomb) && inventory.items[antiGravityBomb] > 0)
            {
                UseExplosive(AGBExplosionRadius, AGBDamage);
                inventory.UseItem(antiGravityBomb);
            }
        }

        // Teleport ability
        if (Input.GetButtonDown("Teleport") && inventory.specialUpgrades.Contains(teleporter))
        {
            Item teleport = gameData.nameToItemDict["Teleport"];
            if (inventory.items.ContainsKey(teleport) && inventory.items[teleport] > 0)
            {
                StartCoroutine(TeleportToSurface());
                inventory.UseItem(teleport);
            }
        }

        // Repair Kit
        if (Input.GetButtonDown("UseRepairKit"))
        {
            if (playerInfo.currentHealth >= playerInfo.maxHealth) return;
            Item repairKit = gameData.nameToItemDict["Repair Kit"];
            if (inventory.items.ContainsKey(repairKit) && inventory.items[repairKit] > 0)
            {
                playerInfo.currentHealth = Mathf.Min(playerInfo.maxHealth,
                                    playerInfo.currentHealth + repairKitHealAmount);
                HealthBar.instance.SetHealth(playerInfo.currentHealth);
                inventory.UseItem(repairKit);
            }
        }
    }

    // teleports player to surface
    IEnumerator TeleportToSurface()
    {
        playerAudio.PlayTeleportSound();
        if (groundMapTracker.currentGroundMap != surfaceGroundMap) blackScreen.SetActive(true);
        transform.position = new Vector3(0, 10, 0);
        yield return new WaitForSeconds(1f);
        blackScreen.SetActive(false);
        groundMapTracker.currentGroundMap = surfaceGroundMap;
    }

    void UseExplosive(float explosionRadius, float damage)
    {
        playerAudio.PlayExplosiveSound();
        Tilemap mineralMap = playerController.GetTileMap("MineralMap");
        Tilemap dirtMap = playerController.GetTileMap("DirtMap");
        Tilemap skyMap = playerController.GetTileMap("SkyMap");

        // damage worms in range
        Vector2 pos = new(transform.position.x, transform.position.y);
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, explosionRadius, ~0);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Worm")) hit.GetComponent<WormController>().ApplyDamage(damage);
            if (hit.CompareTag("WormNest")) hit.GetComponent<WormNest>().ApplyDamage(damage);
        }

        // destroy minerals/ground tiles by looping through a grid of all tiles within the explosion radius
        for (int i = 0; i < explosionRadius; i++)
        {
            for (int j = 0; j < explosionRadius; j++)
            {
                float xpos = transform.position.x - (explosionRadius / 2) + i;
                float ypos = transform.position.y - (explosionRadius / 2) + j;
                Vector3Int tilePos = playerController.grid.WorldToCell(new(xpos, ypos, 0));

                if (skyMap.GetTile(tilePos)) continue;

                // loop through all groundMaps in case explosion covers 2 ground chunks
                foreach (Tilemap groundMap in playerController.groundMaps)
                {
                    Vector3Int groundMapTilePos = groundMap.WorldToCell(new(xpos, ypos, 0));
                    if (groundMap.GetTile(groundMapTilePos)) groundMap.SetTile(groundMapTilePos, null);
                }

                if (mineralMap.GetTile(tilePos)) mineralMap.SetTile(tilePos, null);

                Color dirtTileColor = playerController.CalculateDirtTileColor(playerController.baseDirtTileColor, tilePos.y);
                dirtMap.SetTile(tilePos, playerController.dirtTile);
                dirtMap.SetTileFlags(tilePos, TileFlags.None);
                dirtMap.SetColor(tilePos, dirtTileColor);
            }
        }
    }
}
