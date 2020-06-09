using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class HayatePE1 : MonoBehaviour
{
    public bool Active;
    public Movement3D Kinetic;
    public BAG Obtain;
    public GameObject HomeRoomChat1;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;
    public VP_Dialog Ignore2;
    public GameObject FadeToBlack;
    public GameObject FadeFromBlack;
    // Start is called before the first frame update
    void Start()
    {
        Ignore.IsCurrent = false;
        Ignore2.IsCurrent = false;
        FadeFromBlack.SetActive(true);
        StartCoroutine("TurnOffblack");
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
            Obtain.Obtainable = true;
            Kinetic.kinetic = false;
        }
        if (Ignore2.IsCurrent == true)
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
        FadeToBlack.SetActive(true);
        Ignore.IsCurrent = false;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        StopCoroutine("Disable");
    }
    IEnumerator TurnOffblack()
    {
        yield return new WaitForSeconds(4f);
        FadeFromBlack.SetActive(false);

    }
}
