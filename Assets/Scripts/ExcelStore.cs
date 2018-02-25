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

    public void Update()
    {
        if (naos.GetConnection() && running)
        {
            StartCoroutine(DataFetch());
            running = false;
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
            lines[i] = ID + "," + timeStamp[i] + "," + HR[i].ToString() + "," + HRAVG[i].ToString() + "," + HRMAX[i].ToString() + "," + GSR[i].ToString();

        }
        File.WriteAllLines(filePath + fileName + ".txt", lines);
        Debug.Log("writing data");
    }

    private IEnumerator DataFetch()
    {
        while (true)
        {
            HR.Add(naos.GetHeartRate());
            HRAVG.Add(naos.GetAvgHeartRate());
            HRMAX.Add(naos.GetMaxHeartRate());
            GSR.Add(naos.GetGsr());
            timeStamp.Add(DateTime.Now.ToLongTimeString());
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void RecordOff()
    {
        running = false;
        fileName = "Data" + ID;
        Write(fileName);
    }

    public void RecordOn()
    {
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
    }
}
