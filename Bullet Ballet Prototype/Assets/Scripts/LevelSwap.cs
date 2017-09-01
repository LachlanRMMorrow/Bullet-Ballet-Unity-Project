using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelSwap : MonoBehaviour
{
    public GameObject endOfLevelScreen;
    public GameObject nextLevel;
    public Collider playerCollider;
    List<GameObject> enemies = new List<GameObject>();

    void Start ()
    {
       // playerCollider = GameObject.Find("Player").GetComponent<Collider>();
	}

    void OnTriggerEnter(Collider other)
    {
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        Debug.Log("Hit");

        if (other == playerCollider && enemies.Count <= 0)
        {
            
            Debug.Log(enemies);
            endOfLevelScreen.SetActive(true);
            GameObject eS = GameObject.Find("EventSystem");
            eS.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(nextLevel);
            eS.GetComponent<EndofLevelMenu>().EndOfLeveActive();
        }

        enemies.Clear();
    }

    

	void Update ()
    {
		
	}
}
