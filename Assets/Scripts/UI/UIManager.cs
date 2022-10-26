using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 全局UI管理
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject enemyHealthBarHolder;
    public GameObject playerStateHolder;
    public GameObject enemyStateHolder;
    //public GameObject itemsHolder;
    public GameObject gameoverCanvas;
    public GameObject propertyCanvas;
    public DragCamera cameraController;
    public TextMeshProUGUI gameoverTitle;
    public TextMeshProUGUI topBar_text;
    public GameObject topBar_skipButton;


    [Header("面板弹出弹出效果")]
    [SerializeField] private float popSpeed;
    [SerializeField] private AnimationCurve popShowCurve;
    [SerializeField] private AnimationCurve popHideCurve;

    private void Awake()
    {
        if(instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public GameObject GetEnemyHealthBar()
    {
        return enemyHealthBarHolder;
    }

    //清除所有敌人血条
    public void ClearAllEnemyHealthBar()
    {
        for (int i = 0; i < enemyHealthBarHolder.transform.childCount; i++)
        {
            Destroy(enemyHealthBarHolder.transform.GetChild(i).gameObject);
        }
    }

    public PlayerStatesUI GetPlayerStateHolder()
    {
        return playerStateHolder.GetComponent<PlayerStatesUI>();
    }

    public EnemyStatesUI GetEnemyStateHolder()
    {
        return enemyStateHolder.GetComponent<EnemyStatesUI>();
    }

    /*public ItemsHolder GetItemsHolder()
    {
        return itemsHolder.GetComponent<ItemsHolder>();
    }*/

    public void SetGameOverTitleText(string title)
    {
        gameoverTitle.text = title;
    }

    public void ShowInfoBars(string type = "all")
    {
        if (type == "all" || type == "player")
            playerStateHolder.SetActive(true);
        if (type == "all" || type == "enemy")
            enemyStateHolder.SetActive(true);
    }

    public void HideInfoBars(string type = "all")
    {
        if (type == "all" || type == "player")
            playerStateHolder.SetActive(false);
        if (type == "all" || type == "enemy")
            enemyStateHolder.SetActive(false);
    }

    public void ShowEnemyInfo(Enemy e)
    {
        if (e != null)
        {
            // enemyStateHolder.SetActive(true);
            PopShowPanel(enemyStateHolder);

            GetEnemyStateHolder().enemy = e;

        }
        else
        {
            GetEnemyStateHolder().enemy = null;
            // enemyStateHolder.SetActive(false);
            PopHidePanel(enemyStateHolder);
        }
    }

    /// ------------------ 面板弹入弹出效果 ------------------
    public void PopShowPanel(GameObject _panel)
    {
        StartCoroutine(PopShowPanelIE(_panel));
    }
    private IEnumerator PopShowPanelIE(GameObject _panel)
    {
        _panel.transform.localScale = Vector3.zero;
        _panel.SetActive(true);
        float timer = 0;
        while(timer <= 1)
        {
            _panel.transform.localScale = Vector3.one * popShowCurve.Evaluate(timer);
            timer += Time.deltaTime * popSpeed;
            yield return null;
        }
        // Time.timeScale = 0;
    }

    public void PopHidePanel(GameObject _panel)
    {
        StartCoroutine(PopHidePanelIE(_panel));
    }
    private IEnumerator PopHidePanelIE(GameObject _panel)
    {
        // Time.timeScale = 1;
        float timer = 0;
        while(timer <= 1)
        {
            _panel.transform.localScale = Vector3.one * popHideCurve.Evaluate(timer);
            timer += Time.deltaTime * popSpeed;
            yield return null;
        }
        _panel.transform.localScale = Vector3.one;
        _panel.SetActive(false);
    }
}
