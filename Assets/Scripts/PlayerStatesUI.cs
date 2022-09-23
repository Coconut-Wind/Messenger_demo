using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatesUI : MonoBehaviour
{
    public Player player; // 拿到Player组件

    [Header("---- HealthBar ----")]
    private Image healthBarSlider;
    private GameObject playerHealthBar;

    // TODO：这是左上角的玩家状态栏控制脚本，在状态栏除了血量还可以显示别的
    // [Header("---- Other Player States Bar ----")]

    private void Awake()
    {
        playerHealthBar = transform.GetChild(0).gameObject; // 拿到HealthBar物体
        healthBarSlider = playerHealthBar.transform.GetChild(0).GetComponent<Image>(); // 拿到Image后通过fillAmount修改血量百分比
    }

    private void Update()
    {
        SetHealthBar();
    }

    public void SetHealthBar()
    {
        // 实时血条变化
        float slideRate = (float)player.currentHealth / player.maxHealth;
        healthBarSlider.fillAmount = slideRate;
    }
}
