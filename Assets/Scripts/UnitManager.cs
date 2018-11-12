using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour {

	public Camera theCamera;
	public LCProgram sharedProgram;
	public ProgramController programController;

	public InputField addrField;
	public InputField dataField;
	public InputField sourceField;

	public Transform unitsParent;

	public GameObject unitPrefab;

	public string[] initialSource;

	// Use this for initialization
	void Start () {
		checkConnections ();
		string initialProgram = "";
		for (int i = 0; i < initialSource.Length; ++i) {
			initialProgram += initialSource [i];
			if (i < (initialSource.Length - 1))
				initialProgram += "\n";
		}
		sourceField.text = initialProgram;
		programController.compile ();
	}

	bool picked = false;

	GameObject newUnitGO;

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {

				if (hit.collider.tag == "Unit") {
					picked = true;
					newUnitGO = hit.collider.gameObject;
				}

			}
		}
		if (Input.GetMouseButtonDown (1)) {
			picked = true;
			newUnitGO = (GameObject)Instantiate (unitPrefab);
			newUnitGO.transform.SetParent (unitsParent);
			newUnitGO.GetComponentInChildren<LCProcessingUnit> ().attachProgram (sharedProgram);
		}
		if (Input.GetMouseButtonUp (0)) {
			picked = false;
			checkConnections ();
		}
		if (Input.GetMouseButtonUp (1)) {
			picked = false;
			checkConnections ();
		}
		if (picked) {
			RaycastHit hit;
			Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				
				if (hit.collider.tag == "Grid") {
					float x = hit.point.x;
					float z = hit.point.z;
					newUnitGO.transform.position = new Vector3 (Mathf.Round (x), 0, Mathf.Round (z));
				}
					
			}
		}
	}

	private int manhattanDistance(int x1, int z1, int x2, int z2) {
		int deltax = x2 - x1;
		int deltaz = z2 - z1;
		if (deltax < 0)
			deltax = -deltax;
		if (deltaz < 0)
			deltaz = -deltaz;

		return deltax + deltaz;
	}

	public void checkConnections() {
		LCProcessingUnit[] units = unitsParent.GetComponentsInChildren<LCProcessingUnit> ();
		for (int i = 0; i < units.Length; ++i) {
			units [i].rightUnit = null;
			units [i].topUnit = null;
			units [i].bottomUnit = null;
			units [i].leftUnit = null;
		}
		for (int i = 0; i < units.Length; ++i) {
			int x1 = (int)units [i].transform.parent.position.x;
			int z1 = (int)units [i].transform.parent.position.z;
			for (int j = i+1; j < units.Length; ++j) {
				
				int x2 = (int)units [j].transform.parent.position.x;
				int z2 = (int)units [j].transform.parent.position.z;
				int deltax = x2 - x1;
				int deltaz = z2 - z1;

				// 4 cases:
				if ((deltax == 1) && (deltaz == 0)) {
					units [i].rightUnit = units [j];
					units [j].leftUnit = units [i];
				}
				if ((deltax == -1) && (deltaz == 0)) {
					units [i].leftUnit = units [j];
					units [j].rightUnit = units [i];
				}
				if ((deltax == 0) && (deltaz == 1)) {
					units [i].topUnit = units [j];
					units [j].bottomUnit = units [i];
				}
				if ((deltax == 0) && (deltaz == -1)) {
					units [i].bottomUnit = units [j];
					units [j].topUnit = units [i];
				}

			}
		}
	}

	public void setData() {
		string addrep = addrField.text;
		string srep = dataField.text;
		int addr;
		int.TryParse (addrep, out addr);
		int idata;
		float fdata;
		if(int.TryParse(srep, out idata)) {
			newUnitGO.GetComponentInChildren<LCProcessingUnit> ().iMemory [addr] = idata;
			newUnitGO.GetComponentInChildren<LCProcessingUnit> ().physicalCell.value = newUnitGO.GetComponentInChildren<LCProcessingUnit> ().iMemory [0]; // update LED
		}
		else if(float.TryParse(srep, out fdata)) {
			newUnitGO.GetComponentInChildren<LCProcessingUnit> ().fMemory [addr] = fdata;
		}
	}
}
