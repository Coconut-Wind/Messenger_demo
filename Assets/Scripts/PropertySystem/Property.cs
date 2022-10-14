using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary> 道具类，所有道具继承自该类 </summary>
public class Property : MonoBehaviour
{
    [Header("道具信息，道具将设定为预制体，以下信息需提前设定好")]
    public int propertyID; // 道具的唯一标识
    public Image propertyImage; // 道具图标
    public string propertyName; // 道具名
    public string propertyDescription; // 道具描述
    public bool isPassive; // 是否为被动道具，false为主动
    public bool isOneOff; // 是否为一次性道具

    [Header("道具参数")]
    public bool isPlayerGet; // 玩家是否获得

    // 该GameObject启动时，即玩家获取到了该道具，此时添加事件订阅
    private void OnEnable()
    {
        GameManager.instance.player.OnUseProperty += PropertyAbility;

        isPlayerGet = true;
        PropertyManager.instance.AddPlayerOwnedPropertyList(this);
    }

    // 该GameObject关闭时，即玩家获取到了该道具，此时取消事件订阅
    private void OnDisable()
    {
        GameManager.instance.player.OnUseProperty -= PropertyAbility;

        isPlayerGet = false;
        PropertyManager.instance.RemovePlayerOwnedPropertyList(this);
    }

    /// <summary> 要实现道具的功能在子类实现该方法 </summary>
    protected virtual void PropertyAbility(Player _player, UsePropertyEventArgs _args) { }

    public void OpenPropertyDetailPanel()
    {
        PropertyManager.instance.currentClickProperty = gameObject;

        // 根据道具类型选择打开的面板
        GameObject panel = isPassive? PropertyManager.instance.passivePropertyDetailPanel : PropertyManager.instance.activePropertyDetailPanel;
        
        panel.SetActive(true);

        Image image = panel.transform.GetChild(0).GetComponent<Image>();
        image.sprite = propertyImage.sprite;

        TextMeshProUGUI propertyNameText =  panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        propertyNameText.text = propertyName;

        TextMeshProUGUI propertyDescriptionText =  panel.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        propertyDescriptionText.text = propertyDescription;
    }

}
