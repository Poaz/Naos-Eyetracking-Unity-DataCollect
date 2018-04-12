using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Net.Http;
using System.Threading;
using SimpleJSON;
using Eppy;


public class ReceiveLiveStream : MonoBehaviour
{
    
    public bool streaming;
    Thread sendThread;
    Thread receiveThread;
    IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.71.50"), 49152);
    private UdpClient client;
    //Keep-alive message content used to request live data and live video streams
    static string KA_DATA_MSG = "{\"type\": \"live.data.unicast\", \"key\": \"some_GUID\", \"op\": \"start\"}";
    public string base_url = "http://192.168.71.50"; 
    private byte[] bytes;
    
    public double gdlX,gdlY,gdlZ,gdrX,gdrY,gdrZ, velL, velR,  pdl, pdr, ts;

    //public List<double>  pdrBuff; 
    public List<Tuple<double, double>> pdlBuff, pdrBuff, gdlXBuff, gdlYBuff, gdlZBuff, gdrXBuff, gdrYBuff, gdrZBuff;
    
    public int project_id, participant_id;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        streaming = true;

        client = new UdpClient();
        client.Connect(ep);

        bytes = Encoding.ASCII.GetBytes(KA_DATA_MSG);
    }

    void Start()
    {
        sendThread = new Thread(new ThreadStart(SendKAMessage));
        sendThread.IsBackground = true;
        sendThread.Start();

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start(); 

       // pdlBuff = new List<Tuple<double, double>>();    
    }

    public void ReceiveData(){

        byte[] data = new byte[0]; 

        while(true){
            
            data = client.Receive(ref ep);
           
            string json = Encoding.ASCII.GetString(data);
            char [] removed = { '{', '}' , '"', ':'} ;
            json.TrimStart(removed);

            var _json = JSON.Parse(json);
            if(json != ""){
                 
               ts = double.Parse( json.Substring(6 ,10) );
                               
                if(json.Contains("gd")){

                    if(json.Contains("left")){
                        
                         string gdl = _json["gd"].ToString();
                         
                         gdl =  gdl.TrimStart('[');
                         gdl = gdl.TrimEnd(']');

                         string [] values = gdl.Split(',');

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
                         
                         //gdlX = double.Parse(values[0]);
                         //gdlY = double.Parse(values[1]);
                         //gdlZ = double.Parse(values[2]);

                    }else{

                        //right

                        string gdl = _json["gd"].ToString();
                         
                         gdl =  gdl.TrimStart('[');
                         gdl = gdl.TrimEnd(']');

                         string [] values = gdl.Split(',');

                          //x
                         if(gdrXBuff.Count < 10)
                        {
                            gdrXBuff.Add(Tuple.Create(ts,double.Parse(values[0])));
                                                                                  
                        }else
                        {
                            AddAvgVal(values[0],ts,gdrXBuff,gdrX);
                        }

                        //y
                         if(gdrYBuff.Count < 10)
                        {
                            gdrYBuff.Add(Tuple.Create(ts,double.Parse(values[1])));
                                                                                  
                        }else
                        {
                            AddAvgVal(values[1],ts,gdrYBuff,gdrY);
                        }

                        //z
                         if(gdrZBuff.Count < 10)
                        {
                            gdrZBuff.Add(Tuple.Create(ts,double.Parse(values[2])));
                                                                                  
                        }else
                        {
                            AddAvgVal(values[2],ts,gdrZBuff,gdrZ);
                        }
                         
                        // gdrX = double.Parse(values[0]);
                        // gdrY = double.Parse(values[1]);
                        // gdrZ = double.Parse(values[2]);

                    }

                } //gd found

                if(json.Contains("pd")){

                    if(json.Contains("left")){
                        
                        string gdl = _json["pd"].ToString();
                    
                        gdl =  gdl.TrimStart('[');
                        gdl = gdl.TrimEnd(']');
                       // pdl = double.Parse(gdl);

                        if(pdlBuff.Count < 10)
                        {
                            pdlBuff.Add(Tuple.Create(ts,double.Parse(gdl)));
                                                                                  
                        }else if(pdlBuff.Count == 10){
                             print("pdl");

                            pdlBuff.RemoveAt(0);
                            pdlBuff.Add(Tuple.Create(ts,double.Parse(gdl)));
                            // beregn average                                                      
                           
                           double avg = 0;
                           for (int i = 0; i < pdlBuff.Count-1; i++)
                           {
                             avg +=  (pdlBuff[i].Item2-pdlBuff[i+1].Item2)/(pdlBuff[i].Item1-pdlBuff[i+1].Item1);
                           }
                            // overfør average
                            pdl = avg/pdlBuff.Count;
                            

                        }
                      
                    }else{
                       //pdr
                        string gdl = _json["pd"].ToString();
                        
                        gdl =  gdl.TrimStart('[');
                        gdl = gdl.TrimEnd(']');
                        //pdr = double.Parse(gdl);

                        if(pdrBuff.Count < 10)
                        {
                            pdrBuff.Add(Tuple.Create(ts,double.Parse(gdl)));
                                                                                  
                        }else if(pdrBuff.Count == 10){
                             print("pdl");

                            pdrBuff.RemoveAt(0);
                            pdrBuff.Add(Tuple.Create(ts,double.Parse(gdl)));
                            // beregn average                                                      
                           
                           double avg = 0;
                           for (int i = 0; i < pdrBuff.Count-1; i++)
                           {
                             avg +=  (pdrBuff[i].Item2-pdrBuff[i+1].Item2)/(pdrBuff[i].Item1-pdrBuff[i+1].Item1);
                           }
                            // overfør average
                            pdr = avg/pdlBuff.Count;
                            
                        }

                    }

                }

            }
               
        }//while true

    }//Receive data

    private void AddAvgVal(string str, double ts,List<Tuple<double, double>> list, double result){
    
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

    public  void SendKAMessage(){

        while(streaming){

            client.Send(bytes, bytes.Length);
            //print("send message");
            Thread.Sleep(1000);
        }
        
    }

    public void SendRequest(string apiAction, string jsonData){

        //'Content-Type', 'application/json'

    }
    public void createProject()
    {
        //{"pr_info":{"name":"my new project","xid":"19"}}
        //SendRequest()
    
    }

    public void createParticipant()
    {
        //called by the setID

    }

    public void  createCalibration()
    {

/*
create_calibration(project_id, participant_id):
    data = {'ca_project': project_id, 'ca_type': 'default', 'ca_participant': participant_id}
    json_data = post_request('/api/calibrations', data)
    return json_data['ca_id']


 */
    }

    public void StartCalibration(){

        //POST /api/calibrations HTTP/1.1

        // post_request('/api/calibrations/' + calibration_id + '/start')

        

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
    
    void Update()
    {


        if(Input.GetKeyUp(KeyCode.Space)){

            client.Close();
            streaming = false;
            
        }
    }

     void OnApplicationQuit()
    {
        client.Close();
    }


}
