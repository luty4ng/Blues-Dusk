using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum Dice_SuitType
{
    SWORD,
    GRAIL,
    STARCOIN,
    WAND,
    SPECIAL
}

public class UI_DiceSystem : UIFormChildBase
{
    [Header("Basic Elements")]
    //在背包里的骰子
    private List<UI_Dice> _negativeDices = new List<UI_Dice>();

    //被选中了的格子 进入投掷状态后将这些骰子的父物体设为this，并投出他们即可
    private List<UI_Dice> _activedDices = new List<UI_Dice>();

    private List<Transform> _negativeDiceSlots = new List<Transform>();

    [SerializeField]
    private List<Transform> _activedDiceSlots = new List<Transform>();

    [SerializeField]
    private UI_Dice _dicePrefab;

    [SerializeField]
    private UnityEngine.UI.Image _diceSlotPrefab;

    [Space]
    [SerializeField]
    private Transform _gridLayoutByTwo;

    [SerializeField]
    private Transform _gridLayoutByOne;

    [Space]
    [Header("ChildComponents")]
    [SerializeField]
    private UI_DiceStartButton _startButton;

    [SerializeField]
    private CanvasGroup _selectPanel;

    /*[SerializeField]
    private RectTransform _diceSheet;*/

    [Space]
    [Header("Temp Data")]
    [SerializeField]
    private List<UI_DiceData_SO> _tempDiceList = new List<UI_DiceData_SO>();

    [SerializeField]
    private List<RectTransform> _diceSheets = new List<RectTransform>();

    public Dice_Result ResultSum { get; private set; } = null;
    private Dictionary<Dice_SuitType, RectTransform> _usedSheets = new Dictionary<Dice_SuitType, RectTransform>();

    public void OnInit()
    {
        _startButton.OnInit(OnStartButtonClicked);
        CreateDicesFromInventory();
        ResultSum = new Dice_Result();
    }

    private void Start()
    {
        OnInit();
    }

    public void OnDiceClicked(UI_Dice dice)
    {
        if (_negativeDices.Contains(dice))
        {
            if (_activedDices.Count == _activedDiceSlots.Count) return;
            DiceSelected(dice);
        }
        else
        {
            DiceUnSelected(dice);
        }

        if (_activedDices.Count > 0)
            _startButton.Enable();
        else
            _startButton.Disable();
    }

    private void DiceSelected(UI_Dice dice)
    {
        _negativeDices.Remove(dice);
        _activedDices.Add(dice);
        //替换材质，更改父物体
        dice.transform.SetParent(FindEmptyDiceSlot());
        dice.ChangeToDiceUIMaterial();

        //移动到目标格子
        dice.DOComplete();
        dice.transform.DOMove(dice.transform.parent.position, 0.5f);
    }

    private void DiceUnSelected(UI_Dice dice)
    {
        _negativeDices.Add(dice);
        _activedDices.Remove(dice);

        dice.transform.SetParent(_negativeDiceSlots[dice.Index]);

        //回到原本的位置
        dice.DOComplete();
        dice.transform.DOMove(_negativeDiceSlots[dice.Index].position, 0.5f)
            .OnComplete(() => { dice.ChangeToDiceMaskMaterial(); });
    }

    private void OnStartButtonClicked()
    {
        ResetActivedDiceParent();
        ChangeStateToRolling();
    }

    private Transform FindEmptyDiceSlot()
    {
        foreach (Transform slot in _activedDiceSlots)
        {
            if (slot.childCount == 0) return slot;
        }

        return null;
    }

    private void CreateDicesFromInventory()
    {
        Transform targetGrid = null;
        Transform parent = null;
        UI_Dice dice = null;
        //生成骰子
        for (int i = 0; i < _tempDiceList.Count; i++)
        {
            //按照2121进行排列
            if ((i + 1) % 3 == 0)
                targetGrid = _gridLayoutByOne;
            else
                targetGrid = _gridLayoutByTwo;


            parent = Instantiate(_diceSlotPrefab, targetGrid).transform;
            dice = Instantiate(_dicePrefab, parent).OnInit(_tempDiceList[i], i, OnDiceClicked);

            _negativeDices.Add(dice);
            _negativeDiceSlots.Add(parent);
        }
    }

