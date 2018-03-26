using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazeTracker : MonoBehaviour {

	public ReceiveLiveStream RLS; 
	public TextMesh textMesh;

	private float gdlx, gdly, gdlz;




	// Use this for initialization
	void Start () {

		RLS = GetComponent<ReceiveLiveStream>();
		
		
	}
	
	// Update is called once per frame
	void Update () {

		
		gdlx = (float) RLS.GetGDLX();
		gdly = (float) RLS.GetGDLY();
		gdlz = (float) RLS.GetGDLZ();

		transform.position = new Vector3(gdlx,gdly,gdlz);

		textMesh.text = gdlx + "," + gdly +","+ gdlz;

		
	}
}
