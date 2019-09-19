# Solar-Activity-Viewer
An application visualizing the locations of the origin points of coronal mass ejections on the Sun. This was created for University of Maryland's Time Domain Astronomy Hackathon that took place in November 2018.

*The following explanation was written for CMSC389U: Intro to AR App Development with Microsoft HoloLens, and was taught the week following the creation of Solar Activity Viewer at the hackathon*


## Using an API with HoloLens

As of November 2018, the Microsoft HoloLens is currently incapable of directly calling an API (or at least, the two most common methods of doing so don’t work, they work in Unity, but not when they’re built to HoloLens).

A workaround for this is to save the data you want from the API into a .txt file, upload it to the HoloLens, and then have it read from it.

Read this document to see how it can be done in three easy steps!

### Getting Data:

For this example, we will be using https://api.nasa.gov/api.html#donkiFLR, which is NASA’s API for accessing data on solar flares. This website is very good for getting data, the solar flare API is just one of many available, and the process is nearly identical for each.

Look at the example given for the solar flare API on the website (or the example given for whichever API you are using):

https://api.nasa.gov/DONKI/FLR?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd&api_key=DEMO_KEY

Replace the first yyyy-MM-dd in the URL with the first date you’d like to get data from, and the second yyyy-MM-dd with the last date you’d like to get data from.

Then replace DEMO_KEY at the end of the URL with your API key. You can get a NASA API key almost instantaneously by filling out this form: https://api.nasa.gov/index.html#apply-for-an-api-key
It’s free, they’ll email it to you, mine came only a few seconds after I filled out the form.

Here is an example key that gets solar flare data between May 26, 2015 and November 11, 2018:

https://api.nasa.gov/DONKI/FLR?startDate=2015-05-26&endDate=2018-11-11&api_key=Hj3AgfMw4ahroXDJCOKY5BBVJbYBS6a3aKpG755c

Notice how the dates have replaced yyyy-MM-dd in the first example, and how DEMO_KEY has been replaced by an actual key at the end after *api_key=*

Once you have assembled your URL, paste it into your web browser and go to it. It should give you a lot of text representing whatever data you’re getting. This text (at least coming from NASA) will be in the form of JSON (JavaScript Object Notation), with which we can use to make our own classes.

Create a new .txt document on your computer (not sure how it’s done on Mac, but in Windows you can just open Notepad), and then copy and paste all the text that you got from going to the URL you assembled into the .txt, and then save it as something meaningful (for example: solar_flare.txt). Save it to wherever you’d like on your computer, but make sure you know where it is.

### Reading from a file full of JSONs:

*Part 1: Make a class for whatever the data is for*

The goal is for the HoloLens to read the .txt during runtime and get meaningful data from it, so we will write the code for that part now.

First, create the class that will store the data for each individual object instance. For example, when you get the solar flare data using the example URL above, it doesn’t just give you information on one solar flare, it gives you information on several solar flares.

We need a way to store the data from each JSON object the API gives us (which we will eventually read from the .txt), so we will create a class to store the data from each object in an instance of the class.

Basically, we’re gonna write a SolarFlare class that has fields such as “beginTime”, “endTime”, “flrID”, etc.

Here is our class for our Solar Flare. The individual fields will most likely be different for whatever data you are getting, so be sure to look at the JSON objects in the .txt to see what the name of each field is.

```
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Notice this class is NOT a Monobehaviour
[Serializable]
public class SolarFlare
{
// These fields can all be found within the .txt, make sure you give them the same name and list
// them in the same order. For instance, in the .txt for the solar flares, each JSON begins with 
// “flrID”: and is followed by a string. Thus, we begin with a public string flrID.
// Also, it’d probably be good practice to make these private and then write getters for the class,
// but just making them all public is fine for what we’re doing.

    public string flrID;
    // Within each JSON for the solar flare, we have in the “instruments” field: 
    // "instruments":[{"id":11,"displayName":"GOES15: SEM/XRS 1.0-8.0"}]
    // 11 is an int (it’s not 11 for every object, but it’s always an int), and "GOES15: SEM/XRS     1.0-8.0" is a string (it’s not always going to be that string, but it will be a string), so we will make
    // our own nested class of type Instrument with an int and a string later in the class. Because
    // “instruments” is followed by brackets (i.e. [ ]), this means we will have to make the
    // instruments field a List of Instruments:

    public List<Instrument> instruments;
    public string beginTime;
    public string endTime;
    public string peakTime;
    public string classType;
    public string sourceLocation;
    public int activeRegionNum;

    // Like with the instruments above, we can see from the JSONs in the .txt that the
    // linkedEvents field can contain multiple linked events
    // (meaning other solar flares or coronal mass ejections), and each linked event is kept track
    // of using its ActivityID. For example:
//"linkedEvents":[{"activityID":"2015-06-18T09:25:00-SEP-001"},{"activityID":"2015-06-18T15:30:00-SEP-001"}]
    // This has two linked events, and each one is a string preceded by “activityID”:, thus, we
    // need to make an activityID class consisting of a string called activityID. Therefore,
    // linkedEvents must be a List of type ActivityID, which will be a nested class we will create
    // later in this class.

    public List<ActivityID> linkedEvents;

   // This is a custom ToString() method (hence the override) I wrote that just returns a string
   // consisting of all the data in the class. You might not need one depending on what you’re
   // doing. I only had one in this project so that all the data could be easily sent to a textMesh
   // object in the scene in a different script.
    public override string ToString()
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

// Here is our Instrument class as described in a comment above. We need this for
// List<Instrument> instruments, as the field “instruments” in the JSON object can contain
// multiple instruments, each containing an int called “id” and a string called “displayName”, and
// thus those are the names of the two fields in this class.
[Serializable]
public class Instrument
{
    public int id;
    public string displayName;
}

// As described above, the field in each JSON called “linkedEvents” can contain multiple linked
// events, each containing a string called “activityID”. Thus, we need a class containing only a
// string called “activityID” to use in List<ActivityID> linkedEvents above
[Serializable]
public class ActivityID
{
    public string activityID;
}

// If you are using an API formatted similar to the solar flare API (or most other NASA APIs),
// then just copy and paste the rest of this into your class. It will allow for getting data from a file
// with multiple JSON instances (for example, there’s usually data for multiple solar flares that’s
// given when we call the solar flare API). This may not always be the same for every API,
// though, if yours is different, refer to this link for help (this guy showed an example of almost
// every case for getting data from an API in Unity):
//https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
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
```

