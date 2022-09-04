using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using static Assets.Scripts.UI.UIManager;

namespace Assets.Scripts
{
    public class ObjectSnapper : MonoBehaviour
    {
        [SerializeField] private InputActionReference _objectSnapAction;
        private bool _isAttached;
        private GameObject _objectColliding;
        private UIManager _uiMgr;

        private void Awake()
        {
            _uiMgr = GetComponentInChildren<UIManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Pickup")) return;

            _objectColliding = other.gameObject;

            if (_uiMgr)
            {
                SetCurrentStatus(VehicleStatus.Pickup);
                _uiMgr.UpdateTextDisplay("-> Status: Object Pickup (Detached)");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_objectSnapAction.action.triggered) return;

            if (other.gameObject != _objectColliding) return;

            if (_objectColliding.transform.parent != transform)
            {
                other.transform.SetParent(transform);
                _isAttached = true;

                if (_uiMgr)
                {
                    _uiMgr.UpdateTextDisplay("-> Status: Object Pickup (Attached)", false);
                }
            }
            else if (_isAttached)
            {
                Transform childToRemove = transform.Find(other.name);
                if (childToRemove.gameObject != other.gameObject) return;
                childToRemove.parent = null;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == _objectColliding)
            {
                if (_uiMgr)
                {
                    SetCurrentStatus(VehicleStatus.Idle);
                    _uiMgr.ResetText(UIManager.GetCurrentText());
                }

                _objectColliding = null;
            }
        }
    }
}
