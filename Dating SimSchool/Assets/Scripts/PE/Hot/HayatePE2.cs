using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class HayatePE2 : MonoBehaviour
{
    public bool HasBag;
    public bool Active;
    public Movement3D Kinetic;
    public GameObject HomeRoomChat1;
    public VP_Dialog Begin;
    public VP_Dialog Ignore;
    public VP_Dialog ContinueRoute;
    public GameObject FadeToBlack;
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
            
            StartCoroutine("Disable");
        }
        if (ContinueRoute.IsCurrent == true)
        {
            StartCoroutine("End");
        }
        if (HasBag == true)
        {
            if (Input.GetKey(KeyCode.E))
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
    IEnumerator End()
    {
        yield return new WaitForSeconds(5f);
        GlobalControl.Instance.Hayate = true;
        FadeToBlack.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator Disable()
    {
        yield return new WaitForSeconds(5f);
        FadeToBlack.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
}
