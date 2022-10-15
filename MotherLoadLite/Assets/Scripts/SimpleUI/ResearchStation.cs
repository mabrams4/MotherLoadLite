using UnityEngine;

// manages opening and closing research station UI

public class ResearchStation : MonoBehaviour
{
    public GameObject researchStationUI;

    PlayerController player;
    public BMResearchStation bMResearchStation;
    bool hasBeenUpdated;


    private void Start()
    {
        player = PlayerController.instance;
    }
    private void Update()
    {
        // if player is in bounds of research station open the UI
        if (GetComponent<Renderer>().bounds.Contains(player.transform.position)
            && player.IsGrounded())
        {
            researchStationUI.SetActive(true);
            if (!hasBeenUpdated)
            {
                hasBeenUpdated = true;
                bMResearchStation.OpenUpgradesTab();
            }
        }
        else
        {
            researchStationUI.SetActive(false);
            hasBeenUpdated = false;
        }
    }
}
