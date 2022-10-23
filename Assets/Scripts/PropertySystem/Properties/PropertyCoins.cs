using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyCoins: Property
{
    protected override void PropertyAbility(Player _player, UsePropertyEventArgs _args)
    {
        if(_args.propertyID == propertyID)
        {
            //_player.moveableTimes = 2;
        }
    }
}