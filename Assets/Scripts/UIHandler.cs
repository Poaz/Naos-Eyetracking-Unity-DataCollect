using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
    public InputField IDInput;
    public string ID;
    public GameObject IDWarning;
    public ExcelStore data;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetID()
    {
        if (IDInput.text != PlayerPrefs.GetString("ID"))
        {
            PlayerPrefs.SetString("ID", IDInput.text);
            PlayerPrefs.Save();
            data.ID = IDInput.text;
            ID = IDInput.text;
            IDWarning.SetActive(false);
        }
        else
        {
            IDWarning.SetActive(true);
        }
    }
}
