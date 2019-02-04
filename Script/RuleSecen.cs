using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Text;

public class RuleSecen : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void RuleExit()
    {
        SceneManager.LoadScene("Login", LoadSceneMode.Single);

    }
}
