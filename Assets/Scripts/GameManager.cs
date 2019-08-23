using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private GameObject _items;
    private static GameObject _endInterface;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _items = GameObject.Find("ToEnabled").transform.Find("items").gameObject;
        _endInterface = GameObject.Find("UI").transform.Find("End").gameObject;
    }

    public void StartGame()
    {
        _items.SetActive(true);
    }

    public void GameOver()
    {
        _endInterface.SetActive(true);
        Application.Quit();
    }
}
