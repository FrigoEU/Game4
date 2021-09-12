using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameObject player;
    public float speed;
    private Transform targetTransform;
    private Transform _transform;
    // Start is called before the first frame update
    void Start()
    {
        _transform = this.GetComponent<Transform>();
        targetTransform = player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 runningTo = targetTransform.position;
        Vector3 movement = runningTo - _transform.position;
        _transform.Translate(Vector3.Normalize(new Vector3(movement.x, 0, movement.z)) * speed * Time.deltaTime);
    }
}
