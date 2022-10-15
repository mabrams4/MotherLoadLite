using UnityEngine;

[CreateAssetMenu(fileName = "New Drill", menuName = "Drill")]
public class Drill : ScriptableObject
{
    public new string name;
    public float drillSpeed;
    public float cost;
    public Sprite sprite;
}
