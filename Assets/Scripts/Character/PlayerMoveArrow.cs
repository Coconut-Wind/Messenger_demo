using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveArrow : MonoBehaviour
{
    private Player player; 
    private Vector2Int to; //所指向的目标位置

    public void init(Player player, Vector2Int to)
    {
        this.player = player;
        this.to = to;
    }

    private void OnMouseUp() 
    {
        this.player.WalkTo(to);
    }
}
