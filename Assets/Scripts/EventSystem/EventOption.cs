using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 事件选项类，用于记录该事件下有几个选项
[System.Serializable]
public class EventOption
{
    public string optionName; // 选项名：打开、拒绝、接受、休息、交谈、购买
    public List<EventOptionEffect> optionEffectList; // 选择该选项后出现的一种或多种情况：比如什么也没有发生、获得道具

}

// 事件选项效果类，用于记录选择该选项后的描述和触发的效果
[System.Serializable]
public class EventOptionEffect
{
    public string optionEffectDescription;
    // public int optionEffectID; // 这个选项有什么效果 0：什么都不发生；1：获得道具；2：恢复或降低某些数值（生命值之类）3：生成敌人
    public UnityEvent effectAction;
}
