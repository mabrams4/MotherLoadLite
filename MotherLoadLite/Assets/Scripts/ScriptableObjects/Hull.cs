using UnityEngine;

[CreateAssetMenu(fileName = "New Hull", menuName = "Hull")]
public class Hull : ScriptableObject
{
    public new string name;
    public float maxHealth;
    public float cost;
    public Sprite sprite;
}