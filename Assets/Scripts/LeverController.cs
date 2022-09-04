using System;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(XRGrabInteractable))]
public class LeverController : MonoBehaviour
{
    #region Member Variables
    public float LeverOnAngle = -45;
    public float LeverOffAngle = 45;
    public bool LeverIsOn;
    public bool LeverWasSwitched;

    private HingeJoint _leverHingeJoint;
    private InteractableDetection _interactable;
    private bool _wasGrabbed;
    private Vector3 _startingEuler;
    private Forklift _forklift;
    private bool _inHand;

    [Header("Game Events")]
    [SerializeField] private GameEvent _toggleLeverUp;
    [SerializeField] private GameEvent _toggleLeverDown;
    #endregion

    private void Start()
    {
        _leverHingeJoint = GetComponent<HingeJoint>();
        _forklift = GetComponentInParent<Forklift>();
        _interactable = GetComponent<InteractableDetection>();

        JointLimits limits = _leverHingeJoint.limits;
        limits.max = Mathf.Max(LeverOnAngle, LeverOffAngle);
        limits.min = Mathf.Min(LeverOnAngle, LeverOffAngle);
        _leverHingeJoint.limits = limits;
        _leverHingeJoint.useLimits = true;

        _startingEuler = transform.localEulerAngles;
    }

    private void FixedUpdate() 
    {
        LeverWasSwitched = false;

        float offDistance = Quaternion.Angle(transform.localRotation, OffHingeAngle());
        float onDistance = Quaternion.Angle(transform.localRotation, OnHingeAngle());
        bool shouldBeOn = (Mathf.Abs(onDistance) < Mathf.Abs(offDistance));

        if (shouldBeOn != LeverIsOn)
        {
            LeverIsOn = !LeverIsOn;
            LeverWasSwitched = true;
            UpdateHingeJoint();
        }

        if (_wasGrabbed != InHand()) 
        {
            _wasGrabbed = InHand();
            UpdateHingeJoint();
        }

        if (name == "HeightLever")
        {
            // update the forklift fork height position
            const float fTolerance = 0.1f;
            if (Math.Abs(_leverHingeJoint.spring.targetPosition - LeverOffAngle) < fTolerance)
            {
                _forklift.LowerFork();
            }
            else if (Math.Abs(_leverHingeJoint.spring.targetPosition - LeverOnAngle) < fTolerance)
            {
                _forklift.RaiseFork();
            }
        }

        if (name == "TiltLever")
        {
            // update the forklift fork tilt position
            const float fTolerance = 0.1f;
            if (Math.Abs(_leverHingeJoint.spring.targetPosition - LeverOffAngle) < fTolerance)
            {
                _forklift.TiltForkIn();
            }
            else if (Math.Abs(_leverHingeJoint.spring.targetPosition - LeverOnAngle) < fTolerance)
            {
                _forklift.TiltForkOut();
            }
        }

        UpdateLeverSfx();
    }

    private void UpdateHingeJoint() 
    {
        JointSpring spring = _leverHingeJoint.spring;

        if (InHand()) 
        {
            _leverHingeJoint.useSpring = false;
        } 
        else
        {
            spring.targetPosition = LeverIsOn ? LeverOnAngle : LeverOffAngle;
            _leverHingeJoint.useSpring = true;
        }

        _leverHingeJoint.spring = spring;
    }

    private Quaternion OnHingeAngle()
    {
        return Quaternion.Euler(_leverHingeJoint.axis * LeverOnAngle + _startingEuler);
    }

    private Quaternion OffHingeAngle()
    {
        return Quaternion.Euler(_leverHingeJoint.axis * LeverOffAngle + _startingEuler);
    }

    public void SetInHand(bool isInHand)
    {
        _inHand = isInHand;
    }

    private bool InHand()
    {
        return _inHand;
    }

    private void UpdateLeverSfx()
    {
        if (LeverWasSwitched && LeverIsOn)
        {
            if (_toggleLeverUp)
            {
                _toggleLeverUp.Invoke();
            }
        }
        else if (LeverWasSwitched && !LeverIsOn)
        {
            if (_toggleLeverDown)
            {
                _toggleLeverDown.Invoke();
            }
        }
    }
}