    private void ResetActivedDiceParent()
    {
        foreach (UI_Dice dice in _activedDices)
        {
            dice.transform.SetParent(transform);
        }
    }

    private void ChangeStateToRolling()
    {
        //移动整体UI布局 取消SelectedPanel
        Roll();

        for (int i = 0; i < _negativeDices.Count; i++)
        {
            /*var dice = _negativeDices[i];
            _negativeDices.Remove(dice);*/
            Destroy(_negativeDices[i].gameObject);
            _negativeDices.RemoveAt(i);
            i--;
        }
    }

    private void Roll()
    {
        foreach (UI_Dice dice in _activedDices)
            dice.Roll();

        StartCoroutine("Rolling");
    }

    public bool CheckIfFinishRolling()
    {
        foreach (UI_Dice dice in _activedDices)
        {
            if (!dice.Stopped) return false;
        }

        return true;
    }

    //骰子归位
    public void ResetDicePosition()
    {
        foreach (UI_Dice dice in _activedDices)
        {
            ProvideSheet(dice.Result);
            dice.ResetTransform(_usedSheets[dice.Result]);
        }
    }

    //TODO 需要增加一个优先队列，按顺序触发效果即可
    public void GetSumResult()
    {
        foreach (UI_Dice dice in _activedDices)
            ResultSum.Push(dice.GetResult());
        
    }

    //暂时代替状态机与update
    private IEnumerator Rolling()
    {
        while (!CheckIfFinishRolling())
            yield return 0;
        
        GetSumResult();
        ResetDicePosition();
        
        ResultSum.EffectsProcess();
    }


    //TODO 修改以契合新的数据结构
    private void ProvideSheet(Dice_SuitType type)
    {
        if (_usedSheets.ContainsKey(type))
            return;
        else
        {
            if (_diceSheets.Count == 0)
                Debug.LogError("Lack Of Sheets");
            _usedSheets.Add(type, _diceSheets[0]);
            _diceSheets.RemoveAt(0);
        }
    }

    //TODO 结果的存储与输出
}

public class Dice_Result
{
    public Dictionary<Dice_SuitType, int> sum { get; private set; } = null;

    //通过对这个列表进行操作，达到终止后续效果处理的效果
    public List<UI_DiceFaceBase_SO> results;

    private bool breakOut = false;
    
    public Dice_Result()
    {
        sum = new Dictionary<Dice_SuitType, int>()
        { { Dice_SuitType.SWORD, 0 },
          { Dice_SuitType.GRAIL, 0 },
          { Dice_SuitType.STARCOIN, 0 },
          { Dice_SuitType.WAND, 0 } };
        results = new List<UI_DiceFaceBase_SO>();
    }

    public void Push(UI_DiceFaceBase_SO face)
    {
        results.Add(face);
        
        //给优先级排序 此处后续可优化
        results.Sort((x, y) => 
            { return x.Priority.CompareTo(y.Priority);});
    }
    
    //目前这种做法，如果有相同优先级的效果，似乎会按照选择时的顺序触发
    public void EffectsProcess()
    {
        for (int i = 0; i < results.Count; i++)
        {
            results[i].Effect(this);
            if (breakOut)
            {
                breakOut = false;
                break;
            }
        }
        
        Debug.Log(this.ToString());
    }

    public void BreakOut() => breakOut = true;

    public int Get(Dice_SuitType type) => sum[type] > 0 ? sum[type] : 0;

    public void Add(Dice_SuitType type, int amount) => sum[type] += amount;
    
    public void Set(Dice_SuitType type, int amount) => sum[type] = amount;

    public override string ToString()
    {
        return "Sword: " + sum[Dice_SuitType.SWORD].ToString() + "\n"
               + "Grail: " + sum[Dice_SuitType.GRAIL].ToString() + "\n"
               + "Starcoin: " + sum[Dice_SuitType.STARCOIN].ToString() + "\n"
               + "Wand: " + sum[Dice_SuitType.WAND].ToString() + "\n";
    }
}