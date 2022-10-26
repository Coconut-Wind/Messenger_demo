using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStatesUI : MonoBehaviour
{
    [HideInInspector] public Enemy enemy = null;
    
    public Image healthBarSlider;
    public Image enemyIco;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI attackDMGText;
    public TextMeshProUGUI zoomDistanceText;
    public TextMeshProUGUI chaseDistanceText;
    public TextMeshProUGUI watchDistanceText;

    private void Awake()
    {

    }


    private void Update()
    {
        if (enemy)
            UpdateInfo();
    }

    private void UpdateInfo()
    {
        // 血条更新
        float slideRate = (float)enemy.currentHealth / enemy.maxHealth;
        healthBarSlider.fillAmount = slideRate;
        healthText.text = string.Format("{0} / {1}", enemy.currentHealth, enemy.maxHealth);

        // 更新敌人信息
        enemyIco.sprite = enemy.GetComponent<SpriteRenderer>().sprite;
        nameText.text = enemy.enemyName;
        attackDMGText.text = string.Format("攻击伤害：{0}", enemy.attackDMG);
        zoomDistanceText.text = string.Format("巡逻距离：{0}", enemy.maxZoomDistance);
        chaseDistanceText.text = string.Format("追逐距离：{0}", enemy.maxChaseDistance);
        watchDistanceText.text = string.Format("监视距离：{0}", enemy.maxWatchingDistance);
        // dataText.text = "health:" + enemy.GetCurrentHealth() + "/" + enemy.maxHealth + "\n" +
        //                 "zoom size:" + enemy.maxZoomDistance + "\n" +
        //                 "chase distance:" + enemy.maxChaseDistance + "\n" +
        //                 "watch diatance:" + enemy.maxWatchingDistance;
    }
}
