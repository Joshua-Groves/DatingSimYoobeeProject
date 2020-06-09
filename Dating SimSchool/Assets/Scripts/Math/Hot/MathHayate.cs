using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class MathHayate : MonoBehaviour
{
    
    public bool Active;
    public Movement3D Kinetic;

    public GameObject HomeRoomChat1;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;
    public VP_Dialog Fail;

    public GameObject HomeRoomChat2;
    public VP_Dialog Begin2;
    public VP_Dialog ContinueRoute;


    public bool signature1;
    public bool signature2;
    public Signature1 sign;
    public Signature2 sign2;
      

    public GameObject FadeToBlack;
    public GameObject FadeFromBlack;

    public GameObject Kazuki;
    public GameObject Hideo;
    

    // Start is called before the first frame update
    void Start()
    {
        ContinueRoute.IsCurrent = false;
        
        Ignore.IsCurrent = false;
        Fail.IsCurrent = false;

        FadeFromBlack.SetActive(true);
        StartCoroutine("TurnOffblack");
    }

    // Update is called once per frame
    void Update()
    {
        if (Begin.IsCurrent == true)
        {
            Kinetic.kinetic = true;
            sign.Getsig = true;
            sign2.Getsig = true;
            Hideo.GetComponent<ScienceGoAway>().enabled = false;
            Kazuki.GetComponent<EriScienceKazuki>().enabled = false;
        }
        if (Begin2.IsCurrent == true)
        {
            Kinetic.kinetic = true;
        }
        if (Ignore.IsCurrent == true)
        {
            Kinetic.kinetic = false;
        }
        if (Fail.IsCurrent == true)
        {
            Kinetic.kinetic = false;
            StartCoroutine("FailRoute");
        }

        if (Active == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HomeRoomChat1.SetActive(true);

                if (signature1 == true && signature2 == true)
                {
                    HomeRoomChat2.SetActive(true);
                }

            }


            if (ContinueRoute.IsCurrent == true)
            {
                StartCoroutine("PE");
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
    IEnumerator PE()
    {
        yield return new WaitForSeconds(5f);
        FadeToBlack.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator FailRoute()
    {
        yield return new WaitForSeconds(5f);
        FadeToBlack.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

    IEnumerator TurnOffblack()
    {
        yield return new WaitForSeconds(4f);
        FadeFromBlack.SetActive(false);

    }
}
