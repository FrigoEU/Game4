using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillsEnemies : MonoBehaviour
{
    void OnCollisionEnter(Collision collision){
        EnemyMovement c;
        if (collision.gameObject.TryGetComponent<EnemyMovement>(out c)){
            Destroy(collision.gameObject);
        }
    }
}
