using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    private int targetLevel = 1;
    private string targetLevelPath;
    public TextMesh num;
    
    public void SetTargetLevel(int level, string path)
    {
        targetLevel = level;
        targetLevelPath = path;
        num.text = "" + level;
    }

    private void OnMouseUp() 
    {
        if (Input.GetMouseButtonUp(0))
        {
            JumpToLevel();
        }
    }

    private void JumpToLevel()
    {
        LevelManager.currentLevelId = targetLevel;
        LevelManager.currentLevelPath = targetLevelPath;
        SceneManager.LoadScene(1);
        
    }
}
