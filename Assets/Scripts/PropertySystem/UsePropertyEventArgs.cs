using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> OnUseProperty事件的事件参数 </summary>
public class UsePropertyEventArgs
{
    public int propertyID; // 道具ID，一定要传的参数
    public List<Enemy> enemies; // 考虑到技能可能对多个敌人造成影响，所以使用列表

    public UsePropertyEventArgs(int _propertyID)
    {
        propertyID = _propertyID;
    }

    public UsePropertyEventArgs(int _propertyID, List<Enemy> _enemies)
    {
        propertyID = _propertyID;
        enemies = _enemies;
    }

}
