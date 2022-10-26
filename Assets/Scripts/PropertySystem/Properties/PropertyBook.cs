using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyBook : Property
{
    public int duration = 4;
    private int startedTurn = 0;
    private bool isTriggered = false;
    protected override void PropertyAbility(Player _player, UsePropertyEventArgs _args)
    {
        if(_args.propertyID == propertyID)
        {
            if (!isTriggered)
            {
                foreach(Enemy e in _args.enemies){
                    e.maxZoomDistance += 2;
                    e.maxChaseDistance += 2;
                }
                _player.isUsingBook = true;
                isTriggered = true;
                startedTurn = GameManager.instance.turnCount;
            }
            else
            {
                if (GameManager.instance.turnCount - startedTurn == duration)
                {
                    Debug.Log("Effect off");
                    foreach(Enemy e in _args.enemies){
                        e.maxZoomDistance -= 2;
                        e.maxChaseDistance -= 2;
                    }
                    //_player.isUsingBook = false;
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
