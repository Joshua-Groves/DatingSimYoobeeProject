using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

public class Signature1 : MonoBehaviour
{
    public bool Getsig;

    public bool Active;
    public Movement3D Kinetic;
    public MathHayate Signed;
    public GameObject HomeRoomChat1;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;
    // Start is called before the first frame update
    void Start()
    {
        Ignore.IsCurrent = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Begin.IsCurrent == true)
        {
            Kinetic.kinetic = true;
        }
        if (Ignore.IsCurrent == true)
        {
            Kinetic.kinetic = false;
        }

        if (Active == true)
        {
            if(Getsig == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    HomeRoomChat1.SetActive(true);
                    Signed.signature1 = true;
                }
                    
            }
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Active = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Active = false;
    }
}
