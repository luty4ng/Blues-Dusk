using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameKit.Event;
using GameKit;
using UnityEngine;
using UnityGameKit.Runtime;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class GameSettings : MonoSingletonBase<GameSettings>
{
    private const string GamePrefix = "GameSettings";
    private const string ScenePrefix = "SceneSettings";

    private void Start()
    {
        GameKitCenter.Event.Subscribe(LoadSettingsEventArgs.EventId, OnLoad);
    }

    public void FirstTimeLoadSettings()
    {
        Log.Success("First Time Load Settings");
        GameKitCenter.Setting.SetBool("GameSettings", true);
        string[] configs = GameKitCenter.Data.GameConfigTable.Data.ToString().RemoveBrackets().RemoveEmptySpaceLine().RemoveLast().Split(',');
        for (int i = 0; i < configs.Length; i++)
        {
            string[] pair = configs[i].Split(':');
            string configName = pair[0];
            string configValue = pair[1];
            GameKitCenter.Setting.SetString(string.Format("{0}.{1}", GamePrefix, configName), configValue);
        }

        foreach (var config in GameKitCenter.Data.SceneConfigTable.DataList)
        {
            string configName = config.SceneAssetName.Correction();
            string configValue = config.IsUnlocked.Correction();
            Log.Warning(string.Format("{0}.{1}", ScenePrefix, configName) + " >> " + configValue);
            GameKitCenter.Setting.SetString(string.Format("{0}.{1}", ScenePrefix, configName), configValue);
        }
    }

    public void OnLoad(object sender, GameEventArgs e)
    {
        bool hasGameSettings = GameKitCenter.Setting.GetBool("GameSettings", false);
        // 如果之前没有存档，从默认配置中生成存档
        if (!hasGameSettings)
        {
            FirstTimeLoadSettings();
        }
    }

    public bool GetSceneState(string sceneName)
    {
        return GameKitCenter.Setting.GetBool(string.Format("{0}.{1}", ScenePrefix, sceneName));
    }

    public void SetSceneState(string sceneName, bool value)
    {
        GameKitCenter.Setting.SetBool(string.Format("{0}.{1}", ScenePrefix, sceneName), value);
    }

    public bool GetBool(string settingName)
    {
        // Log.Warning(string.Format("Get Bool {0}.{1}", GamePrefix, settingName) + " >> " + GameKitCenter.Setting.GetBool(string.Format("{0}.{1}", GamePrefix, settingName)));
        return GameKitCenter.Setting.GetBool(string.Format("{0}.{1}", GamePrefix, settingName));
    }
    public int GetInt(string settingName)
    {
        return GameKitCenter.Setting.GetInt(string.Format("{0}.{1}", GamePrefix, settingName));
    }
    public float GetFloat(string settingName)
    {
        return GameKitCenter.Setting.GetFloat(string.Format("{0}.{1}", GamePrefix, settingName));
    }
    public string GetString(string settingName)
    {
        return GameKitCenter.Setting.GetString(string.Format("{0}.{1}", GamePrefix, settingName));
    }

    public void SetBool(string settingName, bool value)
    {
        // Log.Warning(string.Format("Set Bool {0}.{1}", GamePrefix, settingName) + " >> " + value);
        GameKitCenter.Setting.SetBool(string.Format("{0}.{1}", GamePrefix, settingName), value);
    }
    public void SetInt(string settingName, int value)
    {
        GameKitCenter.Setting.SetInt(string.Format("{0}.{1}", GamePrefix, settingName), value);
    }
    public void SetFloat(string settingName, float value)
    {
        GameKitCenter.Setting.SetFloat(string.Format("{0}.{1}", GamePrefix, settingName), value);
    }
    public void SetString(string settingName, string value)
    {
        GameKitCenter.Setting.SetString(string.Format("{0}.{1}", GamePrefix, settingName), value);
    }
#if UNITY_EDITOR
    [Button("保存场景元素配置(Editor Only)")]
    public static void EditorSaveElementConfig([ValueDropdown("GetDay"), LabelText("天数")] int day = 1, [ValueDropdown("GetStage"), LabelText("时间")] int stage = 1)
    {
        string configName = string.Format("D{0}_{1}_{2}", day, stage, SceneManager.GetSceneAt(1).name.Replace("_", "-"));
        GameElementBase[] elements = GameObject.FindObjectsOfType<GameElementBase>();
        StageConfig_SO config = ScriptableObject.CreateInstance<StageConfig_SO>();

        config.SetDayAndStage(day, stage, SceneManager.GetSceneAt(1).name);
        for (int i = 0; i < elements.Length; i++)
        {
            ElementData data = new ElementData();
            data.Name = elements[i].name;
            data.ElementType = elements[i].GetType().ToString();
            data.Position = elements[i].transform.position;
            data.Rotation = elements[i].transform.rotation.eulerAngles;
            data.DestinationPosition = elements[i].InteractPosition;

            data.onInteractBeginEvent = elements[i].OnInteractBegin;
            data.onInteractAfterEvent = elements[i].OnInteractAfter;
            
            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(elements[i].gameObject))
            {
                if (elements[i].GetType() == typeof(NPCElement) || elements[i].GetType() == typeof(CollectElement))
                {
                    Debug.Log(elements[i].gameObject.name);
                    GameObject prefabAsset = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(elements[i].gameObject);
                    data.Prefab = prefabAsset.GetComponent<GameElementBase>();
                }
            }
            if (elements[i].GetType() == typeof(NPCElement))
            {
                data.NPC_Dialog = ((NPCElement)elements[i]).Dialog;
                data.NPC_Posture = ((NPCElement)elements[i]).Posture;
            }
            else if (elements[i].GetType() == typeof(CollectElement))
            {
                data.Item_Dialog = ((CollectElement)elements[i]).Dialog;
            }
            else if (elements[i].GetType() == typeof(DoorElement))
            {
                data.Door_CanPass = ((DoorElement)elements[i]).CanPass;
                data.Door_CanPassCondition = ((DoorElement)elements[i]).CanPassCondition;
                data.Door_TargetScene = ((DoorElement)elements[i]).TargetScene;
            }
            else if (elements[i].GetType() == typeof(CustomElement))
            {
                data.Custom_Dialog = ((CustomElement)elements[i]).Dialog;
                data.Custom_CanRepeatDialog = ((CustomElement)elements[i]).CanRepeatDialog;
                data.Custom_HasDialoged = ((CustomElement)elements[i]).HasDialoged;
            }
            config.AddData(data);
        }

        if (!Directory.Exists(AssetUtility.ElementConfigPath + "/Configs/"))
            Directory.CreateDirectory(AssetUtility.ElementConfigPath + "/Configs/");
        string fullPath = AssetUtility.ElementConfigPath + "/Configs/" + configName + ".asset";
        if (File.Exists(fullPath))
            UnityEditor.AssetDatabase.DeleteAsset(fullPath);
        UnityEditor.AssetDatabase.CreateAsset(config, fullPath);
        UnityEditor.AssetDatabase.Refresh();
    }

    [Button("加载场景元素配置(Editor Only)")]
    public static void EditorLoadElementConfig(StageConfig_SO config)
    {
        Transform elementParent = GameObject.Find("Dynamic").transform;
        GameElementBase[] elements = GameObject.FindObjectsOfType<GameElementBase>();
        List<GameObject> waitForDestroy = new List<GameObject>();
        for (int j = 0; j < elementParent.childCount; j++)
        {
            waitForDestroy.Add(elementParent.GetChild(j).gameObject);
        }

        foreach (var elementConfig in config.GetAll())
        {
            if (elementConfig.Prefab == null)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].Name == elementConfig.Name && elements[i].GetType().ToString() == elementConfig.ElementType)
                    {
                        // Debug.Log(elements[i].Name);
                        ConfigToElemnt(elements[i], elementConfig);
                    }
                }
            }
            else
            {
                GameElementBase prefabInstance = (GameElementBase)UnityEditor.PrefabUtility.InstantiatePrefab(elementConfig.Prefab, elementParent);
                prefabInstance.transform.position = elementConfig.Position;
                prefabInstance.transform.rotation = elementConfig.Rotation.ToQuaternion();
                ConfigToElemnt(prefabInstance, elementConfig);
            }
        }

        for (int j = 0; j < waitForDestroy.Count; j++)
        {
            DestroyImmediate(waitForDestroy[j]);
        }
    }

    [Button("清空场景动态元素(Editor Only)")]
    public void EditorClearAllDynamicElement()
    {
        Transform elementParent = GameObject.Find("Dynamic").transform;
        List<GameObject> waitForDestroy = new List<GameObject>();
        for (int j = 0; j < elementParent.childCount; j++)
        {
            waitForDestroy.Add(elementParent.GetChild(j).gameObject);
        }

        for (int j = 0; j < waitForDestroy.Count; j++)
        {
            DestroyImmediate(waitForDestroy[j]);
        }
    }

