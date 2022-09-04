using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    public class InteractableDetection : XRBaseInteractable
    {
        #region Member Variables
        private IXRSelectInteractable _interactableObject;
        [SerializeField] private LeverController _leverController;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            FindLeverController();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            Debug.Log("OnSelectEntered");
            base.OnSelectEntered(args);
            _interactableObject = args.interactableObject;
            _leverController.SetInHand(true);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            Debug.Log("OnSelectExited");

            base.OnSelectExited(args);
            _interactableObject = null;
            _leverController.SetInHand(false);
        }
        private void FindLeverController()
        {
            if (!_leverController)
            {
                _leverController = FindObjectOfType<LeverController>();
            }
        }

        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return base.IsHoverableBy(interactor) && interactor is XRDirectInteractor;
        }

        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return base.IsSelectableBy(interactor) && interactor is XRDirectInteractor;
        }

        public IXRSelectInteractable GetSelectInteractable()
        {
            return _interactableObject;
        }
    }
}