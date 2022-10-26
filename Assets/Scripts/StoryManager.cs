using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryManager : MonoBehaviour
{
    public bool enableStory = false;
    public static StoryManager instance;
    public GameObject storyPanel;
    public TextMeshProUGUI storyLineText;
    public bool isStoryPanelOpen = false;

    [TextArea(1, 3)] public List<string> storyLineList;
    [SerializeField] private int currentLineIndex;

    [SerializeField] private bool isScrolling;
    [SerializeField] private float scrollSpeed;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        storyLineText.text = storyLineList[currentLineIndex];
    }

    private void Update()
    {
        if (storyPanel.activeInHierarchy)
        {
            if (Input.GetMouseButtonUp(0) && !isScrolling)
            {
                currentLineIndex++;
                if (currentLineIndex < storyLineList.Count)
                {
                    StartCoroutine(ScrollingText());
                }
                else
                {
                    AudioPlayer.instance.Play("bgm", "bgm");
                    isStoryPanelOpen = false;
                    storyPanel.SetActive(false);
                }
            }
            else if (Input.GetMouseButtonUp(0) && isScrolling)
            {
                StopAllCoroutines();
                storyLineText.text = storyLineList[currentLineIndex];
                isScrolling = false;
            }
        }
    }

    public void OpenStoryPanel()
    {
        if (enableStory)
        {
            AudioPlayer.instance.Play("StoryBGM", "bgm");
            isStoryPanelOpen = true;
            storyPanel.SetActive(true);
        }
    }

    private IEnumerator ScrollingText()
    {
        isScrolling = true;
        storyLineText.text = "";
        foreach (var word in storyLineList[currentLineIndex])
        {
            storyLineText.text += word;
            yield return new WaitForSeconds(scrollSpeed);
        }
        isScrolling = false;
    }
}
