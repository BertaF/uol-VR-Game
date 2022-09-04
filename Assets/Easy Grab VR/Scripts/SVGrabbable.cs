using UnityEngine;

[RequireComponent(typeof(SVControllerInput))]
[RequireComponent(typeof(Rigidbody))]
public class SVGrabbable : MonoBehaviour
{
    //------------------------
    // Constants
    //------------------------
    private const float kGrabResetTime = 0.5f; // 120 ms.

    //------------------------
    // Variables
    //------------------------
    [Space(15)]
    [Header("Pickup Settings")]
    [Tooltip("Defines if its it possible to pick this object up")]
    public bool canGrab = true;

    [Space(15)]
    [Header("Hold Settings")]

    [Tooltip("This allows you to mirror the offset on the Local X Axis if the object is held in your left hand. You usually want this on.")]
    public bool mirrorXOffsetInLeftHand = true;
    [Tooltip("Where on the object should you grip it? Offsets are useful for tools like hammers and other things.")]
    public Vector3 positionOffsetInHand = new Vector3(0, 0, 0);

    [Tooltip("If true this forces the object to a specific local rotation when you pick it up.")]
    public bool forceRotationInHand = false;
    [Tooltip("The rotation forced in hand. Only applies if forceRotationInHand is true.")]
    public Vector3 rotationInHand = new Vector3(0, 0, 0);

    [Tooltip("If true a held object won't collide with anything and doesn't use inHandLerpSpeed, rather it sets its position directly.")]
    public bool ignorePhysicsInHand = true;

    [Tooltip("How quickly the object will match your hands position when moving. Higher values give you less lag but less realistic physics when interacting with immobile objects.")]
    [Range(0.0f, 1.0f)]
    public float inHandLerpSpeed = 0.4f;

    [Tooltip("How far from your hand the object needs to be before we drop it automatically. Useful for collisions.")]
    public float objectDropDistance = 0.3f;

    [Space(15)]
    [Header("Collision Settings")]

    [HideInInspector]
    public bool inHand = false;

    // Private Components
    private SVAbstractGripIndicator gripIndicatorComponent;
    private SVControllerInput input;

    // Private AND Static.
    private static GameObject leftHandCollider;
    private static GameObject rightHandCollider;

    struct GrabData
    {
        public float grabEndTime;
        public bool recentlyReleased;
        public bool recentlyDropped;
        public Quaternion grabStartLocalRotation;
        public Quaternion grabStartWorldRotation;
        public bool wasKinematic;
        public bool didHaveGravity;
        public bool wasKnockable;
        public bool hasJoint;
    };

    private GrabData grabData;
    private Rigidbody rb;
    private Collider[] colliders;

    //------------------------
    // Init
    //------------------------
    void Start() 
    {
        if (this.gameObject.GetComponent<SVAbstractGripIndicator>())
        {
            gripIndicatorComponent = this.gameObject.GetComponent<SVAbstractGripIndicator>();
        }

        this.input = this.gameObject.GetComponent<SVControllerInput>();
        this.rb = this.GetComponent<Rigidbody>();
        this.colliders = (Collider[])SVUtilities.AllComponentsOfType<Collider>(gameObject);

        if (SVGrabbable.leftHandCollider == null) 
        {
            GameObject obj = new GameObject("Left Hand Collider");
            obj.AddComponent<SVColliderUpdater>().isLeft = true;
            SVGrabbable.leftHandCollider = obj;
        }

        if (SVGrabbable.rightHandCollider == null) 
        {
            GameObject obj = new GameObject("Right Hand Collider");
            obj.AddComponent<SVColliderUpdater>().isLeft = false;
            SVGrabbable.rightHandCollider = obj;
        }
    }

    //------------------------
    // Public
    //------------------------

    public void DropFromHand() 
    {
        if (this.inHand) {
            this.ClearActiveController();
        }
    }

    //------------------------
    // Update
    //------------------------
    /* It's so we can run BEFORE our physics calculations.  This enables us to force position to hand position while
     * still respecting the Unity physics engine. This is great when you hand joints connected to your objects.
	*/
    private void FixedUpdate()
    {
        DoGrabbedUpdate();
    }

    void DoGrabbedUpdate()
    {
        if (input.activeController == SVControllerType.SVController_None) 
        {
            UngrabbedUpdate();
        } 
        else
        {
            if (canGrab) 
            {
                GrabbedUpdate();
            }
        }
    }

    private void UngrabbedUpdate()
    {
        this.inHand = false;

        // Reset our knockable state after dropping this object
        if (grabData.recentlyReleased && (Time.time - grabData.grabEndTime) > kGrabResetTime) 
        {
            grabData.recentlyReleased = false;
            grabData.recentlyDropped = false;
        }

        // If we drop something, give it a little cooldown so it can drop to the floor before we snag it.
        if (grabData.recentlyDropped) 
        {
            return;
        }

        float distanceToLeftHand = 1000;
        if (input.LeftControllerIsConnected) 
        {
            distanceToLeftHand = (transform.position - input.LeftControllerPosition).magnitude;
        }

        float distanceToRightHand = 1000;
        if (input.RightControllerIsConnected) 
        {
            distanceToRightHand = (transform.position - input.RightControllerPosition).magnitude;
        }

        if (gripIndicatorComponent) 
        {
            gripIndicatorComponent.indicatorActive = 0;
        }
        // Clear our object as nearest if it's not in grabbin range!
        if (SVControllerManager.nearestGrabbableToRightController == this) 
        {
            SVControllerManager.nearestGrabbableToRightController = null;
        }
        if (SVControllerManager.nearestGrabbableToLeftController == this) 
        {
            SVControllerManager.nearestGrabbableToLeftController = null;
        }
    }

