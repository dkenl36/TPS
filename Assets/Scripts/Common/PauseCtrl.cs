using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCtrl : MonoBehaviour {

    private GameObject pauseMenu;
    public GameManager gameManager;

	// Use this for initialization
	void Start ()
    {
        pauseMenu = transform.Find("PauseMenu").gameObject;
        pauseMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(gameManager.isPaused == true)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
	}
}
