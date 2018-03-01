using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
    public InputField IDInput;
    public string ID;
    public GameObject IDWarning, b1G, b2G, input;
    public ExcelStore data;
    public GameObject red, green;
    public RestartData RD;
    public Button b1, b2;

    public void Start()
    {
        RD = GetComponent<RestartData>();   
    }

    public void SetID()
    {
        data = GameObject.FindGameObjectWithTag("Data").GetComponent<ExcelStore>();

;       if (IDInput.text != PlayerPrefs.GetString("ID"))
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

        if (IDInput.text == null)
        {
            b1.interactable = false;
            b2.interactable = false;
        } else
        {
            b1.interactable = true;
        }
    }

    public void RecordOn()
    {
        red.SetActive(false);
        green.SetActive(true);
        input.SetActive(false);
        b1.interactable = false;
        b2.interactable = true;
        data.RecordOn();
    }

    public void RecordOff()
    {
        data.RecordOff();
        red.SetActive(true);
        green.SetActive(false);
        StartCoroutine(RestartData());
        input.SetActive(true);
        b1.interactable = true;
        b2.interactable = false;
    }

    IEnumerator RestartData()
    {
        yield return new WaitForSeconds(2f);
        RD.DestroyData();
    }
}
