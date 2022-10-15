using System.Collections;
using UnityEngine;

/* this script deals with all buttons related to the research station */

public class BMResearchStation : MonoBehaviour
{
    Inventory inventory;
    PlayerInfo playerInfo;
    AncientTechText ancientTechText;
    BabyWorm babyWorm;
    AudioManager audioManager;

    Item selectedItem;
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI descriptionText;
    public TMPro.TextMeshProUGUI purchaceText;

    public GameObject babyWormTabUI;
    public GameObject upgradeTabUI;

    public int babyWormEvolutionCost;

    void Start()
    {
        inventory = Inventory.instance;
        playerInfo = PlayerInfo.instance;
        ancientTechText = AncientTechText.instance;
        babyWorm = BabyWorm.instance;
        audioManager = GetComponent<AudioManager>();
        purchaceText.text = "";
    }

    // selects an upgrade and updates UI accordingly
    public void SelectUpgrade(Item item)
    {
        selectedItem = item;
        purchaceText.text = "Purchace:  " + item.cost + " AT";
        StopAllCoroutines();
        StartCoroutine(TypeSentence());
    }

    // purchaces the selected upgrade
    public void PurchaceUpgrade()
    {
        if (playerInfo.ancientTech < selectedItem.cost)
        {
            Debug.Log("Not enough Ancient Tech");
            return;
        }
        if (inventory.specialUpgrades.Contains(selectedItem))
        {
            Debug.Log("You can only have one " + selectedItem.name + "!");
            return;
        }
        playerInfo.ancientTech -= selectedItem.cost;
        ancientTechText.SetAncientTechValue(playerInfo.ancientTech);
        inventory.specialUpgrades.Add(selectedItem);
        Debug.Log("equipped " + selectedItem.name);
        audioManager.PlayPurchaseItemSound();
        UpdatePlayerGameData();
    }

    // Animates out typing a sentence one character at a time
    IEnumerator TypeSentence()
    {

        nameText.text = selectedItem.name + ":";
        descriptionText.text = "";

        foreach (char c in selectedItem.description.ToCharArray())
        {
            descriptionText.text += c;
            yield return new WaitForSecondsRealtime(.01f);
        }
    }

    // updates game data to keep track of what special upgrades the player has
    void UpdatePlayerGameData()
    {
        switch (selectedItem.name)
        {
            case ("Anti-Gravity Drill"):
                playerInfo.hasAntiGravityDrill = true;
                break;
            case ("Regenerative Hull"):
                playerInfo.hasRegenerativeHull = true;
                break;
            case ("Teleporter"):
                playerInfo.hasTeleporter = true;
                break;
            case ("Anti-Matter Bombs"):
                playerInfo.hasAntiGravityBombs = true;
                break;
        }
    }

    // opens the upgrades tab and updates UI
    public void OpenUpgradesTab()
    {
        babyWormTabUI.SetActive(false);
        upgradeTabUI.SetActive(true);
    }

    // opens the baby worm tab and updates UI
    public void OpenBabyWormTab()
    {
        upgradeTabUI.SetActive(false);
        babyWormTabUI.SetActive(true);
    }

    // evolves baby worm into an adult if possible
    public void FeedBabyWorm()
    {
        if (babyWorm.evolution == BabyWorm.Evolutions.ADULT)
        {
            Debug.Log("Baby Worm has already grown Up!");
        }
        else if (babyWorm.evolution == BabyWorm.Evolutions.BABY)
        {
            if (playerInfo.ancientTech >= babyWormEvolutionCost)
            {
                playerInfo.ancientTech -= babyWormEvolutionCost;
                ancientTechText.SetAncientTechValue(playerInfo.ancientTech);
                babyWorm.isEvolved = true;
            }
            else { Debug.Log("Not enough Ancient Techonology"); }
        }
        else
        {
            Debug.Log("You must discover Baby Worm before you can feed it!");
        }
    }
}
