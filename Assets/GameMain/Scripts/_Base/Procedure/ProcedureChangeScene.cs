using System.Transactions;
using UnityEngine;
using System.Collections;
using GameKit.Event;
using GameKit;
using GameKit.Element;
using UnityGameKit.Runtime;
using ProcedureOwner = GameKit.Fsm.IFsm<GameKit.Procedure.IProcedureManager>;


public class ProcedureChangeScene : ProcedureBase
{
    private const int MenuSceneId = 1;
    private bool m_ChangeToMenu = false;
    private bool m_IsChangeSceneComplete = false;
    private int m_BackgroundMusicId = 0;
    private Prototyper m_Prototyper;
    private bool m_IsScenePreloaded;
    private ProcedureOwner m_CachedOwner;

    public override bool UseNativeDialog
    {
        get
        {
            return false;
        }
    }

    protected override void OnInit(ProcedureOwner procedureOwner)
    {
        base.OnInit(procedureOwner);
        m_CachedOwner = procedureOwner;
    }

    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);
        m_IsChangeSceneComplete = false;
        // QuickCinemachineCamera.Clear();
        GameKitCenter.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
        GameKitCenter.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);

        // 停止所有声音
        // GameKitCenter.Sound.StopAllLoadingSounds();
        // GameKitCenter.Sound.StopAllLoadedSounds();

        // 隐藏所有实体
        GameKitCenter.Entity.HideAllLoadingEntities();
        GameKitCenter.Entity.HideAllLoadedEntities();

        // 卸载所有场景
        // string[] loadedSceneAssetNames = GameKitCenter.Scene.GetLoadedSceneAssetNames();
        // for (int i = 0; i < loadedSceneAssetNames.Length; i++)
        // {
        //     GameKitCenter.Scene.UnloadScene(loadedSceneAssetNames[i]);
        // }

        // 还原游戏速度
        GameKitCenter.Core.ResetNormalGameSpeed();

        string sceneName = procedureOwner.GetData<VarString>(ProcedureStateUtility.NEXT_SCENE_NAME);
        m_IsScenePreloaded = procedureOwner.GetData<VarBoolean>(ProcedureStateUtility.IS_SCENE_PRELOADED);
        if (!m_IsScenePreloaded)
        {
            GameKitCenter.Element.SaveAll();
            if (GameKitCenter.Scheduler.MultiScene)
                GameKitCenter.Scheduler.DoTransition(AssetUtility.GetSceneAsset(sceneName));
            else
                GameKitCenter.Scheduler.LoadSceneAsyn(AssetUtility.GetSceneAsset(sceneName), onSuccess: OnSceneLoad);
        }
        else
        {
            procedureOwner.SetData<VarBoolean>(ProcedureStateUtility.IS_SCENE_PRELOADED, false);
            OnSceneLoad();
        }
        // m_BackgroundMusicId = drScene.BackgroundMusicId;
        if (m_IsChangeSceneComplete)
        {

        }
    }

    protected override void OnExit(ProcedureOwner procedureOwner, bool isShutdown)
    {
        GameKitCenter.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
        GameKitCenter.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
        base.OnExit(procedureOwner, isShutdown);
    }

    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (!m_IsChangeSceneComplete)
        {
            return;
        }

        if (m_ChangeToMenu)
        {
            ChangeState<ProcedureMenu>(procedureOwner);
        }
        else
        {
            ChangeState<ProcedureMain>(procedureOwner);
        }
    }

    public Transform GetEnterTransform()
    {
        DoorElement element = (DoorElement)GameKitCenter.Element.GetElement(GameKitCenter.Procedure.CachedDoorName);
        if (element == null)
            return null;
        return element.EnterTranform;
    }

    public Transform GetDefaultTransform()
    {
        return GameObject.Find("DefaultSpawnPoint").transform;
    }

    private void OnSceneLoad()
    {
        GameKitCenter.Element.Clear();
        GameKitCenter.Element.LoadAll();
        Transform targetTrans = GetEnterTransform();
        if (targetTrans == null)
            targetTrans = GetDefaultTransform();
        AddressableManager.instance.GetAssetAsyn(AssetUtility.GetElementAsset("Prototyper"), (GameObject obj) =>
        {
            GameObject realObj = GameObject.Instantiate(obj);
            m_Prototyper = realObj.GetComponent<Prototyper>();
            m_Prototyper.transform.SetParent(GameKitCenter.Procedure.DynamicParent);
            m_Prototyper.SetTransform(targetTrans);
            // Debug.Log(targetTrans.position);
            // Debug.Log(m_Prototyper.transform.position);
            // QuickCinemachineCamera.current.SetFollowPostion(m_Prototyper.transform.position);
            QuickCinemachineCamera.current.SetFollowTarget(m_Prototyper.transform);
            OnSceneLoadEnd();
        });
    }

    private void OnSceneLoadEnd()
    {
        m_IsChangeSceneComplete = true;
    }

    private void OnLoadSceneSuccess(object sender, GameEventArgs e)
    {
        LoadSceneSuccessEventArgs args = (LoadSceneSuccessEventArgs)e;
        if (args.UserData == null)
        {
            Log.Warning("DoTransitionCompleteEventArgs is null, procedure phase the load without transition.");
            OnSceneLoad();
            return;
        }

        DoTransitionCompleteEventArgs transitionArgs = (DoTransitionCompleteEventArgs)args.UserData;
        if (transitionArgs.UserData == null)
        {
            Log.Warning("The transition is not fired by {0} when loading {1}", typeof(SchedulerComponent).Name, args.SceneAssetName);
            return;
        }

        if (transitionArgs.UserData.GetType() == typeof(SchedulerComponent))
        {
            if (transitionArgs.TargetCount == 0)
            {
                OnSceneLoad();
            }
        }

        // if (m_BackgroundMusicId > 0)
        // {
        //     GameKitCenter.Sound.PlayMusic(m_BackgroundMusicId);
        // }
        // m_IsChangeSceneComplete = true;
    }

    private void OnLoadSceneFailure(object sender, GameEventArgs e)
    {
        LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
        if (ne.UserData != this)
        {
            return;
        }

        Log.Error("Load scene '{0}' failure, error message '{1}'.", ne.SceneAssetName, ne.ErrorMessage);
    }


    private void OnUnloadSceneSuccess(object sender, GameEventArgs e)
    {
        UnloadSceneSuccessEventArgs ne = (UnloadSceneSuccessEventArgs)e;
        // if (ne.UserData != this)
        // {
        //     return;
        // }

        Log.Success("Load Scene '{0}' By Event.", ne.SceneAssetName);


        // if (m_BackgroundMusicId > 0)
        // {
        //     GameKitCenter.Sound.PlayMusic(m_BackgroundMusicId);
        // }
        // m_IsChangeSceneComplete = true;
    }

    private void OnUnloadSceneFailure(object sender, GameEventArgs e)
    {
        UnloadSceneFailureEventArgs ne = (UnloadSceneFailureEventArgs)e;
        if (ne.UserData != this)
        {
            return;
        }

        Log.Error("Load scene '{0}' failure, error message '{1}'.", ne.SceneAssetName, ne.SceneAssetName);
    }
}