Once you’ve got your script written, just make sure that it’s saved in your Assets folder in your Unity project. We won’t actually need to attach this script to any gameObjects, it just needs to be in Assets.


*Part 2: Write a script that can access the data from the .txt and save it in an array of objects of type SolarFlare (or whatever the class is you just made)*

To access our .txt and save all its data into an array of SolarFlare objects, we will write the following script:

```
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Unlike the SolarFlare class, this class will be a MonoBehaviour, as we will be attaching it to a
// gameObject
public class solarFlare_API_Hololens : MonoBehaviour {

    // This int will be the maximum number of solar flares we get from the API. You can make this
    // any size you see as appropriate, we will only be using it to give our array a size. You can
    // end up having fewer than 800 solar flares, but not more.
    private const int NUM_SOLAR_FLARES = 800;

    private SolarFlare[] solarFlareInstances = new SolarFlare[NUM_SOLAR_FLARES];

    
    void Start () 
    {
        // For this example, we’ve put ReadTxtJson() in Start() so that the function is executed as
        // soon as the program begins running. You don’t need to do this if you want to call it from a
        // different script later or something.
        ReadTxtJson();
    }


 void ReadTxtJson()
    {
       // Application.persistentDataPath is the location of where we will save our SolarFlare.txt (or
       // whatever you called your .txt) once we have finished writing our script.
       // Application.persistentDataPath gives us a different file location depending on the device
       // (for example, for iOS devices it would give us /var/mobile/Containers/Data/Application/<guid>/Documents), but it always is a location specific
       // to the specific application that we are working with, to give us a safe place to store data.
       // For the HoloLens, Application.persistentDataPath is \AppData\Local\Packages\<productname>\LocalState. We will see more of this later on.
       // we add “\\SolarFlare.txt” to the end of Application.persistentDataPath in our path string
       // because that is the name of the file located at that path that we are looking for.
       // Use \\ because Application.persistentDataPath does not end with a \, and we need one
       // before our file name in our path.
        string path = Application.persistentDataPath + "\\SolarFlare.txt";
        
        // This places all the raw text from SolarFlare.txt in a string
        string jsonString = File.ReadAllText(path);

        // This line is necessary for the string to be successfully read
        jsonString = "{\r\n    \"Items\": " + jsonString + "}";
        // solarFlareInstances is our array of instances of SolarFlare that we get from the JSONs in
        // this line.
        // Make sure you have the correct type name here in JsonHelper.FromJson<T>, change it
        // to whatever your class name is that you wrote for your JSONs.
        solarFlareInstances = JsonHelper.FromJson<SolarFlare>(jsonString);

        
    }

    // Returns the array of SolarFlare object instances that we got from reading the .txt, you’ll
    // need this to access your SolarFlares in another script.
    public SolarFlare[] GetArray()
    {
        return solarFlareInstance;
    }
}
```

Make sure this script is attached to a GameObject.

Great! Now that this is written, you can use this in other scripts by writing code such as the following:

```
// In another script: (we assume for this example that this script is attached to the same GameObject as solar_flare_API_Hololens, but it doesn’t have to be)
// Gets the array of solarFlare instances
SolarFlare[] solarFlareArray;
solarFlareArray = GetComponent<solarFlare_API_Hololens>().GetArray();

// Prints the IDs of every solar flare
for (int i = 0; i < solarFlareArray.length; i++) 
{
Debug.Log(solarFlareArray[i].flrID);
}
```

Basically, in another script, just use solarFlareArray[index].flrID (or .instruments or .beginTime or whatever you’d like) to access the fields of the solar flare at that index of the array, assuming you made them all public, and then do what you’d like with that data.

Now you can build your project and deploy it to HoloLens!


## Uploading the .txt to HoloLens:

Once your project has been successfully built to HoloLens, you can upload your .txt with your JSONs to the HoloLens.

First connect to the HoloLens’s Device Portal by typing its IP address into your web browser (make sure the HoloLens is on first though).
It will probably say the webpage is not secure and discourage you from continuing. Ignore this, just click “Continue to webpage”.

You will be prompted to enter a username and password. For all the HoloLenses this should be as follows:
Username: xrclub
Password: xrclub1029!
If that doesn’t work, try arclub and arclub1029! as the username and password, respectively.

The device portal should then show up. Navigate to “File Explorer,” and then navigate to AppData\Local\Packages\<YourProject’sName>\LocalState (remember, this is the path that is given by Application.persistentDataPath).

Click “Browse”, and browse to your .txt with your JSONs that you saved somewhere on your computer, and select that file.

Then click upload. The .txt should upload to the HoloLens in the LocalState folder.

Now you’re done! Run your project and make sure it works.

If you would like to change the amount of data that is given (i.e. add more solar flares to the .txt or something along those lines), you don’t have to rebuild your entire project! Just delete the .txt that you first uploaded, and replace it with another of the same name.

However, if you change something else in your project and do end up rebuilding it, you will need to upload the .txt again to the LocalState folder for your project in the HoloLens, as it will be cleared out every time you rebuild your project.
