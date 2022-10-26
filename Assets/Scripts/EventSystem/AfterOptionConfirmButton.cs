using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterOptionConfirmButton : MonoBehaviour
{
    public void NeedPropertyFailReturn()
    {
        if (EventManager.isOptionReturn)
        {
            EventManager.isOptionReturn = false;
            EventManager.instance.InitEventPanel(EventManager.instance.currentEvent);
            EventManager.instance.OpenEventPanel();
        }
    }
}
