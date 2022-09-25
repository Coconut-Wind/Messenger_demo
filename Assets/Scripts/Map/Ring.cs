using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public float alpha = 0;
    public float speed = 0.0001f;
    private bool isShining = false;
    private bool isIncreasing = false;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (isShining)
        {
            if (isIncreasing)
            {
                alpha += speed;
                if (alpha >= 1)
                {
                    isIncreasing = false;
                }  
            }
            else
            {
                alpha -= speed;
                if (alpha <= 0)
                {
                    isIncreasing = true;
                }   
            }
            
            
            spriteRenderer.color = new Color(spriteRenderer.color.r, 
                                            spriteRenderer.color.g, 
                                            spriteRenderer.color.b, alpha);
            //Debug.Log(spriteRenderer.color);
        }

        //游戏结束时不再闪烁
        if (GameManager.instance.IsGameOver())
        {
            isShining = false;
            this.gameObject.SetActive(false);
        }
    }

    public void SetShining(bool shining)
    {
        isShining = shining;
        isIncreasing = shining;
        alpha = 0;
    }
}
