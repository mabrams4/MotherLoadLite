using UnityEngine;

/* this script acts as the Misson Control character of the game and is reponsible for triggering
 * all dialogues related to itself once certain conditions are met. Also contains bools to keep track of
 * what dialogues have already happened for saving/loading the game.
 */
public class MissionControl : MonoBehaviour
{
    #region Singleton
    public static MissionControl instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one MissionControl?");
            return;
        }
        instance = this;
    }
    #endregion

    DialogueTrigger[] dialogues;
    PlayerInfo playerInfo;
    
    public float firstTransmissionDepth;
    public bool givenFirstMissionControlTransmission;

    public bool foundAncientTreasure;
    public bool givenAncientTreasureTransmission;


    public bool babyWormToSurface;
    public bool givenBabyWormToSurfaceTransmission;

    public bool babyWormGrowsUp;
    public bool givenBabyWormGrowsUpTransmission;


    void Start()
    {
        playerInfo = PlayerInfo.instance;
        dialogues = GetComponentsInChildren<DialogueTrigger>();
        
        // New Game Load Transmission
        if (SaveSystem.isNewGame) StartDialogue("New Game");
    }

    // triggers dialogues when certain gsame conditions are met
    void Update()
    {
        if (playerInfo.altitude == -firstTransmissionDepth && !givenFirstMissionControlTransmission)
        {
            StartDialogue("First Mission Control Transmission");
            givenFirstMissionControlTransmission = true;
        }
        
        if (foundAncientTreasure && !givenAncientTreasureTransmission)
        {
            StartDialogue("Found Ancient Treasure");
            givenAncientTreasureTransmission = true;
        }

        if (babyWormToSurface && !givenBabyWormToSurfaceTransmission)
        {
             StartDialogue("Baby Worm To Surface");
            givenBabyWormToSurfaceTransmission = true;
        }


        if (babyWormGrowsUp && !givenBabyWormGrowsUpTransmission)
        {
            StartDialogue("Baby Worm Grows Up");
            givenBabyWormGrowsUpTransmission = true;
        }
    }

    // starts a dialogue based on the dialogue trigger
    void StartDialogue(string trigger)
    {
        foreach (DialogueTrigger dt in dialogues) if (dt.dialogue.trigger == trigger) dt.TriggerDialogue();
    }
}
