using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVLeverSoundFX : MonoBehaviour
{
    private SVLever lever;

    [Header("Events")]
    [SerializeField] GameEvent ToggleLeverUp;
    [SerializeField] GameEvent ToggleLeverDown;

    private void Start()
    {
        lever = GetComponent<SVLever>();
    }

    private void Update()
    {
        if (lever.leverWasSwitched && lever.leverIsOn)
        {
            if (ToggleLeverUp)
            {
                ToggleLeverUp.Invoke();
            }
        }
        else if (lever.leverWasSwitched && !lever.leverIsOn) 
        {
            if (ToggleLeverDown)
            {
                ToggleLeverDown.Invoke();
            }
        }
    }
}
