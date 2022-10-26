using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButton : MonoBehaviour
{
    private int targetLevel = 1;
    private string targetLevelPath;
    // public TextMesh num;
    public TextMeshProUGUI levelNum;
    
    public void SetTargetLevel(int level, string path)
    {
        targetLevel = level;
        targetLevelPath = path;
        levelNum.text = "" + level;
    }

    public void OnMouseUp() 
    {
        if (Input.GetMouseButtonUp(0) && !StoryManager.instance.isStoryPanelOpen)
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
