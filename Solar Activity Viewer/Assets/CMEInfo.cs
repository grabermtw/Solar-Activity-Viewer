using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CME
{
    public string activityID;
    public List<Instrument> instruments;
    public string startTime;
    public string catalog;
    public string note;
    public List<CMEAnalysis> cmeAnalyses;
    public string sourceLocation;
    public int activeRegionNum;
    public List<ActivityID> linkedEvents;

    public string ToString()
    {
        string retString = "Activity ID: " + activityID + "\n";

        foreach (Instrument item in instruments)
        {
            retString += "Instrument Display Name: " + item.displayName + "\n";
        }

        // foreach  (CMEAnalysis item in cmeAnalyses)
        //{
        //retString += item.ToString();
        if (cmeAnalyses.Count != 0)
        {
            retString += cmeAnalyses[0].ToString();
        }
        //}

        retString += "Begin Time: " + startTime + "\n" +
                     "Peak Time: " + catalog + "\n" +
                     "sourceLocation: " + sourceLocation + "\n" +
                     "Active Region Number: " + activeRegionNum + "\n";

        foreach (ActivityID item in linkedEvents)
        {
            retString += "Linked Event: " + item.activityID;
        }

        retString += "Note: " + note;

        return retString;
    }
}

[Serializable]
public class CMEAnalysis
{
    public float latitude;
    public float longitude;
    public double halfAngle;
    public double speed;
    public string note;

    public string ToString()
    {
        return "Latitude: " + latitude + "\n" +
                "Longitude: " + longitude + "\n" +
                "Half Angle: " + halfAngle + "\n" +
                "Speed: " + speed + "\n" +
                "Note: " + note + "\n";
    }
}