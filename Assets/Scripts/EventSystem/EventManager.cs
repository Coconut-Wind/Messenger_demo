using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    public List<Event> eventList;
    public List<Event> posiEventList;
    public List<Event> negaEventList;
    public float eventTriggerProbability; // 事件触发概率

    [Header("UI相关")]
    // 事件面板以及子物体
    public GameObject eventPanel; // 事件面板
    public TextMeshProUGUI eventDescription; // 事件描述
    public GameObject optionsPanel; // 选项面板
    public GameObject optionButtonPrefab; // 动态生成的选项的预制体
    public List<GameObject> optionButtonList; // 动态生成的选项的列表
    // 选择选项后的面板以及子物体
    public GameObject afterOptionPanel; // 选择选项后的面板
    public TextMeshProUGUI optionDescription; // 选择选项后的描述

    public Event currentEvent; // 当前的事件
    public GameObject currentClickOption; // 当前被点击的选项

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);


        InitPosiAndNegaEventList();
    }

    private void Start()
    {
        // // Test
        // currentEvent = eventList[0];
        // InitEventPanel(eventList[0]);
        // OpenEventPanel();
    }

    // <summary> 玩家在普通点位触发事件时调用 </summary>
    public void EventHappen()
    {
        RandomChooseEvent();
        InitEventPanel(eventList[0]);
        OpenEventPanel();
    }

    private void InitPosiAndNegaEventList()
    {
        foreach (var _event in eventList)
        {
            if (_event.eventType == "positive")
            {
                posiEventList.Add(_event);
            }
            else if (_event.eventType == "negative")
            {
                negaEventList.Add(_event);
            }
        }
    }

    // <summary> 随机选择一个事件 </summary>
    public void RandomChooseEvent()
    {
        int rand = Random.Range(0, eventList.Count);
        currentEvent = eventList[rand];
    }

    // <summary> 随机选择一个正面事件 </summary>
    public void RandomChoosePosiEvent()
    {
        int rand = Random.Range(0, posiEventList.Count);
        currentEvent = posiEventList[rand];
    }

    // <summary> 随机选择一个负面事件 </summary>
    public void RandomChooseNegaEvent()
    {
        int rand = Random.Range(0, negaEventList.Count);
        currentEvent = negaEventList[rand];
    }

    // ----------------- 为选项添加效果 ------------------
    //（如获得道具、加血、生成敌人、无事发生就是没有效果）

    // 随机获得道具
    public void GetPropertyRandom()
    {
        int rand = Random.Range(0, PropertyManager.instance.propertyList.Count);

        PropertyManager.instance.GenerateProperty(rand);
    }

    // 获得固定的道具
    public void GetPropertyByPropertyID(int _id)
    {
        PropertyManager.instance.GenerateProperty(_id);
    }

    // 获得道具需要以另一个道具为前提
    public void GetPropertyByNeedProperty(int _id)
    {
        foreach (var property in PropertyManager.instance.playerPropertyList)
        {
            if (property.propertyID == _id) // 如果这个道具玩家拥有，那么就可以获得一个随机道具
            {
                int rand = Random.Range(0, PropertyManager.instance.propertyList.Count);
                PropertyManager.instance.GenerateProperty(rand);
                break;
            }
        }
    }

    // 当前生命值上升或者下降 _num可以是负数，即下降
    public void CurrentHealthUpOrDown(int _num)
    {
        GameManager.instance.player.currentHealth += _num;
    }

    // TODO：生成敌人
    public void GenerateEnemies()
    {

    }


    // ----------------- UI相关 -------------------

    // <summary> 初始化Event面板 </summary>
    public void InitEventPanel(Event _event)
    {
        eventDescription.text = _event.eventDescription;
        GenerateOptionsButton(_event);
    }

    public void InitAfterOptionPanel()
    {
        // 找到当前被点击的事件选项
        EventOption eventOption = currentEvent.eventOptionList[currentClickOption.transform.GetSiblingIndex()];

        // 从事件选项可能发生的情况中选择一种
        int rand = Random.Range(0, eventOption.optionEffectList.Count);

        // 触发选项效果
        eventOption.optionEffectList[rand].effectAction.Invoke();

        // 初始化选项描述
        string descriptionText = eventOption.optionEffectList[rand].optionEffectDescription;
        ref List<Property> propertyList = ref PropertyManager.instance.playerPropertyList;
        descriptionText = descriptionText.Replace("[道具]", propertyList[propertyList.Count-1].propertyName);
        optionDescription.text = descriptionText;
    }

    // 动态生成选项
    public void GenerateOptionsButton(Event _event)
    {
        for (int i = 0; i < _event.optionNum; i++)
        {
            var optionButton = Instantiate(optionButtonPrefab, transform.position, Quaternion.identity);
            optionButton.transform.SetParent(optionsPanel.transform);
            optionButton.transform.localScale = new Vector3(1,1,1);
            optionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _event.eventOptionList[i].optionName;
            optionButtonList.Add(optionButton);

            optionButton.GetComponent<Button>().onClick.AddListener(CloseEventPanel);
            optionButton.GetComponent<Button>().onClick.AddListener(InitAfterOptionPanel);
            optionButton.GetComponent<Button>().onClick.AddListener(OpenAfterOptionPanel);
        }
    }

    public void OpenEventPanel()
    {
        eventPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseEventPanel()
    {
        eventPanel.SetActive(false);
        Time.timeScale = 1;

        // 关闭面板时销毁生成的按钮
        foreach (var optionButton in optionButtonList)
        {
            Destroy(optionButton);
        }
    }

    public void OpenAfterOptionPanel()
    {
        afterOptionPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseAfterOptionPanel()
    {
        afterOptionPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
