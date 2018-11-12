using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutFader : MonoBehaviour {

	float op = 1.0f;
	float targetop = 1.0f;

	public float speed;

	Text theText;
	Image theImage;

	// Use this for initialization
	void Start () {
		theText = this.GetComponent<Text> ();
		theImage = this.GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (targetop < op) {
			op -= Time.deltaTime * speed;
			if (op < 0.0f) {
				op = 0;
				Destroy (this.gameObject);
			}
			if (theText != null)
				theText.color = new Color (theText.color.r, theText.color.g, theText.color.b, op);
			if(theImage != null)
				theImage.color = new Color (theImage.color.r, theImage.color.g, theImage.color.b, op);
			
		}
	}


	public void fadeout() {
		targetop = 0.0f;
	}

}
