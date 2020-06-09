using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class Discovery : MonoBehaviour
{
    public bool Activated;
    private BoxCollider Open;

    public GameObject HomeRoomChat1;
    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;
    public VP_Dialog ContinueRoute;
    public GameObject FadeToBlack;
    public bool Active;
    public GameObject bush;

    // Start is called before the first frame update
    void Start()
    {
        Open = GetComponent<BoxCollider>();
        ContinueRoute.IsCurrent = false;
        Ignore.IsCurrent = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Activated == true)
        {
            Open.isTrigger = true;
            bush.SetActive(false);
        }
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
            
            if (ContinueRoute.IsCurrent == true)
            {
                StartCoroutine("PE");
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
    IEnumerator PE()
    {
        yield return new WaitForSeconds(2f);
        FadeToBlack.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator Disable()
    {
        yield return new WaitForSeconds(5f);
        FadeToBlack.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 18);
    }
    
}
