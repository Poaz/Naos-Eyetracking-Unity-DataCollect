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
    public List<double>  gdlX,gdlY,gdlZ,gdrX,gdrY,gdrZ,pdl,pdr,  HR, HRAVG, HRMAX, GSR;
    public List<String> timeStamp;

    public List<string> data;

    public DataGathering naos;

    public ReceiveLiveStream eyeData;

  //  StreamWriter streamWriter ;

    void Start()
    {
        filePath = string.Concat(Application.dataPath, "/data/");


        naos = GetComponent<DataGathering>();
        eyeData = GetComponent<ReceiveLiveStream>();
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

        if(eyeData != null){

            
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

        string headers= "GDLX, GDLY,GDLZ, GDRX, GDRY, GDRZ, PL, PR, HR, HRAVG, GSR";

      // using ( StreamWriter streamWriter = new StreamWriter(filePath + fileName + ".txt") ){

           lines[0] = headers;

           for (int i = 1; i < gdlX.Count; i++)
        {
            //ID + "," + timeStamp [i] + ","  + 
			//lines [i] = gdlX[i].ToString() + ","+ gdlY[i].ToString() +","+ gdlZ[i].ToString() +","+ gdrX[i].ToString() +","+ gdrY[i].ToString() +","+ gdrZ[i].ToString() +","+ pdl[i].ToString() +","+ pdr[i].ToString();
            // + HR[i].ToString() + "," + HRAVG[i].ToString() + "," + HRMAX[i].ToString() + "," + GSR[i].ToString(); //naos

             
			lines [i] =  gdlX[i].ToString() + ","+ gdlY[i].ToString() +","+ gdlZ[i].ToString() +","+ gdrX[i].ToString() +","+ gdrY[i].ToString() +","+ gdrZ[i].ToString() +","+ pdl[i].ToString() +","+ pdr[i].ToString()+ "," + HR[i].ToString() + "," + HRAVG[i].ToString() + "," + GSR[i].ToString();
            // //naos
            
            
          //  streamWriter.WriteLine(lines[i]);
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
		HRAVG = null;
		HRMAX = null;
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
        HRAVG = new List<double>();
        HRMAX = new List<double>();
        GSR = new List<double>();
        filePath = string.Concat(Application.dataPath, "/data/");
        naos = this.GetComponent<DataGathering>();
    }
}
