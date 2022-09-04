using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    public class OnSelectMakeHandFollow : MonoBehaviour
    {
        public Transform NewParent;
        private XRBaseInteractable _interactable;
        private Transform _handPos;

        private void Start()
        {
            _interactable = GetComponent<XRBaseInteractable>();
            _interactable.selectEntered.AddListener(arg0 => StartHandFollow(arg0));
            _interactable.selectExited.AddListener(arg0 => FinishHandFollow(arg0));
        }

        private void StartHandFollow(SelectEnterEventArgs args)
        {
            if (_handPos) { return; }

            if (args.interactorObject is not XRDirectInteractor) { return; }

            if (!args.interactorObject.transform.CompareTag("PlayerHand")) { return; }

            _handPos = args.interactorObject.transform;

            if (_handPos)
            {
                _handPos.SetParent(NewParent, true);
            }
        }

        private void FinishHandFollow(SelectExitEventArgs args)
        {
            if (args.interactorObject is not XRDirectInteractor) return;

            if (!_handPos) return;

            _handPos.SetParent(args.interactorObject.transform, true);
            _handPos.localPosition = Vector3.zero;
            _handPos.localRotation = Quaternion.identity;
            _handPos = null;
        }
    }
}
