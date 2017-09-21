using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelSwap : MonoBehaviour
{
    public GameObject endOfLevelScreen;
    public int nextLevel;
    public Collider playerCollider;
    List<GameObject> enemies = new List<GameObject>();

    void Awake ()
    {
        endOfLevelScreen = GameObject.Find("Canvas").transform.Find("End of Level Screen").gameObject;
        nextLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        
	}

    void OnTriggerEnter(Collider other)
    {
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        if (other == playerCollider && enemies.Count <= 0)
        {
            
            Debug.Log(enemies);
            endOfLevelScreen.SetActive(true);
            GameObject eS = GameObject.Find("EventSystem");
            //eS.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(nextLevel);
            SlowMoManager.m_isPaused = true;
            eS.GetComponent<EndofLevelMenu>().EndOfLeveActive();
            endOfLevelScreen.transform.Find("Exit To Desktop ES").GetComponent<UnityEngine.UI.Button>().Select();
            endOfLevelScreen.transform.Find("Next Level ES").GetComponent<UnityEngine.UI.Button>().Select();
        }

        enemies.Clear();
    }

  
}
