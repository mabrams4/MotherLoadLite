using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;


/* This script controls mines placed by the player */

public class Mine : MonoBehaviour
{
    public float explosionRadius;
    public float timer;
    public float damage;
    PlayerController player;
    Tilemap[] groundMaps;

    void Start()
    {
        player = PlayerController.instance;
        groundMaps = player.groundMaps;
        StartCoroutine(Explode());
    }

    // after a short delay destroys all ground and mineral tiles and damages any worms/worm nests
    // within the explosion radius 
    IEnumerator Explode()
    {
        Tilemap mineralMap = player.GetTileMap("MineralMap");
        Tilemap dirtMap = player.GetTileMap("DirtMap");
        Tilemap skyMap = player.GetTileMap("SkyMap");

        yield return new WaitForSeconds(timer);
        player.GetComponent<PlayerAudio>().PlayExplosiveSound();

        Vector2 pos = new(transform.position.x, transform.position.y);
        
        // get all colliders within explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, explosionRadius, ~0);
        foreach (Collider2D hit in hits)
        {
            if (hit.tag == "Worm") hit.GetComponent<WormController>().ApplyDamage(damage);
        }

        // destroy minerals/ground tiles by looping through a grid of all tiles within the explosion radius
        for (int i = 0; i < explosionRadius; i++)
        {
            for (int j = 0; j < explosionRadius; j++)
            {
                float xpos = transform.position.x - (explosionRadius / 2) + i;
                float ypos = transform.position.y - (explosionRadius / 2) + j;
                Vector3Int tilePos = player.grid.WorldToCell(new Vector3(xpos, ypos, 0));

                if (skyMap.GetTile(tilePos)) continue;  // dont update sky tiles

                // loop through all groundMaps in case explosion covers 2 seperate ground chunks
                foreach (Tilemap groundMap in groundMaps)
                {
                    Vector3Int groundMapTilePos = groundMap.WorldToCell(new(xpos, ypos, 0));
                    if (groundMap.GetTile(groundMapTilePos)) groundMap.SetTile(groundMapTilePos, null);
                }
                if (mineralMap.GetTile(tilePos)) mineralMap.SetTile(tilePos, null);
                dirtMap.SetTile(tilePos, player.dirtTile);
                dirtMap.SetTileFlags(tilePos, TileFlags.None);
                Color dirtTileColor = player.CalculateDirtTileColor(player.baseDirtTileColor, tilePos.y);
                dirtMap.SetColor(tilePos, dirtTileColor);

            }
        }
        Destroy(gameObject);
    }
}
