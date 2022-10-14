using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 测试用，无实际作用
public class TEST_PropertyMaxHealthUp : Property
{
    protected override void PropertyAbility(Player _player, UsePropertyEventArgs _args)
    {
        // base.PropertyAbility(_player, _args);
        if(_args.propertyID == propertyID)
        {
            // _player.maxHealth += _player.maxHealth * 0.3f; 无法将类型“float”隐式转换为“int”。存在一个显式转换(是否缺少强制转换?)
            _player.maxHealth += _player.maxHealth * 1;
        }
    }
}
