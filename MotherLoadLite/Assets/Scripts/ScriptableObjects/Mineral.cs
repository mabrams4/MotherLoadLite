using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Mineral", menuName = "Mineral")]
public class Mineral : ScriptableObject
{
    public new string name;
    public int cashValue;

    public int numSpawns;

    public float firstFifthSpawnchance;
    public float secondFifthSpawnchance;
    public float thirdFifthSpawnchance;
    public float fourthFifthSpawnchance;

    public TileBase[] tiles;
}
