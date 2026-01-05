using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenController : MonoBehaviour
{
    public String sceneName;
    public String endingGallery;
    public String option;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStart()
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void EedingGallery()
    {
        SceneManager.LoadScene(endingGallery);
    }

    public void Option()
    {
        SceneManager.LoadScene(option);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
