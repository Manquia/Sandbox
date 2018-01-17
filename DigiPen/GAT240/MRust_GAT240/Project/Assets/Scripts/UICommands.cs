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

    public void LoadStartupLevel()
    {
        SceneManager.LoadScene(StartupLevelName);
    }

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
    



}
