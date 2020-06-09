using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

public class MathTest2 : MonoBehaviour
{
    public GameObject HomeRoomChat1;


    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;
    


    public VP_Dialog ContinueRoute;
    // Start is called before the first frame update
    void Start()
    {
        ContinueRoute.IsCurrent = false;
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
        if (ContinueRoute.IsCurrent == true)
        {
            Kinetic.kinetic = false;
            
        }



    }
    private void OnTriggerEnter(Collider other)
    {

        HomeRoomChat1.SetActive(true);
    }
}
