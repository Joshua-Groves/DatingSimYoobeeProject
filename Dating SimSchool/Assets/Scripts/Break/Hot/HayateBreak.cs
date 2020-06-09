using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class HayateBreak : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public GameObject HomeRoomChat2;
    public bool Active;
    public bool Acknowledge;
    public bool HasTrash;
    public VP_Dialog Trash;
    public Trash Obtained;
    public Trash2 Obtained2;
    public Trash3 Obtained3;
    public Trash4 Obtained4;
    public Trash5 Obtained5;
    public VP_Dialog ContinueRoute;
    public GameObject FadeToBlack;
    public GameObject FadeFromBlack;
    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Begin2;
    public VP_Dialog Ignore;
    public VP_Dialog Ignore2;
    public bool HasTrash2;
    public bool HasTrash3;
    public bool HasTrash4;
    public bool HasTrash5;

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

        if (Trash.IsCurrent == true)
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                HomeRoomChat1.SetActive(true);


                if (Acknowledge == true)
                {
                    if (HasTrash == true && HasTrash2 == true && HasTrash3 == true && HasTrash4 == true && HasTrash5 == true)
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
        yield return new WaitForSeconds(1f);
        Acknowledge = true;
        Kinetic.kinetic = false;
        Obtained.Obtainable = true;
        Obtained2.Obtainable = true;
        Obtained3.Obtainable = true;
        Obtained4.Obtainable = true;
        Obtained5.Obtainable = true;
        Trash.IsCurrent = false;

    }

    IEnumerator PE()
    {
        yield return new WaitForSeconds(4f);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 4);
    }

    IEnumerator TurnOffblack()
    {
        yield return new WaitForSeconds(4f);
        FadeFromBlack.SetActive(false);

    }


}
