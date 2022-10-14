using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 道具系统管理类 </summary>
public class PropertyManager : MonoBehaviour
{
    public static PropertyManager instance;
    [Header("有关道具")]
    [SerializeField] private List<Property> propertyList; // 所有道具的列表
    [SerializeField] private List<GameObject> propertyPrefabsList; // 按照ID顺序，用于生成道具图标即道具本身
    [SerializeField] private List<Property> PlayerPropertyList;

    [Header("有关UI")]
    public Canvas propertyCanvas;
    public GameObject propertyPanel;
    public GameObject passivePropertyDetailPanel; // 被动道具详情面板
    public GameObject activePropertyDetailPanel; // 主动道具详情面板
    public GameObject currentClickProperty; // 当前点击的道具图标

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GenerateProperty(0);
        }
    }

    public void AddPlayerOwnedPropertyList(Property _property)
    {
        PlayerPropertyList.Add(_property);
    }

    public void RemovePlayerOwnedPropertyList(Property _property)
    {
        PlayerPropertyList.Remove(_property); ;
    }

    public List<Property> GetPlayerOwnedPropertyList()
    {
        return PlayerPropertyList;
    }

    // ---------------------------------------------

    /// <summary> 获得道具时应调用该方法，根据道具ID生成道具图标即道具本身</summary>
    public void GenerateProperty(int _id)
    {
        var spawnedProperty = Instantiate(propertyPrefabsList[_id], transform.position, Quaternion.identity);
        spawnedProperty.transform.SetParent(propertyPanel.transform);
    }

    public void ClosePropertyDetailPanel(GameObject _panel)
    {
        _panel.SetActive(false);
    }

    public void OnClickUsePropertyButton()
    {
        Property currentProperty = currentClickProperty.GetComponent<Property>();
        GameManager.instance.player.UseProperty(new UsePropertyEventArgs(currentProperty.propertyID));
    }

}
