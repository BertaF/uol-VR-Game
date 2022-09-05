using UnityEngine;

namespace Assets.Scripts
{
    public class ForkliftControllerInput : MonoBehaviour
    {
        public float Speed, CurrentSpeed;
        public float Rotate, CurrentRotate;

        [SerializeField] private Transform _forkliftNormal;
        [SerializeField] private Rigidbody _rbChassis;

        [Header("Steering Input")]
        [SerializeField] public float _turnInput;
        [SerializeField] public bool _speedInput;
        [SerializeField] public bool _reverseInput;

        [Header("Steering Parameters")]
        [SerializeField] private float _acceleration = 20f;
        [SerializeField] private float _revAcceleration = 10f;
        [SerializeField] private float _steering = 10f;
        [SerializeField] private float _gravity = 25f;
        [SerializeField] private float _steerAnimationSpeed = 0.1f;
        [SerializeField] private float _steerAnimationAmount = 8;

        [SerializeField] private LayerMask _layerMask;

        [Header("Model Parts")]
        [SerializeField] private Transform[] _frontWheels;
        [SerializeField] private Transform[] _backWheels;

        private void Update()
        {
            // Set this transform position to be the same as the chassis collider
            transform.position = _rbChassis.transform.position;

            // Store the speed input when detected
            if (_speedInput)
                Speed = _acceleration;

            // Store the reverse input when detected
            if (_reverseInput)
                Speed = -_revAcceleration;

            // Set the steering direction if rotational input is detected
            if (_turnInput != 0.0f)
            {
                int dir = _turnInput > 0.0f ? 1 : -1;
                float amount = Mathf.Abs((_turnInput));
                Steer(dir, amount);
            }

            SetCurrentSpeedAndRotation();

            _forkliftNormal.localEulerAngles = Vector3.Lerp(_forkliftNormal.localEulerAngles,
                new Vector3(0.0f, 90.0f + (_turnInput * _steerAnimationAmount), _forkliftNormal.localEulerAngles.z), _steerAnimationSpeed);

            // Update the wheels rotation (front)
            foreach (var item in _frontWheels)
            {
                item.localEulerAngles = new Vector3(0.0f, (_turnInput * _steering), item.localEulerAngles.z);
                item.localEulerAngles += new Vector3(_rbChassis.velocity.magnitude / 2, 0, 0);
            }

            // Update the wheels rotation (rear)
            foreach (var item in _backWheels)
            {
                item.localEulerAngles += new Vector3(_rbChassis.velocity.magnitude / 2, 0, 0);
            }
        }

        private void FixedUpdate()
        {
            // Add a forward force to the vehicle chassis
            _rbChassis.AddForce(_forkliftNormal.transform.forward * CurrentSpeed, ForceMode.Acceleration);

            // Add a gravity force (down) to the vehicle chassis
            _rbChassis.AddForce(Vector3.down * _gravity, ForceMode.Acceleration);

            // Gradually rotate this transform in the direction given by the steering input
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + CurrentRotate, 0), Time.deltaTime * 5.0f);

            // Making sure the vehicle normal is upright before rotating in the directin of the steering input
            Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out _, 1.1f, _layerMask);
            Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out var hitNear, 2.0f, _layerMask);

            _forkliftNormal.up = Vector3.Lerp(_forkliftNormal.up, hitNear.normal, Time.fixedDeltaTime * 8.0f);
            _forkliftNormal.Rotate(0.0f, transform.eulerAngles.y, 0.0f);
        }

        public void Steer(int direction, float amount)
        {
            Rotate = (_steering * direction) * amount;
        }

        private void SetCurrentSpeedAndRotation()
        {
            CurrentSpeed = Speed;
            Speed = 0.0f;
            CurrentRotate = Rotate;
            Rotate = 0.0f;
        }
    }
}
