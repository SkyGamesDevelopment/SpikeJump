using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTrackScript : MonoBehaviour {

	#region variables
	public GameObject trackObject;
	public float xOffset, yOffset, zOffset;
	public float effectDelay;
	#endregion
	private void Update ()
	{
		if (effectDelay <= 0)
			Destroy(this.gameObject);
		else
			effectDelay -= Time.deltaTime;
		//Tracking an object and adding a offset values
		this.transform.position = new Vector3(trackObject.transform.position.x + xOffset, trackObject.transform.position.y + yOffset, zOffset);
	}
}
