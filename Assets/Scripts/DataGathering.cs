//MAIN SKELETON OF THIS SCRIPT IS CREATED BY MIONIX DEVELOPMENT STAFF JAKOB
// reference props to the Mionics team. 
using UnityEngine;
using System.Collections;
using WebSocketSharp;
using UnityEngine.UI;
using System;

public class DataGathering : MonoBehaviour {
    private bool connected = false;
    WebSocket ws;
	private bool streamEnabled = false; 

	//mionix data variables
	private double heartRate;
	private double heartRateAvg; 
	private double heartRateMax;
	private double gsr;

	private double totTime;
	private double totDist;
	private double totScroll; 
	private double totClicks;

	private double stTime;
	private double stDist;
	private double stScroll; 
	private double stClick;

	private double mMoveSpeed;
	private double mAvgSpeed;
	private double mMaxSpeed;

	private double clickRate;
	private double clickRateAvg;
	private double clickRateMAx; 

	private double scrollRate;
	private double scrollRateAvg; 
	private double scrollRateMax;

	//in case there is an error with the connection with the mionix mouse server this message is called
    private void Ws_OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log("Error");
    }
	//When the communication to the mionix mouse server is closed this message is called. 
    private void Ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("Close");
    }
	//when the program receives a message from the mouse server we enter this function.
	//from here we can do what we want with the data. The data is a .json 
	//but C# can interpret it as a long string.
	//the mouse sends two .json objects first one with the mousemetrics 
	//being clicks, scrolls and movements 
	//then the mouse sends a .json object with the Biometrics 
	// heartrate and GSR. 
    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
		//for every received .json object it is send to a string and chopped into the exact values corresponding 
		//to the keyword that has been asked for fx if it contains heartRateAvg.
        string data = e.Data.ToString();

		if (data.Contains ("heartRate")) 
		{ 
			string str;
			str = data.Remove(0,data.IndexOf("heartRateAvg") -7);
			str = str.Remove(str.IndexOf("heartRateAvg")-2,str.Length+2-str.IndexOf("heartRateAvg"));
			heartRate = Convert.ToDouble (str);

		}
		if (data.Contains ("heartRateAvg")) 
		{
			string str;
			str = data.Remove (0, data.IndexOf ("heartRateAvg")+14);
			str = str.Remove (str.IndexOf ("heartRateMax") - 2, str.Length +2  - str.IndexOf ("heartRateMax"));
			heartRateAvg = Convert.ToDouble (str);  
		}
		if (data.Contains ("heartRateMax")) 
		{
			string str; 
			str = data.Remove (0, data.IndexOf ("heartRateMax") + 14);
			str = str.Remove (str.IndexOf ("gsr") - 2, str.Length +2  - str.IndexOf ("gsr"));
			heartRateMax = Convert.ToDouble(str); 
		}

        if (data.Contains("gsr"))
		{
			string str;
            str = data.Remove(0, data.IndexOf("gsr") + 5);
            str = str.Replace("}", "");
			gsr = Convert.ToDouble(str);

        }
		if (data.Contains ("totalTime")) 
		{
			string str; 
			str = data.Remove (0, data.IndexOf ("totalTime") + 11);
			str = str.Remove (str.IndexOf ("totalDistance") - 2, str.Length +2  - str.IndexOf ("totalDistance"));
			totTime = Convert.ToDouble (str);
		}
		if (data.Contains ("totalDistance")) 
		{
			string str;
			str = data.Remove (0, data.IndexOf ("totalDistance") + 16);
			str = str.Remove (str.IndexOf ("totalScrolls") - 2,str.Length +2  - str.IndexOf ("totalScrolls"));
			totDist = Convert.ToDouble (str);
		}
		if (data.Contains ("totalScrolls")) 
		{
			string str;
			str = data.Remove (0, data.IndexOf ("totalScrolls") + 14);
			str = str.Remove (str.IndexOf ("totalClicks") - 2,str.Length +2  - str.IndexOf ("totalClicks"));
			totScroll = Convert.ToDouble (str);
		}
		if (data.Contains ("totalClicks")) 
		{
			string str;
			str = data.Remove (0, data.IndexOf ("totalClicks") + 13);
			str = str.Remove (str.IndexOf ("streakTime") - 2,str.Length +2  - str.IndexOf ("streakTime"));
			totClicks = Convert.ToDouble (str);
		}
		if(data.Contains ("streakTime"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("streakTime") + 12);
			str = str.Remove (str.IndexOf ("streakDistance") - 2,str.Length +2  - str.IndexOf ("streakDistance"));
			stTime = Convert.ToDouble (str);
		}
		if(data.Contains ("streakDistance"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("streakDistance") + 16);
			str = str.Remove (str.IndexOf ("streakScrolls") - 2,str.Length +2  - str.IndexOf ("streakScrolls"));
			stDist = Convert.ToDouble (str);
		}
		if (data.Contains ("streakScrolls")) 
		{
			string str;
			str = data.Remove (0, data.IndexOf ("streakScrolls") + 15);
			str = str.Remove (str.IndexOf ("streakClicks") - 2,str.Length +2  - str.IndexOf ("streakClicks"));
			stScroll = Convert.ToDouble (str);
		}
		if (data.Contains ("streakClicks")) 
		{
			string str;
			str = data.Remove (0, data.IndexOf ("streakClicks") + 14);
			str = str.Remove (str.IndexOf ("speed") - 2,str.Length +2  - str.IndexOf ("speed"));
			stClick = Convert.ToDouble (str);
		}
		if (data.Contains ("speed")) 
		{
			string str;
			str = data.Remove (0, data.IndexOf ("speed") + 7);
			str = str.Remove (str.IndexOf ("speedAvg") - 2,str.Length +2  - str.IndexOf ("speedAvg"));
			mMoveSpeed = Convert.ToDouble (str);
		}
		if(data.Contains("speedAvg"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("speedAvg") + 10);
			str = str.Remove (str.IndexOf ("speedMax") - 2,str.Length +2  - str.IndexOf ("speedMax"));
			mAvgSpeed = Convert.ToDouble (str);
		}
		if(data.Contains("speedMax"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("speedMax") + 10);
			str = str.Remove (str.IndexOf ("clickRate") - 2,str.Length +2  - str.IndexOf ("clickRate"));
			mMaxSpeed = Convert.ToDouble (str);
		}
		if(data.Contains("clickRate"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("clickRate") + 11);
			str = str.Remove (str.IndexOf ("clickRateAvg") - 2,str.Length +2  - str.IndexOf ("clickRateAvg"));
			clickRate = Convert.ToDouble (str);
		}
		if(data.Contains("clickRateAvg"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("clickRateAvg") + 14);
			str = str.Remove (str.IndexOf ("clickRateMax") - 2,str.Length +2  - str.IndexOf ("clickRateMax"));
			clickRateAvg = Convert.ToDouble (str);

		}
		if(data.Contains("clickRateMax"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("clickRateMax") + 14);
			str = str.Remove (str.IndexOf ("scrollRate") - 2,str.Length +2  - str.IndexOf ("scrollRate"));
			clickRateMAx = Convert.ToDouble (str);

		}
		if(data.Contains("scrollRate"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("scrollRate") + 12);
			str = str.Remove (str.IndexOf ("scrollRateAvg") - 2,str.Length +2  - str.IndexOf ("scrollRateAvg"));
			scrollRate = Convert.ToDouble (str);

		}
		if(data.Contains("scrollRateAvg"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("scrollRateAvg") + 15);
			str = str.Remove (str.IndexOf ("scrollRateMax") - 2,str.Length +2  - str.IndexOf ("scrollRateMax"));
			scrollRateAvg = Convert.ToDouble (str);

		}
		if(data.Contains("scrollRateMax"))
		{
			string str;
			str = data.Remove (0, data.IndexOf ("scrollRateMax") + 15);
			str = str.Replace ("}","");
			scrollRateMax = Convert.ToDouble (str);

		}
    }
	//in case we have opened the communication to the mouse server this funciton is called. 
    private void Ws_OnOpen(object sender, System.EventArgs e)
    {
       
        Debug.Log("Open");
        ws.SendAsync("echo", OnSendComplete);
    }
	// we have successfully established connection 
    private void OnSendComplete(bool success)
    {
		streamEnabled = true; 
        Debug.Log("message Sent:" + success);
    }
    // Use this for initialization
    void Start () {
        
        Debug.Log("Start");

        if (!connected)
        {
            ws = new WebSocket("ws://localhost:7681/","mionix-beta");
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.OnClose += Ws_OnClose;
            ws.OnError += Ws_OnError;
            ws.ConnectAsync();
        }
    }

    void OnApplicationQuit()
    {
        ws.Close();
    }

	// all these get functions will be your wawy of get access to all the data from the mouse.  
	// all of them are doubles. 
	public double GetHeartRate()
	{
		return heartRate;
	}
	public double GetAvgHeartRate()
	{
		return heartRateAvg;
	}
	public double GetMaxHeartRate()
	{
		return heartRateMax;
	}
	public double GetGsr()
	{
		return gsr;
	}
	public double GetTotalTime()
	{
		return totTime;
	}
	public double GetTotalDistance()
	{
		return totDist;
	}
	public double GetTotalScroll()
	{
	 	return totScroll; 
	}
	public double GetTotalClicks()
	{
		return totClicks;
	}
	public double GetStreakTime()
	{
		return stTime;
	}
	public double GetStreakDistance()
	{
		return stDist;
	}
	public double GetStreakScroll()
	{
		return stScroll; 
	}
	public double GetStreakClicks()
	{
		return stClick;
	}
	public double GetMouseMoveSpeed()
	{
		return  mMoveSpeed;
	}
	public double GetMouseAvgSpeed()
	{
		return mAvgSpeed;
	}
	public double GetMouseMaxSpeed()
	{
		return mMaxSpeed;
	}
	public double GetClickRate()
	{
		return clickRate;
	}
	public double GetClickRateAvg()
	{
		return clickRateAvg;
	}
	public double GetClickRateMax()
	{
		return clickRateMAx; 
	}
	public double GetScrollRate()
	{
		return scrollRate;
	}
	public double GetScrollRateAvg()
	{
		return scrollRateAvg; 
	}
	public double GetScrollRateMax()
	{
		return scrollRateMax;
	}

	public bool GetConnection()
	{
		return streamEnabled;
	} 
}
