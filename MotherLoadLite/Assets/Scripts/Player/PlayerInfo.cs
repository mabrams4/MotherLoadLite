using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/* this script manages the players health and fuel bars, taking damage, and setting the correct altitude */
public class PlayerInfo : MonoBehaviour
{
    #region Singleton
    public static PlayerInfo instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one PlayerInfo?");
            return;
        }
        instance = this;
    }
    #endregion
    public GameObject[] shopAndDialogueUIs;
    public GameObject gameUI;
    public GameObject inventoryUI;
    public AltitudeText altitudeText;
    public Button inventoryButton;

    FuelBar fuelBar;
    HealthBar healthBar;
    MoneyText moneyText;
    Inventory inventory;

    // player stats
    public float maxFuel;
    public float currentFuel;
    public float maxHealth;
    public float currentHealth;
    public float money;
    public float ancientTech;
    public int altitude;

    // falling damage modifiers
    public float minSpeedForDamage;
    public float damageModifier;
    public float power;
    public float maxFallDamage;

    // Game Data
    public bool hasAntiGravityDrill;
    public bool hasRegenerativeHull;
    public bool hasTeleporter;
    public bool hasAntiGravityBombs;

    // Other
    public GameObject gameOverScreen;
    public bool useFuel;
    bool takenDamage = false;
    public Item regenerativeHull;
    public bool UIActive;
    float fallSpeed;
    public float regenRate;
    bool died;

    // Audi
    public AudioSource mainThemeSong;
    public float maxThemeVolume;
    public float minThemeVolume;

    private void Start()
    {
        moneyText = MoneyText.instance;
        fuelBar = FuelBar.instance;
        healthBar = HealthBar.instance;
        inventory = Inventory.instance;

        if (SaveSystem.isNewGame) InitializePlayerStats();

        InvokeRepeating(nameof(UseFuel), .1f, .5f);   // consume fuel every half second while able
        InvokeRepeating(nameof(ResetTakenDamage), .1f, 1.5f);     // stop player from taking infinte damage from worms
        InvokeRepeating(nameof(RegenerateHealth), .1f, regenRate);

    }

    void InitializePlayerStats()
    {
        maxFuel = inventory.currentFuelTank.maxFuel;
        maxHealth = inventory.currentHull.maxHealth;
        currentFuel = maxFuel;
        currentHealth = maxHealth;
        money = 100;
        ancientTech = 0;
        fuelBar.SetMaxFuel(maxFuel);
        fuelBar.SetFuel(maxFuel);
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        moneyText.SetMoneyValue(money);
    }

    private void Update()
    {
        // if fuel or health is 0 end the game
        if (currentFuel <= 0 || currentHealth <= 0 && !died)
        {
            died = true;
            gameUI.SetActive(false);    // disables any open shop UIS
            GetComponent<Animator>().SetBool("died", true);
            gameOverScreen.SetActive(true);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<PlayerMovement>().enabled = false;
            mainThemeSong.enabled = false;
        }

        fallSpeed = GetComponent<Rigidbody2D>().velocity.y;

        // set player altitude
        altitude = Mathf.FloorToInt(transform.position.y - GetComponent<PlayerController>().grid.transform.localScale.y - 1);
        altitudeText.SetAltitude(altitude);

        // if a store or dialogue box is open dont consume fuel, disable inventory button,
        // and lower main theme music volume
        int numActive = 0;
        foreach (GameObject UI in shopAndDialogueUIs)
        {
            if (UI.activeInHierarchy)
            {
                inventoryButton.interactable = false;
                numActive++;
                useFuel = false;
                UIActive = true;
                mainThemeSong.volume = minThemeVolume;
            }
        }
        if (numActive == 0 && !inventoryUI.activeInHierarchy)
        {
            useFuel = true;
            inventoryButton.interactable = true;
            UIActive = false;
            mainThemeSong.volume = maxThemeVolume;
        }
    }

    // apply damage taken from a fall if falling fast enough
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Input.GetAxisRaw("Vertical") == 1) return;
        if (collision.gameObject.CompareTag("Tiles") && Mathf.Abs(fallSpeed) > minSpeedForDamage)
        {
            float damage = Mathf.Round(Mathf.Pow(Mathf.Abs(fallSpeed / damageModifier), power));
            ApplyDamage(Mathf.Min(damage, maxFallDamage));
        }
    }

    // consumes 1 fuel if appropriate
    public void UseFuel()
    {
        if (useFuel)
        {
            currentFuel -= 1;
            fuelBar.SetFuel(currentFuel);
        }
    }

    // applies damage to the player
    public void ApplyDamage(float damage)
    {
        if (!takenDamage)
        {
            takenDamage = true;
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            if (currentHealth < 0) currentHealth = 0;
        }
    }

    // applies acid damage to the player continuously 
    public void ApplyAcidDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth < 0) currentHealth = 0;
    }

    // resets timer for player being able to take damage
    void ResetTakenDamage()
    {
        takenDamage = false;
    }

    // regenerates player health slowly if palyer has regenerative hull item 
    void RegenerateHealth()
    {
        if (currentHealth >= maxHealth) return;
        if (inventory.specialUpgrades.Contains(regenerativeHull))
        {
            currentHealth += 1;
            healthBar.SetHealth(currentHealth);
        }
    }
}
