using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject obj;
    public GameObject player;

    WaitForSeconds waitForSeconds = new WaitForSeconds(5f);

    IEnumerator Start()
    {
        while (true)
        {
            // Place your method calls
            yield return waitForSeconds;

            var x = Random.value * 10;
            var z = Random.value * 10;
            var n = Instantiate(obj, this.transform.position + new Vector3(x, 0, z), Quaternion.identity);
            n.GetComponent<EnemyMovement>().player = player; // TODO Vuile shit
        }
    }
}
