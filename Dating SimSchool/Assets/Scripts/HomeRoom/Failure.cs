using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Failure : MonoBehaviour
{
    public bool RuiFail;
    public bool KazukiFail;
    public bool HayateFail;
    public bool HideoFail;
    public GameObject Fade;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RuiFail == true && KazukiFail == true && HayateFail == true && HideoFail == true)
        {
            Fade.SetActive(true);
            StartCoroutine("Alone");
        }
    }

    IEnumerator Alone()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 21);
    }
}
