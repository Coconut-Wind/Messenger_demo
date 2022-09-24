using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatesUI : MonoBehaviour
{
    private Player player = null; // 拿到Player组件

    [Header("---- HealthBar ----")]
    private Image healthBarSlider;
    private GameObject playerHealthBar;
    private TextMeshProUGUI letterNumText;

    // TODO：这是左上角的玩家状态栏控制脚本，在状态栏除了血量还可以显示别的
    // [Header("---- Other Player States Bar ----")]

    private void Awake()
    {
        playerHealthBar = transform.GetChild(0).gameObject; // 拿到HealthBar物体
        healthBarSlider = playerHealthBar.transform.GetChild(0).GetComponent<Image>(); // 拿到Image后通过fillAmount修改血量百分比
        letterNumText = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void SetPlayer(Player p)
    {
        player = p;
    }

    private void Update()
    {
        ChangeHealthBar();
    }

    public void ChangeHealthBar()
    {
        // 实时血条变化
        if (!player)
            return;
        float slideRate = (float)player.currentHealth / player.maxHealth;
        healthBarSlider.fillAmount = slideRate;
    }

    public void SetLetterNumber(int n)
    {
        letterNumText.text = "x" + n;
    }
}
