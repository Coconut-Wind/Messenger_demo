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
    public TextMeshProUGUI playerHealthText; // 血条中的血量提示
    private TextMeshProUGUI letterNumText;
    public TextMeshProUGUI playerAttackDMGText;
    public TextMeshProUGUI playerMoveTimesText;

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
        ChangePlayerStatesText();
    }

    public void ChangeHealthBar()
    {
        // 实时血条变化
        if (!player)
            return;
        float slideRate = (float)player.currentHealth / player.maxHealth;
        healthBarSlider.fillAmount = slideRate;
    }

    public void ChangePlayerStatesText()
    {
        // 实时玩家属性变化
        if(!player)
            return;
        playerHealthText.text = string.Format("{0} / {1}", player.currentHealth, player.maxHealth);
        playerAttackDMGText.text = string.Format("攻击伤害：{0}", player.attackDMG);
        playerMoveTimesText.text = string.Format("行动次数：{0} / {1}", player.moveTime-1, player.moveableTimes);
    }

    public void SetLetterNumber(int n)
    {
        letterNumText.text = "x" + n;
    }
}
