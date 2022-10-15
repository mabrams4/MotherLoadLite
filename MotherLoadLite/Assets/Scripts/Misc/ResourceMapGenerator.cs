using UnityEngine;
using UnityEngine.Tilemaps;

/* this script is responsible for randomly spawning minerals and drilled dirt patches into the game */

public class ResourceMapGenerator : MonoBehaviour
{
    public Mineral[] minerals;  // minerals to spawn
    public float numSections;

    // manually calculated for the game
    float maxDepth = -701;
    float minDepth = 0;
    float maxWidth = 46;
    float minWidth = -53;

    PlayerController player;
    public int maxNumDirtTiles;

    Tilemap mineralMap;
    Tilemap skyMap;
    Tilemap dirtMap;
    Tilemap storeBases;
    Tilemap[] groundMaps;

    void Start()
    {
        player = PlayerController.instance;
        groundMaps = player.groundMaps;
        skyMap = player.GetTileMap("SkyMap");
        dirtMap = player.GetTileMap("DirtMap");
        storeBases = player.GetTileMap("StoreBases");
        mineralMap = GetComponent<Tilemap>();

        foreach (Mineral m in minerals) SpawnMineral(m);
        for (int i = 0; i > maxDepth; i--) SpawnDirtPatch(i);
    }

    // Spawns a mineral into the mineral map
    void SpawnMineral(Mineral m)
    {
        for (int i = 0; i < m.numSpawns; i++)
        {
            float xPos = Random.Range(minWidth, maxWidth);
            float yPos = CalculateYPos(m);
            Vector3 pos = new Vector3(xPos, yPos, 0);
            if (mineralMap.GetTile(Vector3Int.FloorToInt(pos)) == null
                && storeBases.GetTile(Vector3Int.FloorToInt(pos)) == null)
            {
                int index = Random.Range(0, 4);
                mineralMap.SetTile(Vector3Int.FloorToInt(pos), m.tiles[index]);
            }
        }
    }

    // calculates random y position based on mineral properties
    float CalculateYPos(Mineral m)
    {
        float yPos;
        float p = Random.Range(0f, 1f);
        float firstFifth = maxDepth / numSections;
        float secondFifth = 2 * firstFifth;
        float thirdFifth = 3 * firstFifth;
        float fourthFifth = 4 * firstFifth;

        // y position is in some range of a certain game section based on random chance
        if (p < m.firstFifthSpawnchance) yPos = Random.Range(minDepth, firstFifth);
        else if (p < m.secondFifthSpawnchance) yPos = Random.Range(firstFifth, secondFifth);
        else if (p < m.thirdFifthSpawnchance) yPos = Random.Range(secondFifth, thirdFifth);
        else if (p < m.fourthFifthSpawnchance) yPos = Random.Range(thirdFifth, fourthFifth);
        else { yPos = Random.Range(fourthFifth, maxDepth); }

        return yPos;
    }

    // Spawns a randomly oriented patch of drilled dirt
    void SpawnDirtPatch(int yPos)
    {

        float xPos = Random.Range(minWidth, maxWidth + 1);
        Vector3Int tilePos = Vector3Int.FloorToInt(new(xPos, yPos, 0));

        int count = 0;
        float p = 1;
        while (p > .35)
        {
            if (count == maxNumDirtTiles) break;

            if (!mineralMap.GetTile(tilePos) && !storeBases.GetTile(tilePos) && !skyMap.GetTile(tilePos))
            {
                foreach (Tilemap groundMap in groundMaps)
                {
                    Vector3Int groundMapTilePos = DirtMapTileToGroundMapTile(groundMap, tilePos);
                    if (groundMap.GetTile(groundMapTilePos))
                    {
                        groundMap.SetTile(groundMapTilePos, null);
                        break;
                    }
                }
                Color dirtTileColor = player.CalculateDirtTileColor(player.baseDirtTileColor, tilePos.y);
                dirtMap.SetTile(tilePos, player.dirtTile);
                dirtMap.SetTileFlags(tilePos, TileFlags.None);
                dirtMap.SetColor(tilePos, dirtTileColor);
            }
            tilePos = GenerateNextTilePosition(tilePos);
            p = Random.Range(0f, 1f);
            maxNumDirtTiles++;
        }
    }

    // randomly chooses the next position of the next drilled dirt tile
    Vector3Int GenerateNextTilePosition(Vector3Int tilePos)
    {
        int x = Random.Range(0, 4);
        switch (x)
        {
            case 0:
                tilePos.x++;
                break;
            case 1:
                tilePos.x--;
                break;
            case 2:
                tilePos.y++;
                break;
            case 3:
                tilePos.y--;
                break;

        }
        return tilePos;
    }

    // converts a tile position in the dirt map to a tile position in the given ground map
    Vector3Int DirtMapTileToGroundMapTile(Tilemap groundMap, Vector3Int tilePos)
    {
        float yOffset = groundMap.gameObject.GetComponentInParent<Transform>().position.y
            / player.grid.transform.localScale.y;
        Vector3Int offsetTilePos = new Vector3Int(tilePos.x, tilePos.y - Mathf.FloorToInt(yOffset), tilePos.z);
        return offsetTilePos;
    }
}