    private void GrabbedUpdate() 
    {
        if (input.gripAutoHolds) 
        {
            if (input.GetReleaseGripButtonPressed(input.activeController)) 
            {
                this.ClearActiveController();
                return;
            }
        } 
        else if (!input.GetGripButtonDown(input.activeController))
        {
            this.ClearActiveController();
            return;
        }

        // Get target Rotation
        Quaternion targetRotation;
        Quaternion controllerRotation = input.RotationForController(input.activeController);
        if (forceRotationInHand) 
        {
            targetRotation = controllerRotation * Quaternion.Euler(rotationInHand);
        } 
        else 
        {
            targetRotation = controllerRotation * grabData.grabStartLocalRotation;
        }


        // Make sure position offset respects rotation
        Vector3 targetOffset;
        if (this.input.activeController == SVControllerType.SVController_Left && mirrorXOffsetInLeftHand)
        {
            Matrix4x4 mirrorMatrix = Matrix4x4.Scale(new Vector3(-1, 1, 1));
            Matrix4x4 offsetAndRotation = Matrix4x4.TRS(-positionOffsetInHand, Quaternion.Euler(rotationInHand), Vector3.one);
            Matrix4x4 finalOffsetAndRotation = mirrorMatrix * offsetAndRotation;

            targetRotation = controllerRotation * Quaternion.LookRotation(finalOffsetAndRotation.GetColumn(2), finalOffsetAndRotation.GetColumn(1));
            targetOffset = targetRotation * finalOffsetAndRotation.GetColumn(3);
            targetRotation *= Quaternion.AngleAxis(180, Vector3.up);
        } 
        else 
        {
            targetOffset = targetRotation * -positionOffsetInHand;
        }

        this.inHand = true;
        Vector3 targetPosition = input.PositionForController(input.activeController);

        // If we're moving too quickly and allow physics, drop the object. This also gives us the ability to drop it if you are trying to move it through
        // a solid object.
        if (!this.ignorePhysicsInHand &&
            (transform.position - targetPosition).magnitude >= objectDropDistance) 
        {
            grabData.recentlyDropped = true;
            this.ClearActiveController();
            return;
        }

        // If we've got a joint let's forget about setting the rotation and just focus on the position.
        // This keeps us from losing our minds!
        if (this.grabData.hasJoint)
        {
            transform.position = targetPosition + targetOffset;
        } 
        else 
        {  // otherwise just lock to the hand position so there is no delay
            if (this.ignorePhysicsInHand)
            {
                this.transform.SetPositionAndRotation(targetPosition + targetOffset, targetRotation);
            } 
            else 
            {
                transform.position = Vector3.Lerp(this.transform.position, targetPosition + targetOffset, inHandLerpSpeed);
                transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, inHandLerpSpeed);
                rb.velocity = this.input.ActiveControllerVelocity();
                rb.angularVelocity = this.input.ActiveControllerAngularVelocity();
            }
        }
    }

    //------------------------
    // State Changes
    //------------------------
    private void TrySetActiveController(SVControllerType controller) 
    {
        if (this.input.activeController != SVControllerType.SVController_None ||
            controller == SVControllerType.SVController_None)
            return;

        if (input.gripAutoHolds) 
        {
            if (!input.GetGripButtonPressed(controller)) 
            {
                return;
            }
        } 
        else 
        {
            if (!input.GetGripButtonDown(controller)) 
            {
                return;
            }
        }

        if (this.input.SetActiveController(controller)) 
        {
            this.grabData.grabStartWorldRotation = this.gameObject.transform.rotation;
            this.grabData.grabStartLocalRotation = Quaternion.Inverse(this.input.RotationForController(controller)) * this.grabData.grabStartWorldRotation;
            this.grabData.hasJoint = (this.gameObject.GetComponent<Joint>() != null);
            if (this.gripIndicatorComponent) 
            {
                gripIndicatorComponent.indicatorActive = 0;
            }

            // Update our rigidbody to respect being controlled by the player
            grabData.wasKinematic = rb.isKinematic;
            grabData.didHaveGravity = rb.useGravity;
            if (this.ignorePhysicsInHand) 
            {
                rb.isKinematic = true;
                foreach (Collider collider in this.colliders) 
                {
                    collider.enabled = false;
                }
            } 
            else 
            {
                rb.useGravity = false;
            }
        }
    }

    private void ClearActiveController() 
    {
        grabData.grabEndTime = Time.time;
        grabData.recentlyReleased = true;

        rb.isKinematic = grabData.wasKinematic;
        rb.useGravity = grabData.didHaveGravity;

        if (this.ignorePhysicsInHand) 
        {
            foreach (Collider collider in this.colliders) 
            {
                collider.enabled = true;
            }
        }

        if (!grabData.recentlyDropped) 
        {
            rb.velocity = this.input.ActiveControllerVelocity();
            rb.angularVelocity = this.input.ActiveControllerAngularVelocity();
        }

        // Show the render model
        this.input.ClearActiveController();
    }
}
