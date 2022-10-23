using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyAssassinHood: Property
{
    protected override void PropertyAbility(Player _player, UsePropertyEventArgs _args)
    {
        if(_args.propertyID == propertyID)
        {
            foreach(Enemy e in _args.enemies){
                e.maxZoomDistance -= 2;
            }
        }
    }
}