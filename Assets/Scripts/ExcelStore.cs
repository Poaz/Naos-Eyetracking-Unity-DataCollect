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
    public bool running = false, obtainingBaseline;

    //Data Variables
    public string ID, testType;
    public List<double>  gdlX,gdlY,gdlZ,gdrX,gdrY,gdrZ,pdl,pdr,  HR, HR_Buff, HRAVG, HRAVG_Buff, HRMAX_Buff, HRMAX, GSR;
    public List<String> timeStamp;

    public List<string> data;

    public DataGathering naos;

    public ReceiveLiveStream eyeData;

    public double BaseLineTime = 180,HR_Base, HRAVG_Base, HRMAX_Base;

    public float loading;

  //  StreamWriter streamWriter ;

    void Start()
    {
        filePath = string.Concat(Application.dataPath, "/data/");
        naos = GetComponent<DataGathering>();
        eyeData = GetComponent<ReceiveLiveStream>();
        //obtainBaseline = true;

        loading = 0;
    }

	public void FixedUpdate(){

        if(naos.GetConnection() && obtainingBaseline && HR_Buff.Count < BaseLineTime)
        {
           
            StartCoroutine(CalculateBaseline());
        }


		if (naos.GetConnection() && running)
		{
          
            //withdraw baseline average
            HR.Add(naos.GetHeartRate()-HR_Base);
			HRAVG.Add(naos.GetAvgHeartRate()-HRAVG_Base);
			HRMAX.Add(naos.GetMaxHeartRate()-HRMAX_Base);
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
        //obtainBaseline = true;
    }

    public IEnumerator CalculateBaseline()
    {

        
        HR_Buff.Add(naos.GetHeartRate());
        HRAVG_Buff.Add(naos.GetAvgHeartRate());
	    HRMAX_Buff.Add(naos.GetMaxHeartRate());
	    GSR.Add(naos.GetGsr());

        loading = HR_Buff.Count;

        yield return new WaitForSeconds(1);

        if(HR_Buff.Count == BaseLineTime)
        {

            for (int i = 0; i < HR_Buff.Count; i++)
            {
                HR_Base += HR_Buff[i];
                HRAVG_Base += HRAVG_Buff[i];
                HRMAX_Base += HRMAX_Buff[i];
            }

            HR_Base /= HR_Buff.Count;
            HRAVG_Base /= HRAVG_Buff.Count;
            HRMAX_Base /= HRMAX_Buff.Count; 

            obtainingBaseline = false;
        }
    }

    public void ObtainBaseline()
    {
        HR_Buff = new List<double>();
        HRAVG_Buff = new List<double>();
        HRMAX_Buff =  new List<double>();
        HR_Base = 0;
        HRAVG_Base = 0;
        HRMAX_Base  = 0;
        obtainingBaseline = true;
        
    }

}
