using System.Net;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Security;
using UnityEngine.Networking;

public class solarFlare_API : MonoBehaviour {

    private const int NUM_SOLAR_FLARES = 800;
    private const string API_KEY = "Hj3AgfMw4ahroXDJCOKY5BBVJbYBS6a3aKpG755c";

    private SolarFlare[] solarFlareInstance = new SolarFlare[NUM_SOLAR_FLARES];

    // Use this for initialization
    void Start () {
        StartCoroutine(CallAPI());
        //CallAPI();
    }

    // Creates an array of 
    IEnumerator CallAPI()
    //void CallAPI()
    {
        string url = "https://api.nasa.gov/DONKI/FLR?startDate=2012-11-11&endDate=2018-11-11&api_key=" + API_KEY;

        Debug.Log(url);

        WWW www = new WWW(url);
        yield return www;

        for (int i = 0; i < 800; i++)
        {
            solarFlareInstance[i] = new SolarFlare();
        }

          if (www.error == null)
          {
              string jsonString = "{\r\n    \"Items\": " + www.text + "}";
              solarFlareInstance = JsonHelper.FromJson<SolarFlare>(jsonString);
              Debug.Log(solarFlareInstance[0].beginTime);
          } else
          {
              Debug.Log("ERROR: " + www.error);
          }
        /*  
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);
                string jsonString = "{\r\n    \"Items\": " + www.downloadHandler.text + "}";
                solarFlareInstance = JsonHelper.FromJson<SolarFlare>(jsonString);
                Debug.Log(solarFlareInstance[0].flrID + "HELLO I WORKED");
            }
        }
        
        string path = Application.persistentDataPath + "\\SolarFlare.txt";
        Debug.Log("Solar Flare path: " + path);
        string jsonString = File.ReadAllText(path);
        jsonString = "{\r\n    \"Items\": " + jsonString + "}";
        solarFlareInstance = JsonHelper.FromJson<SolarFlare>(jsonString);
        Debug.Log(solarFlareInstance[0].flrID + "HELLO I WORKED");
        */
    }

    public SolarFlare[] GetArray()
    {
        return solarFlareInstance;
    }
}
