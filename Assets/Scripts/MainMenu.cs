using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public GameObject menupanel;
    public GameObject creditspanel;
    public GameObject settingpanel;
    // Start is called before the first frame update
    void Start()
    {
        menupanel.SetActive(true);
        creditspanel.SetActive(false);
        settingpanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void CreditsButton()
    {
        menupanel.SetActive(false);
        creditspanel.SetActive(true);
        settingpanel.SetActive(false);
    }

    public void SettingsButton()
    {
        menupanel.SetActive(false);
        creditspanel.SetActive(false);
        settingpanel.SetActive(true);
    }

    public void BackButton()
    {
        menupanel.SetActive(true);
        creditspanel.SetActive(false);
        settingpanel.SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}
