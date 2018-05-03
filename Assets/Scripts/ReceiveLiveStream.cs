using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using RestSharp;
using System.Threading;
using SimpleJSON;
using Tuple = Eppy.Tuple;
using UnityEngine.UI;



public class ReceiveLiveStream : MonoBehaviour
{
    public string test = "";
    public bool streaming, CalibrationSuccesful = false;
    Thread dataThread, videoThread;
    Thread receiveThread, receiveVideoThread;
    IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.71.50"), 49152);
    private UdpClient client, client2;

    public bool running = true;
    //Keep-alive message content used to request live data and live video streams
    static string KA_DATA_MSG = "{\"type\": \"live.data.unicast\", \"key\": \"some_GUID\", \"op\": \"start\"}";
    static string KA_VIDEO_MSG = "{\"type\": \"live.video.unicast\", \"key\": \"some_other_GUID\", \"op\": \"start\"}";
    public string base_url = "http://192.168.71.50"; 
    private byte[] bytes1, bytes2;
    
    public double gdlX,gdlY,gdlZ,gdrX,gdrY,gdrZ, velL, velR,  pdl, pdr, ts;

    //public List<double>  pdrBuff; 
    private List<Eppy.Tuple<double, double>> pdlBuff, pdrBuff, gdlXBuff, gdlYBuff, gdlZBuff, gdrXBuff, gdrYBuff, gdrZBuff;

    public String project_id, participant_id, ca_id, rec_id, status_data;

    private UIHandler _uiHandler;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        streaming = true;

        client = new UdpClient();
        client.Connect(ep);

        client2 = new UdpClient();
        client2.Connect(ep);

