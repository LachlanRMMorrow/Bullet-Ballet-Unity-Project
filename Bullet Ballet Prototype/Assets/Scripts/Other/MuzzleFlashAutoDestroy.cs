using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashAutoDestroy : MonoBehaviour {

    int counter = 3;


	
    void Update()
    {
            Destroy(this.gameObject, 5.0f);
    }
}
