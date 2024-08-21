
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))] // 把有掛上Swimmer的物件都加上RigidBody
public class Swimmer : MonoBehaviour
{
    // [SerializeField] 類似public會顯示在inspector，但無法被其他script取用
    [SerializeField] float forwardSpeed = 2f;
    public float accSpeed = 2f;
    [SerializeField] float swimForce = 2f;
    [SerializeField] float dragForce = 1f;
    [SerializeField] float minForce;
    [SerializeField] float minTimeBetweenStroke;

    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference leftControllerVelocity;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] InputActionReference rightControllerVelocity;
    [SerializeField] Transform trackingReference;
    [SerializeField] Transform cameraTransf;


    Rigidbody _rigidbody;
    float _cooldownTimer;
    float acc;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        // _rigidbody.AddForce(cameraTransf.forward * forwardSpeed, ForceMode.Acceleration);


        _cooldownTimer += Time.fixedDeltaTime;
        // Debug.Log(_cooldownTimer);
        if (_cooldownTimer > minTimeBetweenStroke
            && leftControllerSwimReference.action.IsPressed()
            && rightControllerSwimReference.action.IsPressed()) 
        {
            var leftHandVelocity = leftControllerVelocity.action.ReadValue<Vector3>();
            var rightHandVelocity = rightControllerVelocity.action.ReadValue<Vector3>();
            Vector3 localVelocity = leftHandVelocity + rightHandVelocity;
            localVelocity *= -1;

            if (localVelocity.sqrMagnitude > minForce*minForce)
            {
                // AddForce只能加Global的Vector3，所以要將local轉為world
                Vector3 worldVelocity = trackingReference.TransformDirection(localVelocity);
                // _rigidbody.AddForce(worldVelocity * swimForce, ForceMode.Acceleration);
                acc = localVelocity.y*localVelocity.y;
                _rigidbody.AddForce(cameraTransf.forward * acc * accSpeed, ForceMode.Acceleration);
                _cooldownTimer = 0f;
            }
        }

        // 阻力
        if (_rigidbody.velocity.sqrMagnitude > 0.01f)
        {
            _rigidbody.AddForce(-_rigidbody.velocity * dragForce, ForceMode.Acceleration);
        }
    }
    
}
