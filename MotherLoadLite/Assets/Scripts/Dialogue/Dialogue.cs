using UnityEngine;

[System.Serializable]

/* Defines the dialogue class that is used for game dialogue */
public class Dialogue
{
    public string trigger;
    public Sprite speaker;
    public string name;

    [TextArea(3, 10)]
    public string[] senetences;

}
