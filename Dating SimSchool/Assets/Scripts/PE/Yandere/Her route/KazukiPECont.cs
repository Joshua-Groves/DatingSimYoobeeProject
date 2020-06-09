using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VirtualPhenix.Dialog;
using UnityEngine.SceneManagement;

public class KazukiPECont : MonoBehaviour
{
    public bool StudentA;
    public bool StudentB;
    public bool PETeacher;
    public bool Eri;
    public bool Rui;
    public bool Hayate;
    
    public GameObject FadeToBlack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (StudentA == true && StudentB == true && PETeacher == true && Eri == true && Rui == true && Hayate == true)
        {
            StartCoroutine("KazukiEnding");
        }
    }
    IEnumerator KazukiEnding()
    {
        yield return new WaitForSeconds(5f);
        GlobalControl.Instance.Kazuki = true;
        FadeToBlack.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
