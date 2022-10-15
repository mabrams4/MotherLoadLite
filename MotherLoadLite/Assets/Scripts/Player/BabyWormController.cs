using UnityEngine;

/* this script handles player input to control baby worm phase states */
public class BabyWormController : MonoBehaviour
{
    #region Singleton
    public static BabyWormController instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("oops");
            return;
        }
        instance = this;
    }
    #endregion
    BabyWorm babyWorm;

    private void Start()
    {
        babyWorm = BabyWorm.instance;
    }

    private void Update()
    {
        if (Input.GetButtonDown("HuntWorms") && babyWorm.evolution == BabyWorm.Evolutions.ADULT)
        {
            babyWorm.phase = BabyWorm.Phase.HUNT_WORMS;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            babyWorm.phase = BabyWorm.Phase.IDLE;
        }
    }


}
