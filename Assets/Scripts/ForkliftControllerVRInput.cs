using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.XR;
using static Assets.Scripts.UI.UIManager;
using CommonUsages = UnityEngine.XR.CommonUsages;

namespace Assets.Scripts
{
    public class ForkliftControllerVRInput : MonoBehaviour
    {
        [Header("Vehicle Speed Settings")]
        public float speedThreshold = 0.7f;

        [Header("Vehicle Handling Settings")]
        public XRNode speedXrNode = XRNode.RightHand;
        public XRNode reverseXrNode = XRNode.LeftHand;

        [Header("Vehicle Direction Settings")]
        public HingeJoint wheel;
        public float maxValue = 0.35f;
        public float minValue = -0.35f;
        public float turnThreshold = 0.2f;

        private ForkliftControllerInput _forkliftControllerInput;
        private UIManager _uiMgr;

        private void Awake()
        {
            _forkliftControllerInput = GetComponent<ForkliftControllerInput>();
            _uiMgr = GetComponentInChildren<UIManager>();
        }

        private void Update()
        {
            //Speed Input
            if (InputDevices.GetDeviceAtXRNode(speedXrNode).TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > speedThreshold)
            {
                if (_uiMgr)
                {
                    SetCurrentStatus(VehicleStatus.Accelerate);
                    _uiMgr.UpdateTextDisplay("-> Status: Accelerating", false);
                }

                _forkliftControllerInput.speedInput = true;
            }
            else
            {
                if (InputDevices.GetDeviceAtXRNode(reverseXrNode).TryGetFeatureValue(CommonUsages.trigger, out float reverseValue) && reverseValue > speedThreshold)
                {
                    if (_uiMgr)
                    {
                        SetCurrentStatus(VehicleStatus.Reverse);
                        _uiMgr.UpdateTextDisplay("-> Status: Reversing", false);
                    }

                    _forkliftControllerInput.reverseInput = true;
                }
                else
                {
                    _forkliftControllerInput.reverseInput = false;

                    // Clear the previous reverse or accelerate HUD messages if we are now idle.
                    if (GetCurrentStatus() == VehicleStatus.Accelerate || GetCurrentStatus() == VehicleStatus.Reverse)
                    {
                        SetCurrentStatus(VehicleStatus.Idle);
                        if (_uiMgr) _uiMgr.ResetText(GetCurrentText());
                    }
                }

                _forkliftControllerInput.speedInput = false;
            }

            //Turn Input
            float steeringNormal = Mathf.InverseLerp(minValue, maxValue, wheel.transform.localRotation.x);
            float steeringRange = Mathf.Lerp(-1, 1, steeringNormal);
            if (Mathf.Abs(steeringRange) < turnThreshold)
            {
                steeringRange = 0;
            }

            _forkliftControllerInput.turnInput = steeringRange;
        }
    }
}