#endif

    public void LoadElementConfig(int day, int stage, string sceneName)
    {
        string configName = string.Format("{0}-{1}-{2}", day, stage, sceneName);
        StageConfig_SO elementCongfig = GameKitCenter.Data.GetDataSO<StageConfigPool_SO>().GetData<StageConfig_SO>(configName);
        if (elementCongfig == null)
            return;

        Transform elementParent = GameObject.Find("Dynamic").transform;
        for (int j = 0; j < elementParent.childCount; j++)
        {
            Destroy(elementParent.GetChild(j).gameObject);
        }

        GameElementBase[] elements = GameObject.FindObjectsOfType<GameElementBase>();
        foreach (var elementConfig in elementCongfig.GetAll())
        {
            if (elementConfig.Prefab == null)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].Name == elementConfig.Name && elements[i].GetType().ToString() == elementConfig.ElementType)
                    {
                        ConfigToElemnt(elements[i], elementConfig);
                    }
                }
            }
            else
            {
                GameElementBase Instance = GameObject.Instantiate<GameElementBase>(elementConfig.Prefab, elementConfig.Position, elementConfig.Rotation.ToQuaternion(), elementParent);
                ConfigToElemnt(Instance, elementConfig);
            }
        }
    }

    //Info 传入一个Element，从配置数据中同步信息
    private static void ConfigToElemnt(GameElementBase elementBase, ElementData elementData)
    {
        //bug 编译错误
        elementBase.InteractTrans.position = elementData.DestinationPosition;

        elementBase.OnInteractBegin = elementData.onInteractBeginEvent;
        elementBase.OnInteractAfter = elementData.onInteractAfterEvent;
        
        if (elementBase.GetType() == typeof(NPCElement))
        {
            ((NPCElement)elementBase).Dialog = elementData.NPC_Dialog;
            ((NPCElement)elementBase).Posture = elementData.NPC_Posture;
        }
        else if (elementBase.GetType() == typeof(CollectElement))
        {
            ((CollectElement)elementBase).Dialog = elementData.Item_Dialog;
        }
        else if (elementBase.GetType() == typeof(DoorElement))
        {
            ((DoorElement)elementBase).CanPass = elementData.Door_CanPass;
            ((DoorElement)elementBase).CanPassCondition = elementData.Door_CanPassCondition;
            ((DoorElement)elementBase).TargetScene = elementData.Door_TargetScene;
        }
        else if (elementBase.GetType() == typeof(CustomElement))
        {
            ((CustomElement)elementBase).Dialog = elementData.Custom_Dialog;
            ((CustomElement)elementBase).CanRepeatDialog = elementData.Custom_CanRepeatDialog;
            ((CustomElement)elementBase).HasDialoged = elementData.Custom_HasDialoged;
        }
    }

    public static IEnumerable GetDay()
    {
        for (int i = 1; i <= 30; i++)
        {
            yield return i;
        }
    }

    public static IEnumerable GetStage()
    {
        for (int i = 1; i <= 4; i++)
        {
            yield return i;
        }
    }
}