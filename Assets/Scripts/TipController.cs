using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipController : MonoBehaviour {

	public GameObject tip;

	float elapsedTime;

	public bool going = false;

	// Use this for initialization
	void Start () {
		tip.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (!going)
			return;
		elapsedTime += Time.deltaTime;
		if (elapsedTime < 1.0f)
			tip.SetActive (false);
		else if (elapsedTime < 4.0f)
			tip.SetActive (true);
		else
			tip.SetActive (false);
	}

	public void go() {
		going = true;
		tip.SetActive (true);
	}
}
