using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevelDoor : MonoBehaviour {

    public List<GameObject> test = new List<GameObject>();
    public GameObject test2;

	void Start ()
    {
        test = GameObject.Find("World Text").transform.GetChild(0).GetComponent<ExitWorldText>().enemies;
        test2 = GetComponent<Door>().gameObject;
        test2.SetActive(false);
	}
	
	void Update ()
    {
		if (test == null)
        {
            test2.SetActive(true);
        }
	}
}
