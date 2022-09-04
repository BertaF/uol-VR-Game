using UnityEngine;

public class ForkliftControllerInput : MonoBehaviour
{
    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;

    float speed, currentSpeed;
    float rotate, currentRotate;

    [Header("INPUT")]
    public float turnInput;
    public bool speedInput;
    public bool reverseInput;

    [Header("Parameters")]
    public float acceleration = 30f;
    public float revAcceleration = 10f;
    public float steering = 80f;
    public float gravity = 10f;
    public LayerMask layerMask;
    public float steerAnimationSpeed = 0.1f;
    public float steerAnimationAmount = 10;

    [Header("Model Parts")]
    public Transform[] frontWheels;
    public Transform[] backWheels;

    private void Update()
    {
        // Follow Collider
        transform.position = sphere.transform.position;

        // Accelerate
        if (speedInput)
            speed = acceleration;

        // Reverse
        if (reverseInput)
            speed = -revAcceleration;

        // Steer
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
        //Animations

        //a) Kart
        kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90 + (turnInput * steerAnimationAmount), kartModel.localEulerAngles.z), steerAnimationSpeed);

        //b) Wheels
        foreach (var item in frontWheels)
        {
            item.localEulerAngles = new Vector3(0, (turnInput * steering), item.localEulerAngles.z);
            item.localEulerAngles += new Vector3(sphere.velocity.magnitude / 2, 0, 0);
        }

        foreach (var item in backWheels)
        {
            item.localEulerAngles += new Vector3(sphere.velocity.magnitude / 2, 0, 0);
        }
    }

    private void FixedUpdate()
    {
        // Forward Acceleration
        sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);

        // Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        // Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out _, 1.1f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out var hitNear, 2.0f, layerMask);

        // Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.fixedDeltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    public void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }
}
