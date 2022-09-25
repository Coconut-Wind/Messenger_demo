using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static int currentLevelId = 1;
    public static string currentLevelPath = "Assets\\Data\\level_1.txt";

    public int ButtonPadding = 2;

    public GameObject levelButton;

    public static List<string> levelFileNames;

    private void Awake() {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        GetLevels();
        CreateLevelButtons();

        Debug.Log(GameManager.instance == null);
        if (GameManager.instance != null)
        {
            UIManager.instance.gameObject.SetActive(false);
        }
    }

    public static void SetCurrentLevel(int i) 
    {
        if (i > levelFileNames.Count)
            return;
        currentLevelId = i;
        currentLevelPath = levelFileNames[i-1];
    }

    private void GetLevels()
    {
        levelFileNames = new List<string>();
        string path = string.Format("{0}", System.Environment.CurrentDirectory + "\\Assets\\Data");
        Debug.Log(path);
        if (Directory.Exists(path))
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] file = dir.GetFiles();
            foreach (FileInfo f in file)
            {
                if (f.FullName.EndsWith(".txt") &&f.Name.StartsWith("level_"))
                {
                    string name =  "Assets\\Data\\"+f.Name;
                    Debug.Log(name);

                    levelFileNames.Add(name);
                }
            } 
        }
    }

    private void CreateLevelButtons()
    {
        for (int i = 0; i < levelFileNames.Count; i++)
        {
            string path = levelFileNames[i];
            GameObject btn = Instantiate(levelButton, 
                new Vector3(transform.position.x + i * ButtonPadding, transform.position.y, 1f), 
                Quaternion.identity);
            LevelButton lb = btn.GetComponent<LevelButton>();
            lb.SetTargetLevel(i + 1, path);

            btn.transform.SetParent(transform);
        }
    }

    public void ReturnTitleScene()
    {
        SceneManager.LoadScene(0);
    }
}
