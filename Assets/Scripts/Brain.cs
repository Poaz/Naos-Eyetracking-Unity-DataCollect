using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public string pythonLocation, pythonScript;
    public DataGathering naos;
    public ReceiveLiveStream eyeData;
    private float[] testTensor;

    public void Start()
    {
        testTensor = new float[]
        {
            140.285470581055f, 41.11237678527829f, 5.836468505859f, 0.0f,
            0.0f, 0.0f, 2.463f, 0.0f, 64.28699999999999f, 0.7287418126212261f
        };

        naos = GetComponent<DataGathering>();
        eyeData = GetComponent<ReceiveLiveStream>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UnityEngine.Debug.Log(Predict());
        }
    }

    float[] GetTensor()
    {
        float[] tmpTensor = new[]
        {
            (float) eyeData.GetGDLX(), (float) eyeData.GetGDLY(), (float) eyeData.GetGDLZ(),
            (float) eyeData.GetGDRX(), (float) eyeData.GetGDRY(), (float) eyeData.GetGDRZ(),
            (float) eyeData.GetPDL(), (float) eyeData.GetPDR(), (float) naos.GetHeartRate(),
            (float) naos.GetGsr()
        };
        return tmpTensor;
    }

    public int Predict()
    {
        // full path of python interpreter
        pythonLocation = @"C:\Users\Dines\AppData\Local\Programs\Python\Python36\python.exe";

        // python app to call
        pythonScript = @"D:\Projects\NaosQGMouse-DataCollecting\Assets\Scripts\sum.py";

        //Create new process with python script
        ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(pythonLocation);

        //Making sure we can read the output
        myProcessStartInfo.UseShellExecute = false;
        myProcessStartInfo.RedirectStandardOutput = true;

        //Get new Tensor
        //var Tensor = GetTensor();
        var tensor = testTensor;

        //Startup the Python script, calling it with reference to self + all arguments for prediction.
        myProcessStartInfo.Arguments = pythonScript + " " + tensor[0] + " " + tensor[1] + " " + tensor[2] + " " +
                                       tensor[3]
                                       + " " + tensor[4] + " " + tensor[5] + " " + tensor[6] + " " + tensor[7] + " " +
                                       tensor[8]
                                       + " " + tensor[9];

        //Assign the arguments to the process.
        Process myProcess = new Process();
        myProcess.StartInfo = myProcessStartInfo;

        UnityEngine.Debug.Log("Calling Python script with arguments:" + " " + tensor[0] + " " + tensor[1] + " " +
                              tensor[2] + " " + tensor[3]
                              + " " + tensor[4] + " " + tensor[5] + " " + tensor[6] + " " + tensor[7] + " " + tensor[8]
                              + " " + tensor[9]);

        //Start the process
        myProcess.Start();

        //Read the output from the python script.
        // in order to avoid deadlock we will read output first and then wait for process terminate:
        StreamReader myStreamReader = myProcess.StandardOutput;
        string myString = myStreamReader.ReadLine();

        //string myString = myStreamReader.ReadToEnd();

        //Wait for the exit signal, then close.
        myProcess.WaitForExit();
        myProcess.Close();
        return Convert.ToInt32(myString);
    }
}