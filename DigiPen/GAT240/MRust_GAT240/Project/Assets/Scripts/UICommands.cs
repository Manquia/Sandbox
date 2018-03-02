using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UICommands : MonoBehaviour
{
    public string StartupLevelName;
    public string CreditsLevelName;
    public string MenuLevelName;

#region UnityEvent

    void Start()
    {
        // Always make the mouse visible on (UICommands should be in a menu level)
        CaptureMouse(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInGamePanelUI();
        }
    }
    #endregion

    public void ToggleInGamePanelUI()
    {
        var panel = transform.Find("Panel");

        if(panel != null)
        {
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
        }
    }

    public void LoadLevel1(){ SceneManager.LoadScene("HW1"); }
    public void LoadLevel2(){ SceneManager.LoadScene("HW2"); }
    public void LoadLevel3(){ SceneManager.LoadScene("HW3"); }
    public void LoadLevel4(){ SceneManager.LoadScene("HW4"); }
    public void LoadLevel5(){ SceneManager.LoadScene("HW5"); }
    public void LoadLevel6(){ SceneManager.LoadScene("HW6"); }
    public void LoadLevel7(){ SceneManager.LoadScene("HW7"); }
    
    
    public void LoadCredits()
    {
        SceneManager.LoadScene(CreditsLevelName);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(MenuLevelName);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CaptureMouse(bool capture)
    {
        if (capture)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }



}
