using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardColor : MonoBehaviour {


    public static BoardColor Instance { set; get; }

    public GameObject stepPrefab;
    //public GameObject stepPrefab2;

    private List<GameObject> steps;

    public Character[,] Characters { set; get; }


    private void Start()
    {
        Instance = this;
        steps = new List<GameObject>();
    }

    private GameObject GetStepObject()
    {
        GameObject go = steps.Find(g => !g.activeSelf);

        if (go == null)
        {
          
                go = Instantiate(stepPrefab);
                steps.Add(go);
         
        }

        return go;
    }

    public void StepAllowedMoves(bool[,] moves)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (moves[i, j])
                {
                    GameObject go = GetStepObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
                }
            }
        }
    }

    public void HideSteps()
    {
        foreach (GameObject go in steps)
            go.SetActive(false);
    }
}
