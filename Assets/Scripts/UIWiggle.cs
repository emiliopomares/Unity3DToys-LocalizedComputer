using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWiggle : MonoBehaviour {

	public float xAmp;
	public float xFreq;
	public float yAmp;
	public float yFreq;

	float xphase;
	float yphase;

	Vector2 initialPos;

	// Use this for initialization
	void Start () {
		xphase = yphase;
		initialPos = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		xphase += (xFreq) * Time.deltaTime;
		yphase += (yFreq) * Time.deltaTime;
		this.transform.localPosition = initialPos + new Vector2 (xAmp * Mathf.Sin (xphase), yAmp * Mathf.Sin (yphase));
	}
}
