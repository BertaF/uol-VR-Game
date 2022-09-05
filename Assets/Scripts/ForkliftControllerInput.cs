using UnityEngine;

namespace Assets.Scripts
{
    public class ForkliftControllerInput : MonoBehaviour
    {
        public Transform _forkliftModel;
        public Transform _forkliftNormal;
        public Rigidbody sphere;

        float speed, currentSpeed;
        float rotate, currentRotate;

        [Header("Steering Input")]
        public float turnInput;
        public bool speedInput;
        public bool reverseInput;

        [Header("Parameter Settings")]
        public float acceleration = 30f;
        public float revAcceleration = 10f;
        public float steering = 80f;
        public float gravity = 10f;
        public LayerMask layerMask;
        public float steerAnimationSpeed = 0.1f;
        public float steerAnimationAmount = 10;

        [Header("Model Parts")]
        [SerializeField] private Transform[] _frontWheels;
        [SerializeField] private Transform[] _backWheels;

        private void Update()
        {
            // Make this transform follow the vehicle sphere collider
            transform.position = sphere.transform.position;

            // Setting acceleration value
            if (speedInput)
                speed = acceleration;

            // Setting reverse value
            if (reverseInput)
                speed = -revAcceleration;

            // Convert turn input into direction
            if (turnInput != 0)
            {
                int dir = turnInput > 0 ? 1 : -1;
                float amount = Mathf.Abs((turnInput));
                Steer(dir, amount);
            }

            currentSpeed = speed;
            speed = 0;
            currentRotate = rotate;
            rotate = 0;

            // Rotation of the forklift model
            _forkliftModel.localEulerAngles = Vector3.Lerp(_forkliftModel.localEulerAngles, new Vector3(0, 90 + (turnInput * steerAnimationAmount), _forkliftModel.localEulerAngles.z), steerAnimationSpeed);

            // Rotation of the wheels
            foreach (var item in _frontWheels)
            {
                item.localEulerAngles = new Vector3(0, (turnInput * steering), item.localEulerAngles.z);
                item.localEulerAngles += new Vector3(sphere.velocity.magnitude / 2, 0, 0);
            }

            foreach (var item in _backWheels)
            {
                item.localEulerAngles += new Vector3(sphere.velocity.magnitude / 2, 0, 0);
            }
        }

        private void FixedUpdate()
        {
            // Forward Acceleration
            sphere.AddForce(_forkliftModel.transform.forward * currentSpeed, ForceMode.Acceleration);

            // Add Gravity
            sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

            // Steering
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

            Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out _, 1.1f, layerMask);
            Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out var hitNear, 2.0f, layerMask);

            // Normal Rotation
            _forkliftNormal.up = Vector3.Lerp(_forkliftNormal.up, hitNear.normal, Time.fixedDeltaTime * 8.0f);
            _forkliftNormal.Rotate(0, transform.eulerAngles.y, 0);
        }

        public void Steer(int direction, float amount)
        {
            rotate = (steering * direction) * amount;
        }
    }
}
