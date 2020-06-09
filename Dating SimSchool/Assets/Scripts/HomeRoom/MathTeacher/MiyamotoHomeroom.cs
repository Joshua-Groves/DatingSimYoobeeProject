using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using VirtualPhenix.Dialog;

public class MiyamotoHomeroom : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;

    // Start is called before the first frame update
    void Start()
    {
        HomeRoomChat1.SetActive(true);
        Ignore.IsCurrent = false;
        Kinetic.speed = 0;
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
    }
    IEnumerator Disable()
    {
        yield return new WaitForSeconds(2f);
        Ignore.IsCurrent = false;
        yield return new WaitForSeconds(1f);
        StopCoroutine("Disable");
    }
}
