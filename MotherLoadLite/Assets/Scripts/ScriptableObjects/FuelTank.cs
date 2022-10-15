using UnityEngine;

[CreateAssetMenu(fileName = "New FuelTank", menuName = "FuelTank")]
public class FuelTank : ScriptableObject
{
    public new string name;
    public float maxFuel;
    public float cost;
    public Sprite sprite;
}
