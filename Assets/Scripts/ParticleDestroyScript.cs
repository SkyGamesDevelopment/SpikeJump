using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyScript : MonoBehaviour {

	private ParticleSystem ps;

	void Start ()
	{
		ps = this.GetComponent<ParticleSystem>();
	}
	void Update ()
	{
		//Checking is PS is alive
		if (!ps.IsAlive())
		{
			//Checking that is having parent
			if (this.transform.parent)
				Destroy(this.transform.parent.gameObject);
			else
				Destroy(this.gameObject);
		}
	}
}
