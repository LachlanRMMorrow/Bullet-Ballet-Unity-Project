using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashAutoDestroy : MonoBehaviour {

    int counter = 3;


	
    void Update()
    {
        counter -= 1;

        if (counter <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
