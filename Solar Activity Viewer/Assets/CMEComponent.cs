using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMEComponent : MonoBehaviour {

    public int number;

    // Use this for initialization
    void Start()
    {
        number = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetNum()
    {
        return number;
    }
}
