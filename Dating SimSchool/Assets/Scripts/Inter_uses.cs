using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inter_uses : MonoBehaviour
{
    public float Homeroom = 5f;
    public float Science = 5f;
    public float Break = 5f;
    public float Math = 5f;
    public float PhysicalEd = 5f;
    public bool HMR;
    public bool SCI;
    public bool BRK;
    public bool MTH;
    public bool PE;
    public GameObject clock1;
    public GameObject clock2;
    public GameObject clock3;
    public GameObject clock4;
    public GameObject clock5;


    // Start is called before the first frame update
    void Start()
    {
        HMR = true;
        SCI = false;
        BRK = false;
        MTH = false;
        PE = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (HMR == true)
        {
            if (Homeroom == 5f)
            {
                clock1.SetActive(true);
                clock2.SetActive(true);
                clock3.SetActive(true);
                clock4.SetActive(true);
                clock5.SetActive(true);
            }
            if (Homeroom == 4f)
            {
                clock1.SetActive(false);              
            }
            if (Homeroom == 3f)
            {      
                clock2.SetActive(false);               
            }
            if (Homeroom == 2f)
            {              
                clock3.SetActive(false);            
            }
            if (Homeroom == 1f)
            {               
                clock4.SetActive(false);              
            }
            if (Homeroom == 0f)
            {               
                clock5.SetActive(false);
            }
        }

    }
}
