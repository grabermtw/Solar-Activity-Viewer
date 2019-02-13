using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SolarFlare
{
    public string flrID;
    public List<Instrument> instruments;
    public string beginTime;
    public string endTime;
    public string peakTime;
    public string classType;
    public string sourceLocation;
    public int activeRegionNum;
    public List<ActivityID> linkedEvents;

    public string ToString()
    {
        string retString = "Flare ID: " + flrID + "\n";

        foreach (Instrument item in instruments)
        {
            retString += "Instrument Display Name: " + item.displayName + "\n";
        }
        retString += "Begin Time: " + beginTime + "\n" +
                     "Peak Time: " + peakTime + "\n" +
                     "End Time: " + endTime + "\n" +
                     "Class Type: " + classType + "\n" +
                     "sourceLocation: " + sourceLocation + "\n" +
                     "Active Region Number: " + activeRegionNum + "\n";
        foreach (ActivityID item in linkedEvents)
        {
            retString += "Linked Event: " + item.activityID;
        }

        return retString;
    }
}

[Serializable]
public class Instrument
{
    public int id;
    public string displayName;
}

[Serializable]
public class ActivityID
{
    public string activityID;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}