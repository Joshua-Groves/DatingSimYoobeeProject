using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class MihoPE : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public bool Active;
    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;
    
    public GameObject FadeToBlack;
    public GameObject FadeFromBlack;
    public VP_Dialog ContinueRoute;
    
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
        if (Begin.IsCurrent == true)
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

            }
            if (ContinueRoute.IsCurrent == true)
            {
                StartCoroutine("END");
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
    IEnumerator END()
    {
        yield return new WaitForSeconds(5f);
        FadeToBlack.SetActive(true);
        GlobalControl.Instance.Miho = true;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator Disable()
    {
        yield return new WaitForSeconds(2f);
        FadeToBlack.SetActive(true);
        Ignore.IsCurrent = false;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 12);
        StopCoroutine("Disable");
    }
    IEnumerator TurnOffblack()
    {
        yield return new WaitForSeconds(4f);
        FadeFromBlack.SetActive(false);

    }
}
