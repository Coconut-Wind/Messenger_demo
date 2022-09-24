using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public GameObject healthBarPoint;
    private Image healthBarSlider;
    private GameObject spawnedHealthBar;
    private GameObject canvas;
    private Enemy enemy;

    private void Awake()
    {
        canvas = UIManager.instance.GetEnemyHealthBar();
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        spawnedHealthBar = Instantiate(healthBarPrefab, healthBarPoint.transform.position, Quaternion.identity);
        spawnedHealthBar.transform.SetParent(canvas.transform);
        spawnedHealthBar.name = $"{transform.name} HealthBar";
        healthBarSlider = spawnedHealthBar.transform.GetChild(0).GetComponent<Image>();
        
        enemy.SetHealthBar(spawnedHealthBar);
    }

    private void Update()
    {
        ChangeHealthBar();
    }

    public void ChangeHealthBar()
    {
        // Position
        spawnedHealthBar.transform.position = healthBarPoint.transform.position;

        // 实时血条变化
        float slideRate = (float)enemy.currentHealth / enemy.maxHealth;
        healthBarSlider.fillAmount = slideRate;

    }

}
