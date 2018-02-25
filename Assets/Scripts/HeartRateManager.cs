/*
 in this script the heart rate data is managed. 
 it provides an example of how to stream it further to other applications. 
 it provides example of how to "record it" and save it in files afterwards.
 it has basic functions for analysis, which can be used in this system as well.
 These functions are fundamental for psychophysiological analysis
 It is finding the baseline for the different heart rate measures and GSR
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using System.Text;

public class HeartRateManager : MonoBehaviour {
	private DataGathering stream;
	//private UDPDoubleArrSend udpSender; 
	//private UDPSend triggerSender;

	private double[] biometrics = new double[4]; 
	private ArrayList hrArrList = new ArrayList(); 
	private ArrayList avgHrArrList = new ArrayList(); 
	private ArrayList maxHrArrList = new ArrayList(); 
	private ArrayList gsrArrList = new ArrayList();
	private ArrayList timeList = new ArrayList(); 

	private ArrayList hrCSArrList = new ArrayList(); 
	private ArrayList avgHrCSArrList = new ArrayList(); 
	private ArrayList maxHrCSArrList = new ArrayList(); 
	private ArrayList gsrCSArrList = new ArrayList();
	private ArrayList timeCSList = new ArrayList(); 

	private ArrayList hrPDArrList = new ArrayList(); 
	private ArrayList avgHrPDArrList = new ArrayList(); 
	private ArrayList maxHrPDArrList = new ArrayList(); 
	private ArrayList gsrPDArrList = new ArrayList();

	private ArrayList triggers = new ArrayList();
	private ArrayList triggersWithBaseLine = new ArrayList();

	private float time = 0.0f;
	private int participantID = 1; 
	private string directoryPath;
	private double hrBase = 0.0;
	private double avghrBase = 0.0;
	private double maxhrBase = 0.0;
	private double gsrBase = 0.0;
	private bool captureBaseline = true;
	private double baselineTime = 10.0; 
	private bool printTxts = true; 
	private bool triggerEnabled = true; 
	// Use this for initialization
	void Start () {
		// let's first get access to the other components of this object 
		//udpSender = GetComponent<UDPDoubleArrSend> ();
		//triggerSender = GetComponent<UDPSend> ();
		stream = GetComponent<DataGathering> ();
		// This is for the purpose of separating the different subjects you have recorded 
		// each time the program is restarted it creates a new folder for the new participant, 
		// in this way you know exactly which participant you are working with and the number that has been through.  
		while(Directory.Exists("E:/HrTestData/" + participantID))
		{
			participantID += 1;
		}
		Directory.CreateDirectory ("E:/HrTestData/" + participantID);
		directoryPath = "E:/HrTestData/" + participantID + "/";
	}
	
	// Update is called once per frame
	void Update () {
		if (stream.GetConnection () == true) {
			time += Time.deltaTime; 
			// for every update heart rate and gsr is saved in an array and send through UDP to another application. 
			biometrics [0] = stream.GetHeartRate ();
			biometrics [1] = stream.GetAvgHeartRate ();
			biometrics [2] = stream.GetMaxHeartRate ();
			biometrics [3] = stream.GetGsr ();
			//udpSender.sendDoubleArr (biometrics); 

			// here we "record" all the biometrics and the triggers 
			// however, as a standard the trigger is 0 until something else is send. 
			timeList.Add (time); 
			hrArrList.Add (stream.GetHeartRate ());
			avgHrArrList.Add (stream.GetAvgHeartRate ());
			maxHrArrList.Add (stream.GetMaxHeartRate ());
			gsrArrList.Add (stream.GetGsr ());
			triggers.Add (0);

			//is this how a trigger can be placed and the two needed statements needed to only place one is created. 
			if (time > 2.0 && time < 2.1 && triggerEnabled) {
				SetTrigger (2);
				triggerEnabled = false;
			}
			if (time > 2.1 && time < 2.2) {
				triggerEnabled = true;
			}
			//baselineTime is up to change. for now it is 10, being 10 seconds. 
			// after that the baseline is found, which enables finding change score and percentage difference from the baseline. 
			//
			if (time > baselineTime && captureBaseline == true) {
				captureBaseline = false;
				hrBase = MeasureBaseLine (hrArrList);
				avghrBase = MeasureBaseLine (avgHrArrList);
				maxhrBase = MeasureBaseLine (maxHrArrList);
				gsrBase = MeasureBaseLine (gsrArrList);
				time = 0.0f;
				Debug.Log ("Baseline Found");
			}
			//as mentioned above when we got the baseline we are now capable of finding differnt important features.
			// it is always recomended to have a baseline corection in regards to analysis and online feedback systems. 
			// for sufisticated online analysis tools can be created 
			// this create the fundamentals for what you might want to do in the end. 
			if (hrBase != 0.0) {
				triggersWithBaseLine.Add (0);
				hrCSArrList.Add (LiveChangeScore (stream.GetHeartRate (), hrBase));
				avgHrCSArrList.Add (LiveChangeScore (stream.GetAvgHeartRate (), avghrBase));
				maxHrCSArrList.Add (LiveChangeScore (stream.GetMaxHeartRate (), maxhrBase));
				gsrCSArrList.Add (LiveChangeScore (stream.GetGsr (), gsrBase));
				timeCSList.Add (time); 

				hrPDArrList.Add (LivePercentageDiffFromBase (stream.GetHeartRate (), hrBase));
				avgHrPDArrList.Add (LivePercentageDiffFromBase (stream.GetAvgHeartRate (), avghrBase));
				maxHrPDArrList.Add (LivePercentageDiffFromBase (stream.GetMaxHeartRate (), maxhrBase));
				gsrPDArrList.Add (LivePercentageDiffFromBase (stream.GetGsr (), gsrBase));

				if (time > 3.0 && time < 3.1 && triggerEnabled) {
					SetTrigger (3);
					triggerEnabled = false;
				}
				if (time > 3.1 && time < 3.2) {
					triggerEnabled = true;
				}
				if (time > 4.0 && time < 4.1 && triggerEnabled) {
					SetTrigger (4);
					triggerEnabled = false;
				}
				if (time > 4.1 && time < 4.2) {
					triggerEnabled = true;
				}
				// if we have reach a certain tiume. this can be anything else 
				// create files with recordings you have just gathered. 
				if (time > 11 && printTxts == true) {
					printTxts = false; 
					string filenameRaw = "part" + participantID + "bioMeasure.txt"; 
					string filenameCS = "part" + participantID + "ChangeScore.txt"; 
					string filenamePD = "part" + participantID + "PercentageDifferenceFromBase.txt"; 
					CreateTxtContent (timeList, hrArrList, avgHrArrList, maxHrArrList, gsrArrList,triggers, filenameRaw);
					CreateTxtContent (timeCSList, hrCSArrList, avgHrCSArrList, maxHrCSArrList, gsrCSArrList,triggersWithBaseLine, filenameCS); 
					CreateTxtContent (timeCSList, hrPDArrList, avgHrPDArrList, maxHrPDArrList, gsrPDArrList,triggersWithBaseLine, filenamePD); 
					Debug.Log ("Text Files Printed"); 
				}

			}
		}
	}
	// functions to create content and create a txtx file with the created content currently locked to the biometrics 
	// you could create a new create content function for the mousemetrics, which are given to the system, but not used yet!
	private void CreateTxtContent(ArrayList time, ArrayList hr, ArrayList avghr, ArrayList maxhr, ArrayList gsr, ArrayList trig, string filename)
	{
		string labels = "Time,HeartRate,AverageHeartRate,MaximumHeartRate,GSR,Triggers\r\n";
		string info = labels; 
		for (int i = 0; i < hr.Count; i++) {
			info += (time[i]+","+hr[i]+","+avghr[i]+","+maxhr[i]+","+gsr[i]+","+trig[i]+"\r\n");
		}
		CreateTxtFile (info, filename);
	}
	private void CreateTxtFile(string txt, string fileName)
	{
		ASCIIEncoding asciiCoding = new ASCIIEncoding ();
		UnicodeEncoding uniencoding = new UnicodeEncoding();


		byte[] result = asciiCoding.GetBytes(txt);

		using (FileStream SourceStream = File.Open(directoryPath+fileName, FileMode.OpenOrCreate))
		{
			SourceStream.Seek(0, SeekOrigin.End);
			//SourceStream.WriteAsync(result, 0, result.Length);
			SourceStream.Write(result, 0, result.Length);
			SourceStream.Close();
		}
	}
	//this function will calculate the baseline from the inputed Array. 
	//this can be any length you want. 
	public double MeasureBaseLine(ArrayList bioMeasure)
	{
		double baseline = 0.0;
		double sum = 0.0;
		for (int i = 0; i < bioMeasure.Count; i++) {
			string biom = bioMeasure [i].ToString ();
			sum += double.Parse(biom);
		}
		baseline = sum / bioMeasure.Count; 
		return baseline;  
	}
	// this gives you the changeScore based on the psychophysiological measurement in media. This score is the cange from the baseline.
	public double LiveChangeScore(double bioMeasure, double baseline)
	{
		double changeScore = bioMeasure - baseline; 
		return changeScore; 
	}
	// this is the percentage difference from the baseline in case you want the percentage instead of an actual measure. 
	public double LivePercentageDiffFromBase(double bioMeasure, double baseline)
	{
		double percentageDiff = (bioMeasure - baseline)/(baseline/100); 
		return percentageDiff; 
	}
	// this will be the function you'll communicate with when placing triggers. 
	//this function both sends and records triggers. 
	public void SetTrigger(int triggerNo )
	{
		triggers.Add (triggerNo);
		//triggerSender.sendInt (triggerNo);
		if (hrBase != 0.0) {
			triggersWithBaseLine.Add (triggerNo); 
		}
	}
	// the three above functions are nice to use in realtime and will be of great help when working with feedback loops based on the biometrics.
}
