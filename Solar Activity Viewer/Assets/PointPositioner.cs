
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointPositioner : MonoBehaviour {

    public GameObject flarePrefab;

    public GameObject CMEprefab;

    public GameObject earth;

    public Text HUD;

    private List<SolarFlare> linkedEventSolarFlareList;

    private SolarFlare[] initialSolarFlareArray;

    private CME[] CMEArray;

    private SFcomponent[] solarFlareDots;

    private CMEComponent[] CMEDots;

    static Vector3 myLocalScale;

    static List<string> linkedCMENames;

	// Use this for initialization
	void Start () {

        linkedCMENames = new List<string>();

        linkedEventSolarFlareList = new List<SolarFlare>();

        //Gets the arrays that are gotten from the NASA API
        initialSolarFlareArray = GetComponent<solarFlare_API>().GetArray();

        CMEArray = GetComponent<CME_API>().GetArray();

       
        for(int i = 0; i < initialSolarFlareArray.Length; i++)
        {
            if (initialSolarFlareArray[i].linkedEvents.Count != 0 && initialSolarFlareArray[i].linkedEvents[0].activityID.Contains("CME"))
            {
                linkedEventSolarFlareList.Add(initialSolarFlareArray[i]);
                
            }
        }
        Debug.Log(linkedEventSolarFlareList.Count);
        

        myLocalScale = gameObject.transform.localScale;
      

        GetComponent<SphereCollider>().radius = transform.localScale.x * 0.5f;
        for (int i = 0; i < linkedEventSolarFlareList.Count; i++)
        {
            Instantiate(flarePrefab, PositionObject(GetLatLongCoord(linkedEventSolarFlareList[i])), Quaternion.Euler(GetLatLongCoord(linkedEventSolarFlareList[i]).x, GetLatLongCoord(linkedEventSolarFlareList[i]).y, 0), transform);
        }

        for (int i = 0; i < CMEArray.Length; i++)
        {
            if (!linkedCMENames.Contains(CMEArray[i].activityID) && CMEArray[i].cmeAnalyses.Count != 0)
            {
                Instantiate(CMEprefab, PositionObject(new Vector2(CMEArray[i].cmeAnalyses[0].latitude, CMEArray[i].cmeAnalyses[0].longitude)), Quaternion.Euler(CMEArray[i].cmeAnalyses[0].latitude, CMEArray[i].cmeAnalyses[0].longitude, 0), transform);
            }
        }

        solarFlareDots = gameObject.GetComponentsInChildren<SFcomponent>();
        CMEDots = gameObject.GetComponentsInChildren<CMEComponent>();

        for(int i = 0; i < solarFlareDots.Length; i++)
        {
            solarFlareDots[i].gameObject.name = "SF " + i;
        }

        for (int i = 0; i < CMEDots.Length; i++)
        {
            CMEDots[i].gameObject.name = "CME " + i;
        }

    }
	
	// Update is called once per frame
	void Update () {
        if(myLocalScale != gameObject.transform.localScale)
        {
            GetComponent<SphereCollider>().radius = transform.localScale.x * 0.5f;
            for (int i = 1; i < linkedEventSolarFlareList.Count; i++)
            {
                solarFlareDots[i].gameObject.GetComponent<Transform>().position = PositionObject(GetLatLongCoord(linkedEventSolarFlareList[i]));
            }
        }
    }

    private Vector3 PositionObject(Vector2 coord)
    {
        
        float xPos = (GetComponent<SphereCollider>().radius) * Mathf.Cos(coord.x * Mathf.Deg2Rad) * Mathf.Cos(coord.y * Mathf.Deg2Rad);
        float zPos = (GetComponent<SphereCollider>().radius) * Mathf.Cos(coord.x * Mathf.Deg2Rad) * Mathf.Sin(coord.y * Mathf.Deg2Rad);
        float yPos = (GetComponent<SphereCollider>().radius) * Mathf.Sin(coord.x * Mathf.Deg2Rad);
        return new Vector3(xPos, yPos, zPos) + transform.position;
    }

    private Vector2 GetLatLongCoord(SolarFlare flare)
    {
        int correctIndex = -1;
        for(int i = 0; i < CMEArray.Length; i++)
        {
            if (Equals(flare.linkedEvents[0].activityID, CMEArray[i].activityID))
            {
                correctIndex = i;
                linkedCMENames.Add(CMEArray[i].activityID);
                i = CMEArray.Length;
            }
        }
        if (CMEArray[correctIndex].cmeAnalyses.Count != 0)
        {
            return new Vector2(CMEArray[correctIndex].cmeAnalyses[0].latitude, CMEArray[correctIndex].cmeAnalyses[0].longitude);
        }
        else
        {
            return new Vector2(0, 0);
        }
    }

    public string GetData(string name)
    {
        int index;
        //CME 1
        if (name.Contains("CME"))
        {
            index = Int32.Parse(name.Substring(4));
            SetEarthPos(index, 0);
            return CMEArray[index].ToString();
        }
        else if (name.Contains("SF"))
        {
            index = Int32.Parse(name.Substring(3));
            SetEarthPos(index, 1);
            return linkedEventSolarFlareList[index].ToString();
        }
        return null;
        
    }


    // type == 0: CME
    // type == 1: SF
    public void SetEarthPos(int index, int type)
    {
        float lat, lon;
        if (type == 0)
        {
            if (CMEArray[index].cmeAnalyses.Count != 0)
            {
                if (CMEArray[index].sourceLocation.Length != 0)
                {
                    if (CMEArray[index].sourceLocation[0] == 'S')
                    {
                        lat = Int32.Parse(CMEArray[index].sourceLocation.Substring(1, 2)) * -1;
                    }
                    else
                    {
                        lat = Int32.Parse(CMEArray[index].sourceLocation.Substring(1, 2));
                    }

                    if (CMEArray[index].sourceLocation[3] == 'W')
                    {
                        lon = Int32.Parse(CMEArray[index].sourceLocation.Substring(4, 2)) * -1;
                    }
                    else
                    {
                        lon = Int32.Parse(CMEArray[index].sourceLocation.Substring(4, 2));
                    }
                    float xPos = (GetComponent<SphereCollider>().radius + 1.5f) * Mathf.Cos(lat * Mathf.Deg2Rad) * Mathf.Cos(lon * Mathf.Deg2Rad);
                    float zPos = (GetComponent<SphereCollider>().radius + 1.5f) * Mathf.Cos(lat * Mathf.Deg2Rad) * Mathf.Sin(lon * Mathf.Deg2Rad);
                    lon = CMEArray[index].cmeAnalyses[0].longitude - lon;
                    if (lon > 180)
                    {
                        lon -= 360;
                    }
                    if (lon < -180)
                    {
                        lon += 360;
                    }


                    int stringPos, time;

                    if (CMEArray[index].startTime.Length != 0)
                    {
                        stringPos = CMEArray[index].startTime.IndexOf('T');
                        time = Int32.Parse(CMEArray[index].startTime.Substring(stringPos + 1, 2));
                    }
                    else
                    {
                        time = 12;
                    }
                    // lon is the already calculated lon
                    HUD.enabled = true;
                    HUD.text = "You can view the approximate position\nand rotation of the Earth in relation\nto this CME!";
                    earth.transform.SetPositionAndRotation(new Vector3(xPos, 0, zPos) + transform.position, Quaternion.Euler(0, lon - (Math.Abs(time - 24) / 24 * 360), 0));
                }
                else
                {
                    earth.SetActive(false);
                }

            }
            
        }
        else
        {
            if (linkedEventSolarFlareList[index].sourceLocation.Length != 0)
            {
                if (linkedEventSolarFlareList[index].sourceLocation[0] == 'S')
                {
                     lat = Int32.Parse(linkedEventSolarFlareList[index].sourceLocation.Substring(1, 2)) * -1;
                }
                else
                {
                     lat = Int32.Parse(linkedEventSolarFlareList[index].sourceLocation.Substring(1, 2));
                }

                if (linkedEventSolarFlareList[index].sourceLocation[3] == 'W')
                {
                     lon = Int32.Parse(linkedEventSolarFlareList[index].sourceLocation.Substring(4, 2)) * -1;
                }
                else
                {
                     lon = Int32.Parse(linkedEventSolarFlareList[index].sourceLocation.Substring(4, 2));
                }
                float xPos = (GetComponent<SphereCollider>().radius + 1.5f) * Mathf.Cos(lat * Mathf.Deg2Rad) * Mathf.Cos(lon * Mathf.Deg2Rad);
                float zPos = (GetComponent<SphereCollider>().radius + 1.5f) * Mathf.Cos(lat * Mathf.Deg2Rad) * Mathf.Sin(lon * Mathf.Deg2Rad);
                lon = GetLatLongCoord(linkedEventSolarFlareList[index]).y - lon;
                if (lon > 180)
                {
                    lon -= 360;
                }
                if (lon < -180)
                {
                    lon += 360;
                }
            
                int stringPos, time;
                if (linkedEventSolarFlareList[index].beginTime.Length != 0)
                {
                    stringPos = linkedEventSolarFlareList[index].beginTime.IndexOf('T');
                    time = Int32.Parse(linkedEventSolarFlareList[index].beginTime.Substring(stringPos + 1, 2));
                }
                else
                {
                    time = 12;
                }
                // lon is the already calculated lon
                HUD.enabled = true;
                HUD.text = "You can view the approximate position\nand rotation of the Earth in relation\nto this solar flare!";
                earth.transform.SetPositionAndRotation(new Vector3(xPos, 0, zPos) + transform.position, Quaternion.Euler(0, lon - (Math.Abs(time - 24) / 24 * 360), 0));
            }
            else
            {
                earth.SetActive(false);
            }
        }
    }

}
