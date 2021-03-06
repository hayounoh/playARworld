using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLocationService : MonoBehaviour
{

	public LatLon currentPos;
	public bool isChecking = false;

	public GUIStyle style;

	void Start(){
		style.fontSize = 100;

		StartCoroutine (CheckGPS ());

	}
		

	IEnumerator CheckGPS()
	{
		print ("started");
		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser)
			yield break;

		isChecking = true;

		// Start service before querying location
		Input.location.Start();

		// Wait until service initializes
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		print ("init or givup");


		// Service didn't initialize in 20 seconds
		if (maxWait < 1)
		{
			print("Timed out");
			yield break;
		}

		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed)
		{
			print("Unable to determine device location");
			yield break;
		}
		else
		{
			// Access granted and location value could be retrieved
			float lat = Input.location.lastData.latitude;
			float lon = Input.location.lastData.longitude;

			currentPos = new LatLon (lat, lon);

			print("Location: " + currentPos.lat + " " + currentPos.lon + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

		}

		// Stop service if there is no need to query location updates continuously
		Input.location.Stop();

		isChecking = false;

	}



	void OnGUI() {
		if (GUI.Button(new Rect(30, 70, 200, 100), new GUIContent("reload")))
		{
			if (!isChecking)
				StartCoroutine(CheckGPS ());
		}

		string lL = currentPos.lat + ", " + currentPos.lon;
		GUI.Label(new Rect(30, 500, 300, 300), new GUIContent(lL), style);

	}
}