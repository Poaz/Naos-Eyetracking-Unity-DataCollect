using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using SimpleJSON;

public class ReceiveLiveStream : MonoBehaviour
{
    
    public bool streaming;
    Thread sendThread;
    Thread receiveThread;
    IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.71.50"), 49152);
    private UdpClient client;
    //Keep-alive message content used to request live data and live video streams
    static string KA_DATA_MSG = "{\"type\": \"live.data.unicast\", \"key\": \"some_GUID\", \"op\": \"start\"}";
    private byte[] bytes;
    public string[] gazeData;
    public double gazePoint,gdlX,gdlY,gdlZ,gdrX,gdrY,gdrZ, pdl, pdr;

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

                
                
                if(json.Contains("gd")){

                    if(json.Contains("left")){
                        
                         string gdl = _json["gd"].ToString();
                         
                         gdl =  gdl.TrimStart('[');
                         gdl = gdl.TrimEnd(']');

                         string [] values = gdl.Split(',');
                         
                         gdlX = double.Parse(values[0]);
                        
                         gdlY = double.Parse(values[1]);
                        
                         gdlZ = double.Parse(values[2]);

                    }else{

                        string gdl = _json["gd"].ToString();
                         
                         gdl =  gdl.TrimStart('[');
                         gdl = gdl.TrimEnd(']');

                         string [] values = gdl.Split(',');
                         
                         gdrX = double.Parse(values[0]);
                        
                         gdrY = double.Parse(values[1]);
                        
                         gdrZ = double.Parse(values[2]);

                    }

                } //gd found

                if(json.Contains("pd")){

                    if(json.Contains("left")){
                        
                        string gdl = _json["pd"].ToString();

                        print("left " + gdl);
                        gdl =  gdl.TrimStart('[');
                        gdl = gdl.TrimEnd(']');
                        pdl = double.Parse(gdl);

                        

                    }else{

                        

                        string gdl = _json["pd"].ToString();
                        print("right "+gdl);
                        gdl =  gdl.TrimStart('[');
                        gdl = gdl.TrimEnd(']');
                        pdr = double.Parse(gdl);

                    }

                }

            }
               
        }//while true

    }

    public  void SendKAMessage(){

        while(streaming){

            client.Send(bytes, bytes.Length);
            //print("send message");
            Thread.Sleep(1000);



        }
        
    }
    public void createProject()
    {

    
    }

    public void createParticipant()
    {
        //called by the setID
    }

    public void  createCalibration()
    {

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
