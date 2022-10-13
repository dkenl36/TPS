using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPaused;

    private void Start()
    {
        isPaused = false;

        // 마우스 고정
        Cursor.lockState = CursorLockMode.Locked;
        // 커서 숨기기
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update ()
    {
        if(Input.GetKeyDown("escape"))
        {
            OnPauseClick();

            if(!isPaused)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
	}

    public void OnPauseClick()
    {
        isPaused = !isPaused;

        Time.timeScale = (isPaused) ? 0.0f : 1.0f;

        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        var scripts = playerObj.GetComponents<MonoBehaviour>();

        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }
    }

    public void OnResumeClick()
    {
        isPaused = !isPaused;

        Time.timeScale = 1.0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        var scripts = playerObj.GetComponents<MonoBehaviour>();

        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("Start");
    }
}
