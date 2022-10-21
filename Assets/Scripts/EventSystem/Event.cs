using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Event/eventData")]
public class Event : ScriptableObject
{
    public int eventID;
    public string eventType; // positive、negative
    public string eventDescription; // 事件的描述
    public int optionNum; // 选项的数量
    public List<EventOption> eventOptionList; // 事件的选项个数，EventOption中包含了选项名和选项描述

    private void Awake()
    {
        optionNum = eventOptionList.Count;
    }
}
