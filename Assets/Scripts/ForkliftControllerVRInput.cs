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
        [SerializeField] private float _speedThreshold = 0.7f;

        [Header("Vehicle Handling Settings")]
        [SerializeField] private XRNode _speedXrNode = XRNode.RightHand;
        [SerializeField] private XRNode _reverseXrNode = XRNode.LeftHand;

        [Header("Vehicle Direction Settings")]
        [SerializeField] private HingeJoint _steeringWheel;
        [SerializeField] private float _maxValue = 0.35f;
        [SerializeField] private float _minValue = -0.35f;
        [SerializeField] private float _turnThreshold = 0.2f;

        [Header("Vehicle Game Events")] 
        [SerializeField] private GameEvent _forkliftReverse;

        private ForkliftControllerInput _forkliftControllerInput;
        private UIManager _uiMgr;

        private void Awake()
        {
            _forkliftControllerInput = GetComponent<ForkliftControllerInput>();
            _uiMgr = GetComponentInChildren<UIManager>();
        }

        private void Update()
        {
            // Check the controllers for the speed input values
            if (InputDevices.GetDeviceAtXRNode(_speedXrNode).TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > _speedThreshold)
            {
                if (_uiMgr)
                {
                    SetCurrentStatus(VehicleStatus.Accelerate);
                    _uiMgr.UpdateTextDisplay("-> Status: Accelerating", false);
                }

                _forkliftControllerInput._speedInput = true;
            }
            else
            {
                // Check the controllers for the reverse input values
                if (InputDevices.GetDeviceAtXRNode(_reverseXrNode).TryGetFeatureValue(CommonUsages.trigger, out float reverseValue) && reverseValue > _speedThreshold)
                {
                    if (_uiMgr)
                    {
                        SetCurrentStatus(VehicleStatus.Reverse);
                        _uiMgr.UpdateTextDisplay("-> Status: Reversing", false);
                    }

                    if (!_forkliftControllerInput._reverseInput)
                    {
                        _forkliftReverse?.Invoke();
                    }

                    _forkliftControllerInput._reverseInput = true;
                }
                else
                {
                    _forkliftControllerInput._reverseInput = false;

                    // Clear the previous reverse or accelerate HUD messages if we are now idle.
                    if (GetCurrentStatus() == VehicleStatus.Accelerate || GetCurrentStatus() == VehicleStatus.Reverse)
                    {
                        SetCurrentStatus(VehicleStatus.Idle);
                        if (_uiMgr) _uiMgr.ResetText(GetCurrentText());
                    }
                }

                _forkliftControllerInput._speedInput = false;
            }

            // Normalised turn input
            float steeringNormal = Mathf.InverseLerp(_minValue, _maxValue, _steeringWheel.transform.localRotation.x);
            float steeringRange = Mathf.Lerp(-1, 1, steeringNormal);
            if (Mathf.Abs(steeringRange) < _turnThreshold)
            {
                steeringRange = 0;
            }

            _forkliftControllerInput._turnInput = steeringRange;
        }
    }
}