        bytes1 = Encoding.ASCII.GetBytes(KA_DATA_MSG);
        bytes2 = Encoding.ASCII.GetBytes(KA_VIDEO_MSG);

    }

    void Start()
    {
        project_id = PlayerPrefs.GetString("pr_id");
        _uiHandler = GameObject.FindGameObjectWithTag("UI").GetComponent<UIHandler>();
      
        print("creating threads");
        //live data thread
        dataThread = new Thread(new ThreadStart(SendKAMessage));
        dataThread.IsBackground = true;
        dataThread.Start();

        //videoThread = new Thread(new ThreadStart(SendKAMessage2));
        //videoThread.IsBackground = true;
        //videoThread.Start();


        //receiveThread = new Thread(new ThreadStart(ReceiveData));
        //receiveThread.IsBackground = true;
        //receiveThread.Start();

        //receiveVideoThread = new Thread(new ThreadStart(ReceiveVideo));
        //receiveVideoThread.IsBackground = true;
        //receiveVideoThread.Start();

        print("threads created");
      
       InitBuffers();

       StartCoroutine(CallReceiveData());
    }

    public void InitBuffers()
    {
        pdlBuff  = new  List<Eppy.Tuple<double, double>>();
        pdrBuff  = new  List<Eppy.Tuple<double, double>>();
        gdlXBuff = new  List<Eppy.Tuple<double, double>>();
        gdlYBuff = new  List<Eppy.Tuple<double, double>>();
        gdlZBuff = new  List<Eppy.Tuple<double, double>>();
        gdrXBuff = new  List<Eppy.Tuple<double, double>>();
        gdrYBuff = new  List<Eppy.Tuple<double, double>>();
        gdrZBuff = new List<Eppy.Tuple<double, double>> ();
    }

    public void ReceiveVideo()
    {
        byte[] data = new byte[0];
        
        while (running)
        {
            data = client2.Receive(ref ep);
        }
    }

    public IEnumerator CallReceiveData()
    {
        while (running)
        {
            ReceiveData();
            yield return new WaitForEndOfFrame();
        }
    }

    public void ReceiveData(){

        byte[] data = new byte[0];

       // while (running)
       // {
            
            data = client.Receive(ref ep);
            
            string json = Encoding.ASCII.GetString(data);
           // print("json preTrim: " + json.ToString());
            char [] removed = { '{', '}' , '"', ':'} ;
            //var _json = JSON.Parse(json);
        
            if(json != ""){
                 
               ts = double.Parse( json.Substring(6 ,10) );
                               
                if(json.Contains("gd")){

                    if(json.Contains("left")){

                        string gdl = json.Substring(json.IndexOf('[') + 1, 19);
                        string[] values = gdl.Split(',');

                        gdlX = double.Parse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                        gdlY = double.Parse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                        gdlZ = double.Parse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                    /*

                         //x
                         if(gdlXBuff.Count < 10)
                        {
                            gdlXBuff.Add(Tuple.Create(ts,double.Parse(values[0])));
                                                                                  
                        }else
                        {
                            AddAvgVal(values[0],ts,gdlXBuff,gdlX);
                        }

                        //y
                         if(gdlYBuff.Count < 10)
                        {
                            gdlYBuff.Add(Tuple.Create(ts,double.Parse(values[1])));
                                                                                  
                        }else
                        {
                            AddAvgVal(values[1],ts,gdlYBuff,gdlY);
                        }

                        //z
                         if(gdlZBuff.Count < 10)
                        {
                            gdlZBuff.Add(Tuple.Create(ts,double.Parse(values[2])));
                                                                                  
                        }else
                        {
                            AddAvgVal(values[2],ts,gdlZBuff,gdlZ);
                        }
                         */

                }
                else
                {

                    //right
                    string gdl = json.Substring(json.IndexOf('[') + 1, 19);
                    string[] values = gdl.Split(',');

                    gdrX = double.Parse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                    gdrY = double.Parse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    gdrZ = double.Parse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture);

                        /*
                    //x
                    if (gdrXBuff.Count < 10)
                        {
                            gdrXBuff.Add(Tuple.Create(ts,double.Parse(values[0])));
                            print("gdrX: " + double.Parse(values[0]));

                    }
                    else
                        {
                            AddAvgVal(values[0],ts,gdrXBuff,gdrX);
                            print("gdrx: " + double.Parse(values[0]));
                    }

                        //y
                         if(gdrYBuff.Count < 10)
                        {
                            gdrYBuff.Add(Tuple.Create(ts,double.Parse(values[1])));
                            print("gdry: " + double.Parse(values[1]));

                    }
                    else
                        {
                            AddAvgVal(values[1],ts,gdrYBuff,gdrY);
                            print("gdry: " + double.Parse(values[1]));
                    }

                        //z
                         if(gdrZBuff.Count < 10)
                        {
                            gdrZBuff.Add(Tuple.Create(ts, double.Parse(values[2])));
                        print("gdrZ: " + double.Parse(values[2]));

                    }
                    else
                        {
                            AddAvgVal(values[2],ts,gdrZBuff,gdrZ);
                            print("gdrz :" + gdrZ.ToString());
                        }
                        */ //buff left
                         
                    }

                } //gd found

                if(json.Contains("pd")){

                   
                    string pdString = json.Substring(json.IndexOf('p') + 4, 4);
                    print("pd:" +pdString);

                    if (json.Contains("left"))
                    {
                    /*
                    if (pdlBuff.Count < 10)
                     {
                         pdlBuff.Add(Tuple.Create(ts, double.Parse(pdString, NumberStyles.Float, CultureInfo.InvariantCulture)));
                     }
                     else if (pdlBuff.Count == 10)
                     {
                         pdlBuff.RemoveAt(0);
                         pdlBuff.Add(Tuple.Create(ts, double.Parse(pdString, NumberStyles.Float, CultureInfo.InvariantCulture)));

                         // beregn average                                                      
                         double avg = 0;
                         for (int i = 0; i < pdlBuff.Count - 1; i++)
                         {
                             avg += (pdlBuff[i].Item2 - pdlBuff[i + 1].Item2) / (pdlBuff[i].Item1 - pdlBuff[i + 1].Item1);
                         }
                         // overfør average
                         pdl = avg / pdlBuff.Count;
                     }
                     */
                    pdl = double.Parse(pdString, NumberStyles.Float, CultureInfo.InvariantCulture);
                }
                    else
                    {
                        pdr = double.Parse(pdString, NumberStyles.Float, CultureInfo.InvariantCulture);
                    //pdr
                    /*
                    if(pdrBuff.Count < 10)
                    {
                        pdrBuff.Add(Tuple.Create(ts,double.Parse(pdString, NumberStyles.Float, CultureInfo.InvariantCulture)));

                    }else if(pdrBuff.Count == 10){
                        pdrBuff.RemoveAt(0);
                        pdrBuff.Add(Tuple.Create(ts,double.Parse(pdString, NumberStyles.Float, CultureInfo.InvariantCulture)));
                        // beregn average 
                       double avg = 0;
                       for (int i = 0; i < pdrBuff.Count-1; i++)
                       {
                         avg +=  (pdrBuff[i].Item2-pdrBuff[i+1].Item2)/(pdrBuff[i].Item1-pdrBuff[i+1].Item1);
                       }
                        // overfør average
                        pdr = avg/pdlBuff.Count;

                    }
                    */
                    }
                } //pd

                if (json.Contains("gp"))
                {

                }
            }
    }//Receive data



    private void AddAvgVal(string str, double ts,List<Eppy.Tuple<double, double>> list, double result){
    
        list.RemoveAt(0);
        list.Add(Tuple.Create(ts,double.Parse(str)));
        // beregn average                                                      
       
       double avg = 0;
       for (int i = 0; i < list.Count-1; i++)
       {
         avg +=  (list[i].Item2-list[i+1].Item2)/(list[i].Item1-list[i+1].Item1);
       }
        // overfør average
        result = avg/list.Count;
        
    
    }

    public void SendKAMessage()
    {

        while (streaming)
        {

            client.Send(bytes1, bytes1.Length);
            //print("send message");
            Thread.Sleep(1);
        }
    }
    public void SendKAMessage2()
    {

        while (streaming)
        {

            client2.Send(bytes2, bytes2.Length);
            //print("send message");
            Thread.Sleep(1);
        }
    }

    public IEnumerator WaitForStatus(string apiAction, string key, string callType)
    {
        var reqClient = new RestClient();
        reqClient.BaseUrl = base_url + apiAction;
        print("status request: " + reqClient.BaseUrl);
        bool running = true;
        

        while (running)
        {
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");

            var response = reqClient.Execute(request);
            yield return new WaitForSeconds(1);
            var content = JSON.Parse(response.Content); 

            print(content[key]);
           
            if (callType == "calibration")
            {
                if (content[key] == "failed" || content[key] == "calibrated")
                {
                    running = false;
                    status_data = content[key];
                    print(status_data == "failed" ? "calibration failed: " +content.ToString() : "calibration succesful");
                    CalibrationSuccesful = status_data != "failed";
                    
                }
            }

            if (callType == "recording")
            {
                print("testing recording key");
                if (content[key] == "failed" || content[key] == "done")
                {
                    running = false;
                    status_data = content[key];

                    print(status_data == "failed" ? "recording failed" : "recording succesful");
                }
            }
        }
    }
    /*
    public string SendRequest0(string apiAction)
    {
        var reqClient = new RestClient(base_url);
        var request = new RestRequest(apiAction, Method.POST);

        request.AddHeader("Content-Type", "application/json");
        request.RequestFormat = DataFormat.Json;
        request.AddBody(new { });

        var response = reqClient.Execute(request).Content;
        return response;
    }
    */
    public string SendRequest(string apiAction)
    {
        var reqClient = new RestClient(base_url);
        var request = new RestRequest(apiAction,Method.POST);
        request.AddHeader("Content-Type", "application/json");
        request.RequestFormat = DataFormat.Json;
        request.AddBody(new { });

        var response = reqClient.Execute(request).Content;
        return response;
        //json data
    }
    //create participant
    public string SendRequest2(string apiAction)
    {
        var reqClient = new RestClient();
        reqClient.BaseUrl = base_url + apiAction;

        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");

        request.RequestFormat = DataFormat.Json;
        request.AddBody(new { pa_project = project_id, pa_info = new {name = _uiHandler.ID} });

        var response = reqClient.Execute(request);
        var content = response.Content;

        return content;
    }

    public string SendRequest3(string apiAction)
    {
        var reqClient = new RestClient();
        reqClient.BaseUrl = base_url + apiAction;

        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");

        request.RequestFormat = DataFormat.Json;
        request.AddBody(new { ca_project = project_id, ca_type = "default", ca_participant = participant_id });

        var response = reqClient.Execute(request);
        var content = response.Content;
        return content;
    }

    public string SendRequest_3(string apiAction)
    {
        var reqClient = new RestClient(base_url);
        //reqClient.BaseUrl = base_url + apiAction;

        var request = new RestRequest(apiAction,Method.POST);
        request.AddHeader("Content-Type", "application/json");

        request.RequestFormat = DataFormat.Json;
        request.AddBody(new { ca_project = project_id, ca_type = "default", ca_participant = participant_id });

        var response = reqClient.Execute(request);
        var content = response.Content;
        return content;
    }

    public string SendRequest4(string apiAction)
    {
        var reqClient = new RestClient();
        reqClient.BaseUrl = base_url + apiAction;

        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");

        request.RequestFormat = DataFormat.Json;
        request.AddBody(new { rec_participant = participant_id });

        var response = reqClient.Execute(request);
        var content = response.Content;
        return content;
    }


    public void CreateProject()
    {
        print("current project id:" + project_id );
        if (project_id == "")
        {
        var json_string = JSON.Parse(SendRequest("api/projects"));
            project_id = json_string["pr_id"];
            PlayerPrefs.SetString("pr_id", project_id);
        }
        else
        {
            print("current project id:" + project_id );
        }
    }
    //argument is project id
    public void CreateParticipant(string pr_id)
    {
        JSONObject data = new JSONObject();
        data.Add("pr_id",project_id);
        var json_string = JSON.Parse(SendRequest2("/api/participants"));
        participant_id = json_string["pa_id"];  
    }
  

    public void  CreateCalibration()
    {
        var json_string = JSON.Parse(SendRequest_3("api/calibrations"));
        ca_id = json_string["ca_id"];
    }

    public void StartCalibration()
    {
        SendRequest("api/calibrations/" + ca_id + "/start");
        print("started calibration");
    }

    public void CreateRecording()
    {
        var json_string = JSON.Parse(SendRequest4("/api/recordings/"));
        rec_id = json_string["rec_id"];
    }

    public void StartRecording()
    {
        print("startRecord command: " + "/api/recordings/" + rec_id + "/start");
        SendRequest("api/recordings/" + rec_id + "/start");
    }

    public void StopRecording()
    {
        print("stopRecord command: " + "/api/recordings/" + rec_id + "/stop");
        SendRequest("api/recordings/" + rec_id + "/stop");
        print("recording ended");
    }

    public void PauseRecording()
    {
        print("stopRecord command: " + "/api/recordings/" + rec_id + "/stop");
        SendRequest("api/recordings/" + rec_id + "/pause");
        print("recording paused");
    }

    public double GetGDLX()
	{
		return gdlX;
	}
    public double GetGDLY()
	{
		return gdlY;
	}
    public double GetGDLZ()
	{
		return gdlZ;
	}

     public double GetGDRX()
	{
		return gdrX;
	}
    public double GetGDRY()
	{
		return gdrY;
	}
    public double GetGDRZ()
	{
		return gdrZ;
	}

    public double GetPDL()
    {
        return pdl;
    }

    public double GetPDR()
    {
        return pdr;
    }

    public void CalibrationTest()
    {
        print("start calibration");
        CreateCalibration();
        StartCalibration();
        StartCoroutine(WaitForStatus("/api/calibrations/" + ca_id + "/status", "ca_state", "calibration"));
    }

    public void PrepForTest()
    {
        CreateProject();
        CreateParticipant(project_id);
        print("Project: " + project_id + " Participant: " + participant_id);
    }
    
    void Update()
    {


        if(Input.GetKeyUp(KeyCode.Space)){

            client.Close();
            streaming = false;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            print("A");
            CreateProject();
            CreateParticipant(project_id);
            CreateCalibration();

            print("Project: " + project_id + " Participant: " + participant_id + " Calibration: " + ca_id);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            StartCalibration();
            StartCoroutine(WaitForStatus("/api/calibrations/" + ca_id + "/status", "ca_state", "calibration"));
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            CreateRecording();
            print("recording created");
            StartRecording();
            print("recording started");
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            StopRecording();
            print("Recording stopped");
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            PauseRecording();
            print("Recording paused");
        }


        if (Input.GetKeyUp(KeyCode.B))
        {
            print("checking status");
            TestStatus();
        }
    }

    public IEnumerator TestRecording()
    {
        StartRecording();
        yield return new WaitForSeconds(10);
        StopRecording();    
    }

    public void TestStatus()
    {
        StartCoroutine(WaitForStatus("/api/recordings/" + rec_id + "/status", "rec_state", "recording"));
       
    }

     void OnApplicationQuit()
    {
        client.Close();
    }


}
