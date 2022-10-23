using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyHpPotions : Property
{
    protected override void PropertyAbility(Player _player, UsePropertyEventArgs _args)
    {
        if(_args.propertyID == propertyID)
        {
            _player.SetCurrentHealth(_player.currentHealth+1);
        }
    }
}
