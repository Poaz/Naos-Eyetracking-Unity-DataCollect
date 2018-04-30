using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class ExcelStore : MonoBehaviour
{
    //Privates
    private static string filePath;
    private string fileName, tempIntensities, tempSampleNumber, tempDuration;
    public bool running = false, obtainingBaseline, once = true;
    private int prevItems = 0;

    //Data Variables
    public string ID, testType;
    public List<double>  gdlX, gdlY, gdlZ, gdrX, gdrY, gdrZ, pdl, pdr, HR, HR_Buff, GSR, ConationLevels;
    //public double[] ConationLevels;
    public List<String> timeStamp, data;

    public DataGathering naos;

    public ReceiveLiveStream eyeData;

    public double BaseLineTime = 180,HR_Base;
    public float loading, time;
    


    void Start()
    {
        filePath = string.Concat(Application.dataPath, "/data/");
        naos = GetComponent<DataGathering>();
        eyeData = GetComponent<ReceiveLiveStream>();
        //obtainBaseline = true;
        loading = 0;
    }

	public void FixedUpdate()
	{
        if(naos.GetConnection() && obtainingBaseline && HR_Buff.Count < BaseLineTime && once)
        {          
            StartCoroutine(CalculateBaseline());
            once = false;
        }

		if (naos.GetConnection() && running)
		{
            HR.Add(naos.GetHeartRate()-HR_Base);
			GSR.Add(naos.GetGsr());
            print("running");
			timeStamp.Add(DateTime.Now.ToLongTimeString());
		}

        if(eyeData != null && running)
        {            
            gdlX.Add(eyeData.GetGDLX());
            gdlY.Add(eyeData.GetGDLY());
            gdlZ.Add(eyeData.GetGDLZ());
            gdrX.Add(eyeData.GetGDRX());
            gdrY.Add(eyeData.GetGDRY());
            gdrZ.Add(eyeData.GetGDRZ());
            pdl.Add(eyeData.GetPDL());
            pdr.Add(eyeData.GetPDR());
        }
	}

    private void Write(string fileName)
    {
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

		string[] lines = new string[gdlX.Count];

        string headers= "GDLX, GDLY,GDLZ, GDRX, GDRY, GDRZ, PL, PR, HR, GSR, ConationLevel";

        lines[0] = headers;

        print(HR.Count);
        print(GSR.Count);
        print(ConationLevels.Count);
        for (int i = 1; i < ConationLevels.Count; i++)
        {
            //lines[i] = HR[i].ToString() + "," + GSR[i].ToString() + "," + ConationLevels[i].ToString();
            lines [i] =  gdlX[i].ToString() + ","+ gdlY[i].ToString() +","+ gdlZ[i].ToString() +","+ gdrX[i].ToString() +","+ gdrY[i].ToString() +","+ gdrZ[i].ToString() +","+ pdl[i].ToString() +","+ pdr[i].ToString()+ "," + HR[i].ToString() + ","  + GSR[i].ToString() + "," + ConationLevels[i].ToString();
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
        gdlX = null;
        gdlY = null;
        gdlZ = null;
        gdrX = null;
        gdrY = null;
        gdrZ = null;
        pdl = null;
        pdr = null;
		HR = null;
		GSR = null;
		fileName = null;
		ID = null;
		running = false;
        
    }

    public void InitializeVariables()
    {
        gdlX = new List<double>();
        gdlY = new List<double>();
        gdlZ = new List<double>();
        gdrX = new List<double>();
        gdrY = new List<double>();
        gdrZ = new List<double>();
        pdl = new List<double>();
        pdr = new List<double>();
        HR = new List<double>();
        GSR = new List<double>();
        filePath = string.Concat(Application.dataPath, "/data/");
        naos = this.GetComponent<DataGathering>();
        //obtainBaseline = true;
    }

    public IEnumerator CalculateBaseline()
    {
         time = 0;

        while (time < 20)
        {
            time += Time.deltaTime;
            HR_Buff.Add(naos.GetHeartRate());
	        GSR.Add(naos.GetGsr());
            loading = HR_Buff.Count;
            yield return new WaitForSeconds(0.001f);
            print(naos.GetHeartRate());
        }
        HR_Base = HR_Buff.Average(); //move decimal point 2 
        obtainingBaseline = false;
    }

    public void ObtainBaseline()
    {
        HR_Buff = new List<double>();
        HR_Base = 0;
        obtainingBaseline = true;
        //once = true;
    }

    public void PlaceLabels(float ConationLevel)
    {
        print(prevItems);
        var items = gdlX.Count;
        print(items);
        for (int i = prevItems; i < items-1; i++)
        {
            print(ConationLevel);
            ConationLevels.Add(ConationLevel);

        }
        prevItems = gdlX.Count;
    }
}
