using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class HayateScience : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public bool Active;
    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;

    public GameObject FadeToBlack;
    
    public VP_Dialog ContinueRoute;
    public bool Getsoda;
    // Start is called before the first frame update
    void Start()
    {
        ContinueRoute.IsCurrent = false;
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

            
        }


        if (Active == true)
        {
            
            if (Getsoda == false)
            {
                if (ContinueRoute.IsCurrent == true)
                {
                    StartCoroutine("Break");
                }

            }
            

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Active = true;
        HomeRoomChat1.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        Active = false;
    }
    IEnumerator Break()
    {
        yield return new WaitForSeconds(5f);
        FadeToBlack.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
   


}
