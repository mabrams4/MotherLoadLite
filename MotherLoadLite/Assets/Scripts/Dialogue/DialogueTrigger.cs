using UnityEngine;

/* this script is responsible for triggering dialogues */
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = DialogueManager.instance;
    }

    // triggers the dialogue stored in this instance
    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }
}
