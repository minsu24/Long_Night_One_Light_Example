using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainScreenController : MonoBehaviour
{
    public GameObject creat;
    public GameObject saveWindow;
    public TextMeshProUGUI[] slotText;
    public TextMeshProUGUI newPlayerName;

    public String sceneName;
    public String endingGallery;
    public String option;
    bool[] savefile = new bool[3];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //슬롯별로 저장된 데이터가 존재하는지 판단
        for(int i = 0; i < 3; i++)
        {
            if(File.Exists(DataManager.instance.path + $"{i}"))
            {
                savefile[i] = true;
                DataManager.instance.nowSlot = i;
                DataManager.instance.GameLoad();
                slotText[i].text = DataManager.instance.nowPlayer.name + "\n" + DataManager.instance.nowPlayer.lastPlayTime;
            }
            else
            {
                slotText[i].text = "비어있음";
            }
        }
        DataManager.instance.DataClear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Slot(int number)
    {   DataManager.instance.nowSlot = number;

        // 1 저장된 데이터가 있을때 => 불러오기 해서 게임씬으로 넘어감
        if (savefile[number])
        {
            DataManager.instance.GameLoad();
            GameStart();
        }
        // 2 저장된 데이터가 없을때 => 새로운 데이터 저장
        else
        {
            Create();  
        }
    }   
    public void OpenSaveWindow()
    {
        saveWindow.gameObject.SetActive(true);
    }

    public void CloseSaveWindow()
    {
        saveWindow.gameObject.SetActive(false);
    }

    public void Create()
    {
        creat.gameObject.SetActive(true);
    }

    public void GameStart()
    {
        if (!savefile[DataManager.instance.nowSlot])
        { 
            DataManager.instance.nowPlayer.name = newPlayerName.text;
            DataManager.instance.nowPlayer.lastPlayTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DataManager.instance.GameSave();
        }
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
