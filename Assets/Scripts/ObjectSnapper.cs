using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using static Assets.Scripts.UI.UIManager;

namespace Assets.Scripts
{
    [RequireComponent(typeof(UIManager))]

    public class ObjectSnapper : MonoBehaviour
    {
        [SerializeField] private InputActionReference _attachObjectSnapAction;
        [SerializeField] private InputActionReference _detachObjectSnapAction;
        [SerializeField] private UIManager _uiMgr;
        [SerializeField] private bool _isAttached;

        private GameObject _objectColliding;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Pickup")) return;

            _objectColliding = other.gameObject;

            if (_uiMgr)
            {
                SetCurrentStatus(VehicleStatus.Pickup);
                _uiMgr.UpdateTextDisplay("-> Press *primary(A)* button to attach and *secondary(B)* to release object");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other || other.gameObject != _objectColliding) return;

            if (_attachObjectSnapAction.action.triggered)
            {
                if (_objectColliding.transform.parent.gameObject != transform.gameObject)
                {
                    other.transform.SetParent(transform);
                    _isAttached = true;

                    if (_uiMgr)
                    {
                        _uiMgr.UpdateTextDisplay("-> Status: Object Pickup (Attached)", false);
                    }
                }
                return;
            }

            if (_detachObjectSnapAction.action.triggered)
            {
                if (_isAttached)
                {
                    if (_uiMgr)
                    {
                        _uiMgr.UpdateTextDisplay("-> Status: Object Pickup (Detached)", false);
                    }

                    Transform childToRemove = transform.Find(other.name);
                    if (childToRemove.gameObject != other.gameObject) return;

                    childToRemove.parent = null;
                    _isAttached = false;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == _objectColliding)
            {
                if (_uiMgr)
                {
                    SetCurrentStatus(VehicleStatus.Idle);
                    _uiMgr.ResetText(GetCurrentText());
                }

                _objectColliding = null;
            }
        }
    }
}
