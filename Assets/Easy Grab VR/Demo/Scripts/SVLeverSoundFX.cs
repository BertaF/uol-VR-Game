using UnityEngine;

public class SVLeverSoundFX : MonoBehaviour
{
    private LeverController lever;

    [Header("Lever Events")]
    [SerializeField] GameEvent ToggleLeverUp;
    [SerializeField] GameEvent ToggleLeverDown;

    private void Start()
    {
        lever = GetComponent<LeverController>();
    }

    private void Update()
    {
        if (lever.LeverWasSwitched && lever.LeverIsOn)
        {
            if (ToggleLeverUp)
            {
                ToggleLeverUp.Invoke();
            }
        }
        else if (lever.LeverWasSwitched && !lever.LeverIsOn) 
        {
            if (ToggleLeverDown)
            {
                ToggleLeverDown.Invoke();
            }
        }
    }
}
