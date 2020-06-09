using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class MihoMath : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public GameObject HomeRoomChat2;
    public bool Active;
    public bool Acknowledge;
    public bool HasVial;
    public VP_Dialog Vial;
    public Vial Obtained;
    public VP_Dialog ContinueRoute;
    public GameObject FadeToBlack;
    public GameObject FadeFromBlack;
    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Begin2;
    public VP_Dialog Ignore;
    public GameObject Eri;
    // Start is called before the first frame update
    void Start()
    {
        ContinueRoute.IsCurrent = false;
        Ignore.IsCurrent = false;
        FadeFromBlack.SetActive(true);
        StartCoroutine("TurnOffblack");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vial.IsCurrent == true)
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

        if (Active == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HomeRoomChat1.SetActive(true);


                if (Acknowledge == true)
                {
                    if (HasVial == true)
                    {
                        HomeRoomChat2.SetActive(true);

                    }
                    HomeRoomChat1.SetActive(false);

                }

            }


        }
        if (ContinueRoute.IsCurrent == true)
        {
            StartCoroutine("PE");
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

    IEnumerator Good()
    {
        yield return new WaitForSeconds(2f);
        Eri.SetActive(false);
        Acknowledge = true;
        Kinetic.kinetic = false;
        Obtained.Obtainable = true;
        Vial.IsCurrent = false;

    }

    IEnumerator PE()
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
        FadeToBlack.SetActive(true);
        Kinetic.kinetic = false;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 13);
    }
    IEnumerator TurnOffblack()
    {
        yield return new WaitForSeconds(4f);
        FadeFromBlack.SetActive(false);

    }


}
