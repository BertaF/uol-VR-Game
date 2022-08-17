using UnityEngine;
using UnityEngine.InputSystem;

public class Forklift : MonoBehaviour
{
    Transform fork;
    Transform chainRollers;
    Transform forkMechanism;

    Material chainMat;

    const float forkMaxUp = 2.0f;
    float forkMaxDown;

    private ForkliftControls forkliftControls;

    [Header("Events")]
    [SerializeField] GameEvent ToggleChain;

    private void Awake()
    {
        forkliftControls = new ForkliftControls();
    }

    private void OnEnable()
    {
        forkliftControls.Enable();
    }

    private void OnDisable()
    {
        forkliftControls.Disable();
    }

    private void Start()
    {
        //Search children based on MeshFilter components (they all have it)
        foreach (var mf in GetComponentsInChildren<MeshFilter>()) 
        {
            //Find fork
            if (mf.name.Equals ("Fork")) 
            {
                fork = mf.transform;
                forkMaxDown = fork.transform.localPosition.z;
            }

            //Find fork mechanism, when found, store Chain material
            if (mf.name.Equals ("Fork_Mechanism")) 
            {
                forkMechanism = mf.transform;
                Renderer r = mf.GetComponent<Renderer> ();
                foreach (var m in r.materials) 
                {
                    if (m.name.Contains ("Chain")) 
                    {
                        chainMat = m;
                    }
                }
            }

            //Rollers
            if (mf.name.Equals ("Chain_Rollers")) 
            {
                chainRollers = mf.transform;
            }
        }
    }

    private void FixedUpdate() 
    {
        if (forkliftControls.Forklift.RaiseFork.ReadValue<float>() > 0.1) { RaiseFork(); }

        if (forkliftControls.Forklift.LowerFork.ReadValue<float>() > 0.1) { LowerFork(); }

        if (forkliftControls.Forklift.TiltForkIn.ReadValue<float>() > 0.1) { TiltForkIn(); }

        if (forkliftControls.Forklift.TiltForkOut.ReadValue<float>() > 0.1) { TiltForkOut(); }
    }

    public void TiltForkOut()
    {
        // TODO: Find a limit for this movement

        if (forkMechanism.localEulerAngles.x > 270f)
        {
            forkMechanism.Rotate(-Vector3.right * Time.deltaTime * 2);
        }
    }

    public void TiltForkIn()
    {
        if (forkMechanism.localEulerAngles.x < 275f)
        {
            forkMechanism.Rotate(Vector3.right * Time.deltaTime * 2);
        }
    }

    public void LowerFork()
    {
        if (!(fork.transform.localPosition.z >= forkMaxDown)) return;
        fork.transform.localPosition -= Vector3.forward * Time.deltaTime;
        chainMat.mainTextureOffset = new Vector2(chainMat.mainTextureOffset.x + Time.deltaTime, chainMat.mainTextureOffset.y);
        chainRollers.Rotate(-Vector3.right * 6);

        //if (ToggleChain) { ToggleChain.Invoke(); }
    }

    public void RaiseFork()
    {
        if (!(fork.transform.localPosition.z <= forkMaxUp)) return;
        fork.transform.localPosition += Vector3.forward * Time.deltaTime;
        chainMat.mainTextureOffset = new Vector2(chainMat.mainTextureOffset.x - Time.deltaTime, chainMat.mainTextureOffset.y);
        chainRollers.Rotate(Vector3.right * 6);

        //if (ToggleChain) { ToggleChain.Invoke(); }
    }
}
