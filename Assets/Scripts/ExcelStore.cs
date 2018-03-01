using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ExcelStore : MonoBehaviour
{

    //Privates
    private static string filePath;
    private string fileName, tempIntensities, tempSampleNumber, tempDuration;
    public bool running = false;

    //Data Variables
    public string ID, testType;
    public List<double> HR, HRAVG, HRMAX, GSR;
    public List<String> timeStamp;

    public List<string> data;

    public DataGathering naos;

    void Start()
    {
        filePath = string.Concat(Application.dataPath, "/data/");
        naos = GetComponent<DataGathering>();
    }

	public void FixedUpdate(){
		if (naos.GetConnection() && running)
		{
			HR.Add(naos.GetHeartRate());
			HRAVG.Add(naos.GetAvgHeartRate());
			HRMAX.Add(naos.GetMaxHeartRate());
			GSR.Add(naos.GetGsr());
			timeStamp.Add(DateTime.Now.ToLongTimeString());
		}
	}

    private void Write(string fileName)
    {
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

		string[] lines = new string[HR.Count + 1];

        lines[0] = "ID, Timestamp, HR, HRAVG, HRMAX, GSR";

		for (int i = 1; i < HR.Count; i++)
        {
			lines [i] = ID + "," + timeStamp [i] + "," + HR[i].ToString() + "," + HRAVG[i].ToString() + "," + HRMAX[i].ToString() + "," + GSR[i].ToString();

        }
        File.WriteAllLines(filePath + fileName + ".txt", lines);
		Reset();
        lines = null;
    }

    public void RecordOff()
    {
        running = false;
        fileName = "Data" + ID;
        Write(fileName);
    }

    public void RecordOn()
    {
        InitializeVariables();
        running = true;
    }

    public void Reset()
    {
		HR = null;
		HRAVG = null;
		HRMAX = null;
		GSR = null;
		fileName = null;
		ID = null;
		running = false;
        
    }

    public void InitializeVariables()
    {
        HR = new List<double>();
        HRAVG = new List<double>();
        HRMAX = new List<double>();
        GSR = new List<double>();
        filePath = string.Concat(Application.dataPath, "/data/");
        naos = this.GetComponent<DataGathering>();
    }
}
