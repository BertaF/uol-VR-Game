using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum SVControllerType
{
    SVController_None,
    SVController_Left,
    SVController_Right
};

public enum SVInputButton
{
    SVButton_None = -1,
    SVButton_A  = 0,
    SVButton_B,
    SVButton_System,
    SVButton_Thumbstick_Press,
    SVButton_Trigger,
    SVButton_Grip,
    SVButton_Thumbstick_Left,
    SVButton_Thumbstick_Right,
    SVButton_Thumbstick_Down,
    SVButton_Thumbstick_Up
};

public class SVControllerInput : MonoBehaviour
{
    //------------------------
    // Variables
    //------------------------
    [Space(15)]
    [Header("Grip Settings")]

    [Tooltip("The button that should grab the object.")]
    public SVInputButton gripButton = SVInputButton.SVButton_Trigger;

    [Tooltip("If you are auto holding this button will drop the object.")]
    public SVInputButton releaseGripButton = SVInputButton.SVButton_None;

    [Tooltip("If true the object stays in your hand until the releaseGripButton is pressed. If false you drop the object when you release the grip button.")]
    public bool gripAutoHolds = false;

    //------------------------
    // Variables
    //------------------------
    [HideInInspector]
    public SVControllerType activeController;

    private OVRManager controllerManager;
    private Dictionary<int, bool>buttonStateLeft;
    private Dictionary<int, bool>buttonStateRight;
    OVRHapticsClip clipHard;


