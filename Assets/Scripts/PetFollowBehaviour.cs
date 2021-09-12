using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFollowBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    private Transform targetTransform;
    private Transform _transform;
    private bool moving;
    public float maxDistance = 5;
    public float followSpeed = 1;
    public Vector3 offset = new Vector3(-2, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        _transform = this.GetComponent<Transform>();
        targetTransform = target.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(_transform.position, targetTransform.position) > maxDistance){
            moving = true;
        }
        if (target.GetComponent<Rigidbody>().IsSleeping() && (Vector3.Distance(_transform.position, targetTransform.position) < offset.magnitude * 2))
        {
            moving = false;
        }
        Vector3 runningTo = targetTransform.position + (targetTransform.forward * offset.z) + (targetTransform.right * offset.x) + (targetTransform.up * offset.z);
        Vector3 movement = runningTo - _transform.position;
        if (moving){
            _transform.Translate(Vector3.Normalize(new Vector3(movement.x, 0, movement.z)) * followSpeed * Time.deltaTime);
        }
    }
}
