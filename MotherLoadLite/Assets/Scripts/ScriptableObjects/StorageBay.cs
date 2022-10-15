using UnityEngine;

[CreateAssetMenu(fileName = "New StorageBay", menuName = "StorageBay")]
public class StorageBay : ScriptableObject
{
    public new string name;
    public float maxCapacity;
    public float cost;
    public Sprite sprite;
}
