using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStart : MonoBehaviour
{


    public void LoadScene1()
    {
        SceneManager.LoadScene("__Prospector_Scene_0");
    }

    public void LoadScene2()
    {
        SceneManager.LoadScene("Poker");
    }
}
