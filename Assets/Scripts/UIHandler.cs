using System.Collections;
using System.Collections.Generic;
//using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public InputField IDInput;
    public string ID;
    public GameObject IDWarning, input, ConationPopup;
    public ExcelStore data;
    public ReceiveLiveStream rls;
    public GameObject red, green;
    public RestartData RD;
    public Button StartButton, calibrationContinue, ConationButton;
    public float conationValue, time;
    public Text conationTextValue, statusText;
    public Slider slider, conationSlider;
    public Image calibrationImage;


    public void Start()
    {
        RD = GetComponent<RestartData>();
        rls = GameObject.FindGameObjectWithTag("Data").GetComponent<ReceiveLiveStream>();
        data = GameObject.FindGameObjectWithTag("Data").GetComponent<ExcelStore>();
        //slider.maxValue = (float) data.BaseLineTime;
    }

    void Update()
    {
        //slider.value = data.loading;
        if (data.running)
        {
          time += Time.deltaTime;  
        }
        
        if (time >= 300)
        {
            GetConationLevel();
            time = 0;
        }

        if (conationValue > 0)
        {
            ConationButton.interactable = true;
        }

        if (rls.CalibrationSuccesful && !data.obtainingBaseline)
        {
            CalibrationDone();
        }

    }

    public void SetID()
    {
        data = GameObject.FindGameObjectWithTag("Data").GetComponent<ExcelStore>();


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

        if (IDInput.text == null)
        {
            StartButton.interactable = false;
        }
        else
        {
            StartButton.interactable = true;
        }
    }

    public void StartUp()
    {
        data = GameObject.FindGameObjectWithTag("Data").GetComponent<ExcelStore>();
        data.InitializeVariables();
        rls.PrepForTest();
    }

    public void StartCalibration()
    {
        rls.CalibrationTest();
    }

    public void RecordOn()
    {
        data.RecordOn();
        rls.StartRecording();
    }

    public void PauseRecord()
    {
        data.running = false;
    }

    public void UnPauseRecord()
    {
        data.running = true;
    }

    public void RecordOff()
    {
        data.PlaceLabels(conationValue);
        data.RecordOff();
        StartCoroutine(RestartData());
    }

    IEnumerator RestartData()
    {
        yield return new WaitForSeconds(2f);
        RD.DestroyData();
    }

    public void RecordBaseline()
    {
        data.ObtainBaseline();
    }

    public void ConationLevel()
    {
        conationValue = conationSlider.value;
        conationTextValue.text = conationValue.ToString();
    }

    public void GetConationLevel()
    {
        PauseRecord();
        ConationPopup.SetActive(true);        
    }

    public void ConationLevelContinue()
    {
        data.PlaceLabels(conationValue);
        ConationPopup.SetActive(false);
        UnPauseRecord();
        conationValue = 0;
    }

    public void CalibrationDone()
    {
        red.SetActive(false);
        green.SetActive(true);
        calibrationContinue.interactable = true;
    }
}