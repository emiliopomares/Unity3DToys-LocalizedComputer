using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramController : MonoBehaviour {

	public InputField editorSource;
	public LCProgram sharedProgram;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void compile() {
		sharedProgram.compile (editorSource.text);
	}      

	public void stopAll() {
		LCProcessingUnit[] units = GameObject.FindObjectsOfType<LCProcessingUnit> ();
		foreach (LCProcessingUnit u in units)
			u.setRunningState (false);
	}

	public void runAll() {
		LCProcessingUnit[] units = GameObject.FindObjectsOfType<LCProcessingUnit> ();
		foreach (LCProcessingUnit u in units) {
			//u.reset ();
			u.PC = 0;
			u.setRunningState (true);
		}
	}
}
