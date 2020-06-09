using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class RuiHomeRoom : MonoBehaviour
{
    public GameObject HomeRoomChat1;
    public bool Active;
    public VP_Dialog ContinueRoute;
    public GameObject FadeToBlack;
    public Movement3D Kinetic;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;
    public VP_Dialog Ignore2;
    public Failure Fail;
    

    // Start is called before the first frame update
    void Start()
    {
        ContinueRoute.IsCurrent = false;
        Ignore.IsCurrent = false;
        Ignore2.IsCurrent = false;
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
            Fail.RuiFail = true;
            StartCoroutine("Disable");
        }
        if (Ignore2.IsCurrent == true)
        {
            Kinetic.kinetic = false;
            Fail.RuiFail = true;
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
                StartCoroutine("Science");
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
    IEnumerator Science()
    {
        yield return new WaitForSeconds(5f);
        FadeToBlack.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 6);
    }
    IEnumerator Disable()
    {
        yield return new WaitForSeconds(2f);
        Ignore2.IsCurrent = false;
        Ignore.IsCurrent = false;
        yield return new WaitForSeconds(1f);
        StopCoroutine("Disable");
    }
}
