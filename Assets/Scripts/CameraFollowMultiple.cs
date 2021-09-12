using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowMultiple : MonoBehaviour
{

    public List<Transform> targets;
    private Transform _transform;
    private Camera _camera;
    public Vector3 offset = new Vector3(0, 5, -3);
    public float followSpeed = 1;
    public float lookSpeed = 1;
    public float minFov = 20;
    public float maxFov = 100;

    public float backening = 0.5f;
    public float heightening = 1;
    public float startMovingCameraAtDistance = 5;

    private Vector3 velocity;

    void Start()
    {
        _transform = this.GetComponent<Transform>();
        _camera = this.GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        targets.ForEach(t => bounds.Encapsulate(t.position));


        var targetPosition = bounds.center
            + offset
            // Move camera back and up as distance increases
            + (bounds.size.x > startMovingCameraAtDistance || bounds.size.y > startMovingCameraAtDistance ? new Vector3(
            0,
            Mathf.Max(((bounds.size.x + bounds.size.z) / 2 - startMovingCameraAtDistance) * heightening, 0),
            -Mathf.Max(((bounds.size.x + bounds.size.z) / 2 - startMovingCameraAtDistance) * backening, 0)
               ) : Vector3.zero);
        _transform.position = Vector3.SmoothDamp(_transform.position, targetPosition, ref velocity, followSpeed);

        // Smoothly rotate towards the target point.
        var targetRotation = Quaternion.LookRotation(bounds.center - _transform.position);
        _transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);

        _camera.fieldOfView = Mathf.Lerp(minFov, maxFov, (bounds.size.x + bounds.size.y) / 50);
    }
}