    //------------------------
    // Setup
    //------------------------
    void Start() 
    {
        controllerManager = Object.FindObjectOfType<OVRManager> ();
        Assert.IsNotNull (controllerManager, "SVControllerInput (with Open VR) Needs a OVRManager in the scene to function correctly.");

        buttonStateLeft = new Dictionary<int, bool>();
        buttonStateRight = new Dictionary<int, bool>();

        int cnt = 10;
        clipHard = new OVRHapticsClip(cnt);
        for (int i = 0; i < cnt; i++) 
        {
            clipHard.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)180;
        }
        clipHard = new OVRHapticsClip(clipHard.Samples, clipHard.Samples.Length);
    }

    //------------------------
    // Getters
    //------------------------
    public bool LeftControllerIsConnected => ((OVRInput.GetConnectedControllers() & OVRInput.Controller.LTouch) == OVRInput.Controller.LTouch);

    public bool RightControllerIsConnected => ((OVRInput.GetConnectedControllers() & OVRInput.Controller.RTouch) == OVRInput.Controller.RTouch);

    public Vector3 LeftControllerPosition 
    {
        get
        {
            Vector3 leftHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            Transform trackingSpace = GameObject.FindObjectOfType<OVRCameraRig>().trackingSpace;
            if (trackingSpace != null) 
            {
                return trackingSpace.TransformPoint(leftHandPosition);
            }
            return leftHandPosition;
        }
    }

    public Vector3 RightControllerPosition
    {
        get 
        {
            Vector3 rightHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            Transform trackingSpace = GameObject.FindObjectOfType<OVRCameraRig>().trackingSpace;
            if (trackingSpace != null) 
            {
                return trackingSpace.TransformPoint(rightHandPosition);
            }
            return rightHandPosition;
        }
    }

    public Quaternion LeftControllerRotation 
    {
        get {
            if (this.LeftControllerIsConnected) 
            {
                Quaternion leftHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
                Transform trackingSpace = GameObject.FindObjectOfType<OVRCameraRig>().trackingSpace;
                if (trackingSpace != null) 
                {
                    return trackingSpace.rotation * leftHandRotation;
                }
                return leftHandRotation;
            }

            return Quaternion.identity;
        }
    }

    public Quaternion RightControllerRotation 
    {
        get
        {
            if (this.RightControllerIsConnected) 
            {

                Quaternion rightHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
                Transform trackingSpace = GameObject.FindObjectOfType<OVRCameraRig>().trackingSpace;
                if (trackingSpace != null) {
                    return trackingSpace.rotation * rightHandRotation;
                }
                return rightHandRotation;

            }

            return Quaternion.identity;
        }
    }

    public Vector3 LeftControllerVelocity
    {
        get 
        {     
            Vector3 leftHandVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
            Transform trackingSpace = GameObject.FindObjectOfType<OVRCameraRig>().trackingSpace;
            if (trackingSpace != null)
            {
                return trackingSpace.TransformDirection(leftHandVelocity);
            }
            return leftHandVelocity;
        }
    }

    public Vector3 RightControllerVelocity 
    {
        get 
        {
            Vector3 rightHandVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            Transform trackingSpace = GameObject.FindObjectOfType<OVRCameraRig>().trackingSpace;
            if (trackingSpace != null) {
                return trackingSpace.TransformDirection(rightHandVelocity);
            }
            return rightHandVelocity;
        }
    }

    public Vector3 LeftControllerAngularVelocity 
    {
        get 
        {
            Vector3 leftHandAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.LTouch);
            Transform trackingSpace = GameObject.FindObjectOfType<OVRCameraRig>().trackingSpace;
            if (trackingSpace != null) 
            {
                return trackingSpace.TransformDirection(leftHandAngularVelocity);
            }
            return leftHandAngularVelocity;
        }
    }

    public Vector3 RightControllerAngularVelocity 
    {
        get 
        {
            Vector3 rightHandAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            Transform trackingSpace = GameObject.FindObjectOfType<OVRCameraRig>().trackingSpace;
            if (trackingSpace != null)
            {
                return trackingSpace.TransformDirection(rightHandAngularVelocity);
            }
            return rightHandAngularVelocity;
        }
    }

    //------------------------
    // Controller Info
    //------------------------
    public Vector3 PositionForController(SVControllerType controller)
    {
        if (controller == SVControllerType.SVController_Left)
        {
            return LeftControllerPosition;
        } 
        else if (controller == SVControllerType.SVController_Right)
        {
            return RightControllerPosition;
        }

        return Vector3.zero;
    }

    public Quaternion RotationForController(SVControllerType controller) 
    {
        if (controller == SVControllerType.SVController_Left) 
        {
            return LeftControllerRotation;
        } 
        else if (controller == SVControllerType.SVController_Right) 
        {
            return RightControllerRotation;
        }

        return Quaternion.identity;
    }

    public bool ControllerIsConnected(SVControllerType controller) 
    {
        if (controller == SVControllerType.SVController_Left) 
        {
            return LeftControllerIsConnected;
        } 
        else if (controller == SVControllerType.SVController_Right) 
        {
            return RightControllerIsConnected;
        }

        return false;
    }

    //------------------------
    // Input Checkers
    //------------------------

    public bool GetGripButtonDown(SVControllerType controller) 
    {
        return this.GetButtonDown (controller, this.gripButton);
    }

    public bool GetGripButtonPressed(SVControllerType controller) 
    {
        return this.GetButtonPressDown (controller, this.gripButton);
    }

    public bool GetReleaseGripButtonPressed(SVControllerType controller) 
    {
        return this.GetButtonPressDown (controller, this.releaseGripButton);
    }

    //------------------------
    // Public
    //------------------------

    public bool GetButtonDown(SVControllerType controller, SVInputButton button) 
    {
        if (button == SVInputButton.SVButton_None || !ControllerIsConnected(controller))
            return false;

        return GetOVRButtonDown(controller, button);
    }

    public bool GetButtonPressDown(SVControllerType controller, SVInputButton button) 
    {
        if (button == SVInputButton.SVButton_None || !ControllerIsConnected(controller))
            return false;
        
        return GetOVRButtonPressDown(controller, button);
    }

    public bool SetActiveController(SVControllerType activeController) 
    {
        if (activeController == SVControllerType.SVController_Left &&
            SVControllerManager.leftControllerActive) 
        {
            return false;
        }

        if (activeController == SVControllerType.SVController_Right &&
            SVControllerManager.rightControllerActive) 
        {
            return false;
        }

        this.activeController = activeController;

        if (this.activeController == SVControllerType.SVController_Right) 
        {
            SVControllerManager.rightControllerActive = true;
        } 
        else 
        {
            SVControllerManager.leftControllerActive = true;
        }
			
        return true;
    }

    public void ClearActiveController() 
    {
        if (this.activeController == SVControllerType.SVController_Right) 
        {
            SVControllerManager.rightControllerActive = false;
        } 
        else 
        {
            SVControllerManager.leftControllerActive = false;
        }

        this.activeController = SVControllerType.SVController_None;
    }

    public void RumbleActiveController(float rumbleLength) 
    {
        StartCoroutine( OVRVibrateForTime(rumbleLength) );
    }

    public Vector3 ActiveControllerVelocity() 
    {
        if (activeController == SVControllerType.SVController_Left) 
        {
            return LeftControllerVelocity;
        } 
        else if (activeController == SVControllerType.SVController_Right) 
        {
            return RightControllerVelocity;
        } 
        else 
        {
            return Vector3.zero;
        }
    }

    public Vector3 ActiveControllerAngularVelocity() 
    {
        if (activeController == SVControllerType.SVController_Left) 
        {
            return LeftControllerAngularVelocity;
        } 
        else if (activeController == SVControllerType.SVController_Right) 
        {
            return RightControllerAngularVelocity;
        } 
        else 
        {
            return Vector3.zero;
        }
    }

    //------------------------
    // Haptics
    //------------------------
    public IEnumerator OVRVibrateForTime(float time)
    {
        OVRHaptics.OVRHapticsChannel channel;
        if (activeController == SVControllerType.SVController_Left) 
        {
            channel = OVRHaptics.LeftChannel;
        } 
        else 
        {
            channel = OVRHaptics.RightChannel;
        }

        for (float t = 0; t <= time; t += Time.deltaTime) 
        {
            channel.Queue(clipHard);
        }
		
        yield return new WaitForSeconds(time);
        channel.Clear();
        yield return null;
    }

    //------------------------
    // OVR Mappings
    //------------------------
    private OVRInput.Button GetOVRButtonMapping(SVInputButton button)
    {
        switch (button)
        {
            case SVInputButton.SVButton_A:
                return OVRInput.Button.One;
            case SVInputButton.SVButton_B:
                return OVRInput.Button.Two;
            case SVInputButton.SVButton_System:
                return OVRInput.Button.Start;
            case SVInputButton.SVButton_Thumbstick_Press:
                return OVRInput.Button.PrimaryThumbstick;
        }

        return (OVRInput.Button)0;
    }

    private bool GetOVRButtonPressDown(SVControllerType controller, SVInputButton button)
    {
        bool isRight = (controller == SVControllerType.SVController_Right);
        Dictionary<int, bool> buttonState = isRight ? this.buttonStateRight : this.buttonStateLeft;

        bool isDown = GetOVRButtonDown (controller, button);
        bool inputIsDown = buttonState.ContainsKey ((int)button) && (bool)buttonState [(int)button];
        bool isPressDown = (!inputIsDown && isDown);
        buttonState [(int)button] = isDown;
        return isPressDown;
    }

    private bool GetOVRButtonDown(SVControllerType controller, SVInputButton button)
    {
        bool isRight = (controller == SVControllerType.SVController_Right);
        OVRInput.Controller ovrController = (isRight ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch);

        switch (button)
        {
            // Buttons
            case SVInputButton.SVButton_A:
            case SVInputButton.SVButton_B:
            case SVInputButton.SVButton_System:
            case SVInputButton.SVButton_Thumbstick_Press:
                return OVRInput.Get (GetOVRButtonMapping(button), ovrController);

            // 2D Axis
            case SVInputButton.SVButton_Thumbstick_Down:
            case SVInputButton.SVButton_Thumbstick_Left:
            case SVInputButton.SVButton_Thumbstick_Right:
            case SVInputButton.SVButton_Thumbstick_Up:
            {
                OVRInput.Axis2D axis2D = OVRInput.Axis2D.PrimaryThumbstick;

                Vector2 vec = OVRInput.Get (axis2D, ovrController);

                if (button == SVInputButton.SVButton_Thumbstick_Down) 
                {
                    return vec.y < -0.75;
                } 
                else if (button == SVInputButton.SVButton_Thumbstick_Up) 
                {
                    return vec.y > 0.75;
                } 
                else if (button == SVInputButton.SVButton_Thumbstick_Left) 
                {
                    return vec.x < -0.75;
                } 
                else if (button == SVInputButton.SVButton_Thumbstick_Right) 
                {
                    return vec.x > 0.75;
                }
                return false;
            }

            // 1D Axis
            case SVInputButton.SVButton_Trigger:
            case SVInputButton.SVButton_Grip:
            {
                OVRInput.Axis1D axis = OVRInput.Axis1D.PrimaryIndexTrigger;
                if (button == SVInputButton.SVButton_Trigger) 
                {
                    axis = OVRInput.Axis1D.PrimaryIndexTrigger;
                } 
                else if (button == SVInputButton.SVButton_Grip) 
                {
                    axis = OVRInput.Axis1D.PrimaryHandTrigger;
                }
                return (OVRInput.Get (axis, ovrController) > 0.75f);
            }

            default:
                return false;
        }
    }
}
