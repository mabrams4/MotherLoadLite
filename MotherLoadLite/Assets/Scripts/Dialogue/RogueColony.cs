using UnityEngine;

/* this script acts as the Rogue Colony character of the game and is reponsible for triggering
 * all dialogues related to itself once certain conditions are met. Also contains bools to keep track of
 * what dialogues have already happened for saving/loading the game.
 */
public class RogueColony : MonoBehaviour
{
    #region Singleton
    public static RogueColony instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one RogueColony?");
            return;
        }
        instance = this;
    }
    #endregion
    DialogueTrigger[] dialogues;
    PlayerInfo playerInfo;

    public float firstTransmissionDepth;
    public bool givenFirstRogueColonyTransmission;

    public bool encounteredWorm;
    public bool givenEncounterWormTransmission;

    public bool foundBabyWorm;
    public bool givenFoundBabyWormTransmission;

    public bool encounteredNest;
    public bool givenEncounterNestTransmission;

    public bool blownUpQueenNest;
    public bool givenBlownUpQueenNestTransmission;

    void Start()
    {
        playerInfo = PlayerInfo.instance;
        dialogues = GetComponentsInChildren<DialogueTrigger>();
    }

    // triggers dialogues when certain gsame conditions are met
    void Update()
    {
        if (playerInfo.altitude == -firstTransmissionDepth && !givenFirstRogueColonyTransmission)
        {
            StartDialogue("First Rouge Colony Transmission");
            givenFirstRogueColonyTransmission = true;
        }

        if (encounteredWorm && !givenEncounterWormTransmission)
        {
            StartDialogue("Encounter Worm");
            givenEncounterWormTransmission = true;
        }

        if (encounteredNest && !givenEncounterNestTransmission)
        {
            StartDialogue("Encounter Worm Nest");
            givenEncounterNestTransmission = true;
        }

        if (foundBabyWorm && !givenFoundBabyWormTransmission)
        {
            StartDialogue("Found Baby Worm");
            givenFoundBabyWormTransmission = true;
        }

        if (blownUpQueenNest && !givenBlownUpQueenNestTransmission)
        {
            StartDialogue("Blow Up Queen Nest");
            givenBlownUpQueenNestTransmission = true;
        }
    }

    // starts a dialogue based on the dialogue trigger
    void StartDialogue(string trigger)
    {
        foreach (DialogueTrigger dt in dialogues) if (dt.dialogue.trigger == trigger) dt.TriggerDialogue();
    }
}
