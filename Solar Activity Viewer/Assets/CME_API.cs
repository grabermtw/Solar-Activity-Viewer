using System.Net;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Security;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CME_API : MonoBehaviour
{
    public ButtonReceiver buttonThing;

    public TextMesh textMesh;

    public Text HUD;

    private const int NUM_CME = 7000;
    private const string API_KEY = "Hj3AgfMw4ahroXDJCOKY5BBVJbYBS6a3aKpG755c";

    private CME[] CMEInstance = new CME[NUM_CME];

   private void Start()
    {
        StartCoroutine(StartEnumerating());
    }

    // Use this for initialization
    IEnumerator StartEnumerating()
    //private void Start()
    {
        Debug.Log("Started!!!");
        yield return StartCoroutine(CallAPI());
        Debug.Log("bout to enable pointpostioner");
        GetComponent<PointPositioner>().enabled = true;
        buttonThing.enabled = true;
        textMesh.text = "Tap on a dot to begin!";
        HUD.text = "Tap on a dot to begin!\nBlue dots represent coronal mass ejections,\nand green dots represent solar flares";
    }

    // Creates an array of 
    IEnumerator CallAPI()
    //void CallAPI()
    {
        string url = "https://api.nasa.gov/DONKI/CME?startDate=2012-11-11&endDate=2018-11-11&api_key=" + API_KEY;

        Debug.Log(url);

        WWW www = new WWW(url);
        yield return www;

        for (int i = 0; i < 800; i++)
        {
            CMEInstance[i] = new CME();
        }

          if (www.error == null)
          {
              string jsonString = "{\r\n    \"Items\": " + www.text + "}";
              CMEInstance = JsonHelper.FromJson<CME>(jsonString);
              Debug.Log(CMEInstance[0].cmeAnalyses[0].longitude);
          }
          else
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
                CMEInstance = JsonHelper.FromJson<CME>(jsonString);
                Debug.Log(CMEInstance[0].cmeAnalyses[0].longitude + "HELLO I WORKED");
            }
        }
        
    */
        //string path = Application.persistentDataPath + "\\CME.txt";
        //Debug.Log("CME path: " + path);
        //string jsonString = File.ReadAllText(path);
        //jsonString = "{\r\n    \"Items\": " + jsonString + "}";
        //CMEInstance = JsonHelper.FromJson<CME>(jsonString);
        //Debug.Log(CMEInstance[0].cmeAnalyses[0].longitude + "HELLO I WORKED");
    }

    public CME[] GetArray()
    {
        return CMEInstance;
    }
}
