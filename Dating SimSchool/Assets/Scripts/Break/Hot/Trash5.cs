using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

public class Trash5 : MonoBehaviour
{
    public GameObject TheTrash;
    public HayateBreak GotIt;
    public bool Active;
    public bool Obtainable;
    public GameObject HomeRoomChat1;
    public VP_Dialog Begin;
    public Movement3D Kinetic;
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
            StartCoroutine("delay");
        }
        
        if (Obtainable == true)
        {
            if (Active == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    HomeRoomChat1.SetActive(true);
                    GotIt.HasTrash5 = true;
                    StartCoroutine("Trashgotten");
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
    IEnumerator Trashgotten()
    {
        yield return new WaitForSeconds(3.5f);
        TheTrash.SetActive(false);
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(3f);
        Kinetic.kinetic = false;
        Begin.IsCurrent = false;
        StopCoroutine("delay");
    }
}
