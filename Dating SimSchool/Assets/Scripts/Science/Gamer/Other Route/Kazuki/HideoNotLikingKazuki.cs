using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

public class HideoNotLikingKazuki : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public bool Active;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;
    public Movement3D Kinetic;

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
            
            StartCoroutine("Disable");
        }
        if (Active == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HomeRoomChat1.SetActive(true);

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
    IEnumerator Disable()
    {
        yield return new WaitForSeconds(2f);
        
        Ignore.IsCurrent = false;
        yield return new WaitForSeconds(1f);
        StopCoroutine("Disable");
    }
}
