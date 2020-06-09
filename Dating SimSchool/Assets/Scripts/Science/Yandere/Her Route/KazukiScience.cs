using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class KazukiScience : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public GameObject HomeRoomChat2;
    public bool Active;
    public bool Acknowledge;
    public bool HasCloth;
    public VP_Dialog Cloth;
    public Cloth Obtained;
    public VP_Dialog ContinueRoute;
    public GameObject FadeToBlack;
    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Begin2;
    public VP_Dialog Ignore;
    public GameObject FadeFromBlack;

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
        if (Cloth.IsCurrent == true)
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
            StartCoroutine("Fail");
            
        }

        if (Active == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HomeRoomChat1.SetActive(true);


                if (Acknowledge == true)
                {
                    if (HasCloth == true)
                    {
                        HomeRoomChat2.SetActive(true);

                    }
                    HomeRoomChat1.SetActive(false);

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
        Cloth.IsCurrent = false;

    }

    IEnumerator Break()
    {
        yield return new WaitForSeconds(5f);
        FadeToBlack.SetActive(true);
        Kinetic.kinetic = false;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator Fail()
    {
        yield return new WaitForSeconds(4f);
        FadeToBlack.SetActive(true);
        Kinetic.kinetic = false;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 20);

    }
    IEnumerator TurnOffblack()
    {
        yield return new WaitForSeconds(4f);
        FadeFromBlack.SetActive(false);

    }

    
}
