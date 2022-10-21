using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public void OnClick()
    {
        EventManager.instance.currentClickOption = this.gameObject;

    }
}
