using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
public class DisplayValue : MonoBehaviour {

	public ReceiveLiveStream eyeData;

	public Text text;


	// Use this for initialization
	void Start () {

		text = GetComponent<Text>();
		
	}
	
	// Update is called once per frame
	void Update () {

		text.text = eyeData.GetGDLX().ToString();
		
	}
}
