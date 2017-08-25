using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwap : MonoBehaviour
{

    Collider playerCollider;
    List<GameObject> enemies = new List<GameObject>();

    void Start ()
    {
        playerCollider = GameObject.Find("Player").GetComponent<Collider>();
	}

    void OnTriggerEnter(Collider other)
    {
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        Debug.Log("enemies");

        if (other.gameObject.tag == "Player" && enemies == null)
        {
            GameObject.Find("End of Level Screen").SetActive(true);
        }
    }



	void Update ()
    {
		
	}
}
