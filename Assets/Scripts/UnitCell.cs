using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCell : MonoBehaviour {

	public Texture[] hexDigit;
	public Texture s0;
	public Texture s1_0;
	public Texture s1_1;

	public int value;

	public GameObject leftDigitMat;
	public GameObject rightDigitMat;
	public GameObject statusLED;

	public bool bcd;

	// Use this for initialization
	void Start () {
		statusLED.GetComponent<Renderer> ().material.color = Color.red;
	}

	public void setStatusLED(bool running) {
		if (running) {
			statusLED.GetComponent<Renderer> ().material.color = Color.green;
		} else {
			statusLED.GetComponent<Renderer> ().material.color = Color.red;
		}
	}
	
	// Update is called once per frame
	void Update () {
		updateValue ();
	}

	public void updateValue() {

		//special cases:
		if (value == 255) { // void
			leftDigitMat.GetComponent<Renderer> ().material.mainTexture = s0;
			rightDigitMat.GetComponent<Renderer> ().material.mainTexture = s0;
		} else if (value == 254) { // full
			leftDigitMat.GetComponent<Renderer> ().material.mainTexture = s1_0;
			rightDigitMat.GetComponent<Renderer> ().material.mainTexture = s1_1;
		} else {

			if (!bcd) {
				int byteData = value & 0xff;
				int highNibble = (byteData & 0xf0) >> 4;
				int lowNibble = byteData & 0x0f;
				leftDigitMat.GetComponent<Renderer> ().material.mainTexture = hexDigit [highNibble];
				rightDigitMat.GetComponent<Renderer> ().material.mainTexture = hexDigit [lowNibble];
			} else {
				int highDigit = (value % 100) / 10;
				int lowDigit = value % 10;
				leftDigitMat.GetComponent<Renderer> ().material.mainTexture = hexDigit [highDigit];
				rightDigitMat.GetComponent<Renderer> ().material.mainTexture = hexDigit [lowDigit];
			}

		}
	}
}
