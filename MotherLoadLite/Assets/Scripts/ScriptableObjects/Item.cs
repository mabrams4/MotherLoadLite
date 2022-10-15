using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public new string name;

    [TextArea(3,10)]
    public string description;
    public float cost;

    public Sprite sprite;
}
