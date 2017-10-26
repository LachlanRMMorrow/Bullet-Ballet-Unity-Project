using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour 
{
	public GameObject creditsHolder;
	public Transform creditsStartTrans;
	public Transform creditsEndTrans;

	public float scrollSpeed = 1.0f;
	private float startTime;
	private float journeyLength;

	private float timer;
	public float timeToPass = 3;

	// Use this for initialization
	void Start () 
	{
		startTime = Time.time;
		timer = Time.time;
		journeyLength = Vector3.Distance (creditsStartTrans.position, creditsEndTrans.position);
	}
	
	// Update is called once per frame
	void Update () 
	{
		float distCovered = (Time.time - startTime) * scrollSpeed;
		float fracJourney = distCovered / journeyLength;
		creditsHolder.GetComponent<RectTransform>().position = Vector3.Lerp (creditsStartTrans.position, creditsEndTrans.position, fracJourney);

		if (Time.time - timer > timeToPass) 
		{
			SceneManager.LoadScene (0);
		}
	}
}
