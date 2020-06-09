using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class GetMeaSoda : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public GameObject HomeRoomChat2;
    public bool Active;
    public bool Acknowledge;
    public bool HasSoda;
    public VP_Dialog Soda;
    public Soda Obtained;
    public VP_Dialog ContinueRoute;
    public GameObject FadeToBlack;
    public GameObject FadeFromBlack;
    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Begin2;
    public VP_Dialog Ignore;
    public VP_Dialog Ignore2;
    public GameObject Meeting;
    public HayateScience SodaOk;
    // Start is called before the first frame update
    void Start()
    {
        ContinueRoute.IsCurrent = false;
        Ignore.IsCurrent = false;
        Meeting.SetActive(false);
        FadeFromBlack.SetActive(true);
        StartCoroutine("TurnOffblack");

    }

    // Update is called once per frame
    void Update()
    {
        if (Soda.IsCurrent == true)
        {
            StartCoroutine("Good");

        }
        if (Begin.IsCurrent == true)
        {
            Kinetic.kinetic = true;
        }
        if (Begin2.IsCurrent == true)
        {
            Kinetic.kinetic = true;
        }
        if (Ignore.IsCurrent == true)
        {

            Kinetic.kinetic = false;
            
            StartCoroutine("Disable");
        }
        if (Ignore2.IsCurrent == true)
        {

            Kinetic.kinetic = false;

            StartCoroutine("Disable");
        }

        if (Active == true)
        {
            if (Acknowledge == true)
            {
                SodaOk.Getsoda = true;
                if (HasSoda == true)
                {
                    HomeRoomChat2.SetActive(true);

                }


            }


        }
        if (ContinueRoute.IsCurrent == true)
        {
            StartCoroutine("Break");
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        Active = true;
        HomeRoomChat1.SetActive(true);
        Meeting.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        Active = false;
    }

    IEnumerator Good()
    {
        yield return new WaitForSeconds(2f);
        Acknowledge = true;
        Kinetic.kinetic = false;
        Obtained.Obtainable = true;
        Soda.IsCurrent = false;

    }

    IEnumerator Break()
    {
        yield return new WaitForSeconds(2f);
        FadeToBlack.SetActive(true);
        Kinetic.kinetic = false;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(2f);

        Ignore.IsCurrent = false;
        Ignore2.IsCurrent = false;
        yield return new WaitForSeconds(1f);
        StopCoroutine("Disable");
    }
    IEnumerator TurnOffblack()
    {
        yield return new WaitForSeconds(4f);
        FadeFromBlack.SetActive(false);

    }


}
