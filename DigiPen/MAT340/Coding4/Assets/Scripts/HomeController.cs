using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HomeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void LoadP1(){ SceneManager.LoadScene("P1");}
    public void LoadP2(){ SceneManager.LoadScene("P2");}
    public void LoadP3(){ SceneManager.LoadScene("P3");}
    public void LoadP4(){ SceneManager.LoadScene("P4");}

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
