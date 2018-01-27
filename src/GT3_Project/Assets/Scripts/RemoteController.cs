using System;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Extensions.WebServices;

public class RemoteController : MonoBehaviour
{
	public TextAsset  htmlTemplate;
	public CarManager carManager;

	public static RemoteController instance;

	[HideInInspector]
	public float steerH = 0.0f;
	[HideInInspector]
	public float steerV = 0.0f;
	[HideInInspector]
	public bool updateStatus;
	[HideInInspector]
	public string statusData;
	[HideInInspector]
	public bool controllerConnected;
	[HideInInspector]
	public bool updateOrientation;
	[HideInInspector]
	public string orientationData;
	[HideInInspector]
	public bool updateVibration;
	[HideInInspector]
	public string vibrationData;
	[HideInInspector]
	public string[] httpUrls = {
		"http://127.0.0.1:8080/",
		"http://localhost:8080/",
		""
	};

	private HttpServer httpService;
	private WSServer wsService;
	private int wsPort = 1337;

	private void Update()
	{
		if (updateStatus)
		{
			if (statusData == "connection open")
				carManager.ControllerConnected = controllerConnected;
			else if (statusData == "connection closed")
			{
				carManager.ControllerConnected = controllerConnected;
				steerH = 0.0f;
				steerV = 0.0f;
			}

			statusData = "";
			updateStatus = false;
		}
		if (updateOrientation && orientationData != "")
		{
			UpdateOrientationData();
			updateOrientation = false;
		}
	}

	private void Start()
	{
		instance = this;
		SetInternalIP();
		InitWebServices();
	}

	private void OnApplicationQuit()
	{
		Stop();
	}

	public void Stop()
	{
		httpService.Stop();
		wsService.Stop();
	}

	private string SetExternalIP()
	{
		string externalIP = "";
		externalIP = new WebClient().DownloadString("http://checkip.dyndns.org/");
		externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
		httpUrls[2] = "http://" + externalIP + ":8080/";

		return externalIP;
	}

	private string SetInternalIP()
	{
		string strHostName = "";
		string internalIP = "";
		strHostName = Dns.GetHostName();

		IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
		IPAddress[] addr = ipEntry.AddressList;

		for (int i = 0; i <= addr.Length - 1; i++)
		{
			if (addr[i].AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
				internalIP = addr[i].ToString();
		}

		httpUrls[2] = "http://" + internalIP + ":8080/";

		return internalIP;
	}


    private void InitWebServices()
	{
		string htmlDoc = htmlTemplate.ToString().Replace("{", "{{").Replace("}", "}}").Replace("{{0}}", "{0}").Replace("{{1}}", "{1}");

		string htmlResponse = string.Format(
			htmlDoc,
			Regex.Match(httpUrls[2], "(http|https)(:\\/\\/)(.+)(:.\\d+)").Groups[3].Value,
			wsPort
		);

		httpService = new HttpServer(htmlResponse, httpUrls);
		wsService   = new WSServer(wsPort);
        httpService.Run();
        wsService.Run();
    }
		
	private void UpdateOrientationData()
	{
		string regexPattern = "^rot\\((-?\\d+\\.\\d+);(-?\\d+\\.\\d+);(-?\\d+\\.\\d+)";

		string alpha = Regex.Match(orientationData, regexPattern).Groups[1].Value;
		string beta  = Regex.Match(orientationData, regexPattern).Groups[2].Value;
		string gamma = Regex.Match(orientationData, regexPattern).Groups[3].Value;

		steerH = -((float)Convert.ToDouble(beta) / 100.0f);

		float acc = (float)Convert.ToDouble(gamma);

		if (acc >= 0.0)
			acc = 90.0f - (float)Convert.ToDouble(gamma);
		else
			acc = -90.0f - (float)Convert.ToDouble(gamma);

		steerV = (acc / 100.0f);
	}
}