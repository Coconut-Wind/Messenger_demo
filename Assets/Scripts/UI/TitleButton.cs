using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{

    public void OnStartPressed()
    {
        SceneManager.LoadScene(2);
    }

    public void OnExitPressed()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
