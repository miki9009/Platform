using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenLabel : MonoBehaviour
{
    public GameObject failed;
    public GameObject completed;
    void OnEnable()
    {
        if (GameManager.State == GameManager.GameState.Idle)
        {
            DoIdle();
        }
        else if(GameManager.State == GameManager.GameState.Completed)
        {
            DoCompleted();
        }
        else
        {
            DoFailed();
        }
    }

    void DisableAll()
    {
        failed.SetActive(false);
        completed.SetActive(false);
    }

    void DoIdle()
    {

    }

    void DoCompleted()
    {
        completed.SetActive(true);
    }

    void DoFailed()
    {
        failed.SetActive(true);
    }
}
