using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class ButtonReceiver : InteractionReceiver {

    public GameObject sun;
    public TextMesh textDisplay;
    public Text text;
    public GameObject earth;
    
    private Transform[] sunDotsTrans;

    private int numObjs;

	// Use this for initialization
	void Start () {
        
        sunDotsTrans = sun.GetComponentsInChildren<Transform>();

        numObjs = sunDotsTrans.Length;
        for(int i = 1; i < numObjs; i++)
        {
            base.interactables[i-1] = sunDotsTrans[i].gameObject;
        }
	}
	
	protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        earth.SetActive(true);
       // ParticleSystem ps;
        
        for(int i = 0; i < numObjs - 1; i++)
        {
            base.interactables[i].transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
          //  ps = base.interactables[i].GetComponent<ParticleSystem>();
         //   var em = ps.emission;
           // em.enabled = false;
        }
        obj.transform.localScale = obj.transform.localScale * 2f;
        //  ps = obj.GetComponent<ParticleSystem>();
        // var emm = ps.emission;
        // emm.enabled = true;
        text.enabled = false;
        textDisplay.text = sun.GetComponent<PointPositioner>().GetData(obj.name);
        
    }
}
