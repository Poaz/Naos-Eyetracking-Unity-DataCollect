using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;


public class ReceiveLiveStream : MonoBehaviour
{

    string GLASSES_IP = "192.168.71.50";
    int PORT = 49152;

    public bool streaming;
    Thread sendThread;
    Thread receiveThread;
    IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.71.50"), 49152);


     private UdpClient client;

    //Keep-alive message content used to request live data and live video streams
    static string KA_DATA_MSG = "{\"type\": \"live.data.unicast\", \"key\": \"some_GUID\", \"op\": \"start\"}";
     byte[] bytes;

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
            if(json != "")
                print(json);


        }

    }

    public  void SendKAMessage(){

        while(streaming){

            client.Send(bytes, bytes.Length);
            print("send message");
            Thread.Sleep(1000);
        }
        
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
