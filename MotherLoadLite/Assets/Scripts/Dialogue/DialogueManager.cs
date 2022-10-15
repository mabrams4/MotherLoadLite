using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* This script handles how all dialogue events play out */

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Dialogue Manager?");
            return;
        }
        instance = this;
    }
    #endregion

    public Dialogue currentDialogue;
    public TMPro.TextMeshProUGUI dialogueText;
    public TMPro.TextMeshProUGUI nameText;
    public Image image;
    public GameObject dialogueUI;
    public GameObject researchStation;

    private Queue<string> sentences = new();

    PlayerInfo playerInfo;
    MoneyText moneyText;
    AncientTechText ancientTechText;

    float originalZoom;
    public float maxZoomOut;
    public float zoomOutSpeed;
    float numSentences;
    bool finishedTyping;
    string currentSentence;

    private void Start()
    {
        playerInfo = PlayerInfo.instance;
        moneyText = MoneyText.instance;
        ancientTechText = AncientTechText.instance;
        originalZoom = Camera.main.orthographicSize;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dialogueUI.activeInHierarchy) EndDialogue(); // skip the current dialogue
    }

    // Begins a dialogue given a Dialogue object
    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;     // keeps track of current dialogue
        numSentences = dialogue.senetences.Length;
        Time.timeScale = 0f;

        dialogueUI.SetActive(true);
        sentences.Clear();
        image.sprite = dialogue.speaker;
        nameText.text = dialogue.name;

        foreach (string sentence in dialogue.senetences)
        {
            sentences.Enqueue(sentence);    
        }
        DisplayNextSentence();
    }

    // if dialogue is still being typed out, displays the full dialogue. If full dialogue is already displayed,
    // continues to the next sentence in the dialogue
    public void Continue()
    {
        if (sentences.Count == 0)
        {
            if (!finishedTyping) FinishTyping();
            else { EndDialogue(); }
        }
        else if (!finishedTyping)
        {
            FinishTyping();
        }
        else { DisplayNextSentence(); }
    }

    // displays the full current sentence of the current dialogue
    void FinishTyping()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialogueText.text = currentSentence;
        finishedTyping = true;
    }

    // displays the next sentence in the dialogue
    public void DisplayNextSentence()
    {
        StopAllCoroutines();

        // certain dialogues include a camera zoom in and out
        if (currentDialogue.trigger == "Encounter Worm" || currentDialogue.trigger == "Encounter Worm Nest"
            || currentDialogue.trigger == "Found Baby Worm" && IsFirstSentence())
        {
            Time.timeScale = 1f;
            StartCoroutine(ZoomOut());
        }
        StartCoroutine(TypeSentence());
    }

    // returns true if the current dialogue is on the first sentence
    bool IsFirstSentence()
    {
        return (numSentences == currentDialogue.senetences.Length);
    }

    // Animates out typing a sentence one character at a time
    IEnumerator TypeSentence()
    {
        finishedTyping = false;
        string sentence = sentences.Dequeue();
        currentSentence = sentence;

        dialogueText.text = "";

        foreach (char c in sentence.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(.01f);
        }
        finishedTyping = true;
    }

    // Zooms the camera back in to the original game zoom
    IEnumerator ZoomIn()
    {
        float currentZoom = Camera.main.orthographicSize;
        while (currentZoom > originalZoom)
        {
            Camera.main.orthographicSize -= zoomOutSpeed;
            currentZoom -= zoomOutSpeed;
            yield return null;
        }
    }

    // Zooms camera out to maxZoomOut
    IEnumerator ZoomOut()
    {
        float currentZoom = Camera.main.orthographicSize;
        while (currentZoom < maxZoomOut)
        {
            Camera.main.orthographicSize += zoomOutSpeed;
            currentZoom += zoomOutSpeed;
            yield return null;
        }
    }

    // Ends the current dialogue, taking care of all post dialogue events
    public void EndDialogue()
    {
        dialogueUI.SetActive(false);
        Time.timeScale = 1f;

        if (currentDialogue.trigger == "First Mission Control Transmission")
        {
            playerInfo.money += 1000;
            moneyText.SetMoneyValue(playerInfo.money);
        }

        if (currentDialogue.trigger == "First Rouge Colony Transmission")
        {
            playerInfo.ancientTech += 1;
            ancientTechText.SetAncientTechValue(playerInfo.ancientTech);
            researchStation.SetActive(true);
        }

        if (currentDialogue.trigger == "Encounter Worm" || currentDialogue.trigger == "Encounter Worm Nest"
            || currentDialogue.trigger == "Found Baby Worm")
        {
            if (currentDialogue.trigger == "Encounter Worm") UpdateAncientTech(3);
            if (currentDialogue.trigger == "Encounter Worm Nest") UpdateAncientTech(5);

            StopAllCoroutines();
            StartCoroutine(ZoomIn());
        }

        if (currentDialogue.trigger == "Blow Up Queen Nest") SceneManager.LoadScene(2);
    }

    // Update players ancient tech amount
    void UpdateAncientTech(float amount)
    {
        playerInfo.ancientTech += amount;
        ancientTechText.SetAncientTechValue(playerInfo.ancientTech);
    }
}
