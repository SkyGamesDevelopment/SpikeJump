using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour {

	#region variables
	[SerializeField] private GameObject player;
	[SerializeField] private float speed, distanceTolerance;
	#endregion

	private void Start ()
	{
		//Seraching for player
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void Update ()
	{
		//Checking is player in range
		if (Vector3.Distance(this.transform.position, player.transform.position) <= distanceTolerance && player.GetComponent<PlayerScript>().activeBoostID == 2)
			//Adding position to coin
			this.transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
	}
}
