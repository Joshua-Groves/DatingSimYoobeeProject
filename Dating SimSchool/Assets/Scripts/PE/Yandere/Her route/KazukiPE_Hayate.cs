using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

public class KazukiPE_Hayate : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public bool Active;
    public Movement3D Kinetic;
    public KazukiPECont PlusOne;
    public VP_Dialog Begin;

    // Start is called before the first frame update
    void Start()
    {
        Begin.IsCurrent = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Begin.IsCurrent == true)
        {
            Kinetic.kinetic = true;
            StartCoroutine("Move");
        }
        if (Active == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HomeRoomChat1.SetActive(true);
                PlusOne.Hayate = true;
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
    IEnumerator Move()
    {
        yield return new WaitForSeconds(4f);
        Begin.IsCurrent = false;
        Kinetic.kinetic = false;
    }
}
