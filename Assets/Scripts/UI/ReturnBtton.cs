using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnBtton : MonoBehaviour
{
    public int returnSceneId = 0;

    public void ReturnTo()
    {

        if((StoryManager.instance && !StoryManager.instance.isStoryPanelOpen) && (!PropertyManager.instance  || !PropertyManager.instance.isOpenedPanel && !EventManager.instance.isEventPanelOpen))
            SceneManager.LoadScene(returnSceneId);
    }
}
