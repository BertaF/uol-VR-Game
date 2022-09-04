using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    public class XROffsetGrabInteractable : XRGrabInteractable
    {
        //s5-1 -it will be safe to reset grab point to original if we've not offset grabbing
        private Vector3 _initialAttachLocalPos;
        private Quaternion _initialAttachLocalRot;

        private void Start()
        {
            //s4 - create attach point to offset
            if (!attachTransform)
            {
                //check if we have an attachment point, if none we recreate a new one
                GameObject grab = new("Grab Pivot");
                grab.transform.SetParent(transform, false);
                attachTransform = grab.transform;
            }

            _initialAttachLocalPos = attachTransform.localPosition;
            _initialAttachLocalRot = attachTransform.localRotation;

        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (args.interactorObject is XRDirectInteractor)
            {
                attachTransform.position = args.interactorObject.transform.position;
                attachTransform.rotation = args.interactorObject.transform.rotation;
            }
            else
            {
                Debug.Log("Ray Interactor");
                attachTransform.localPosition = _initialAttachLocalPos;
                attachTransform.localRotation = _initialAttachLocalRot;
            }

            base.OnSelectEntered(args);
        }
    }
}
