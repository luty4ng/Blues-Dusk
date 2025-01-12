using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using GameKit.QuickCode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public Animator diceAnimator;

    //在背包里的骰子
    private List<UI_Dice> _negativeDices = new List<UI_Dice>(); //这些骰子最初是遵循排列规则的

    private List<UI_Dice> _activedDices = new List<UI_Dice>();

    private List<Transform> _negativeDiceSlots = new List<Transform>(); //程序生成

    [SerializeField]
    private List<Transform> _activedDiceSlots = new List<Transform>(); //手动赋值

    [Space]
    [Header("Prefabs")]
    [SerializeField]
    private UI_Dice _dicePrefab;

    [SerializeField]
    private UnityEngine.UI.Image _diceSlotPrefab; //仅仅是用于规范骰子的位置，防止在反复的点选中丢失位置

    [Space]
    [Header("LayoutComponents")]
    public int basicOffset = 70;

    public int lineOffset = 150;

    [SerializeField]
    private ScrollRect _scrollView;

    [SerializeField]
    private RectTransform _content;

    [SerializeField]
    private RectTransform _gridLayoutByTwo;

    [SerializeField]
    private RectTransform _gridLayoutByOne;

    [Space]
    [SerializeField]
    private UI_DiceStartButton _startButton;

    /*[SerializeField]
    private CanvasGroup _selectPanel;*/

    [SerializeField]
    private List<RectTransform> _diceSheets = new List<RectTransform>(); //五种类型，五个栏位，手动赋值

    public Dice_Result Result { get; private set; } = new Dice_Result(); //保存结果的类

    private Dictionary<Dice_SuitType, RectTransform>
        _usedSheets = new Dictionary<Dice_SuitType, RectTransform>(); //相当于池子

    #region KeybordSelecter

    private int _currentDiceIndex = -1;
    private List<UI_Dice> _currentList;

    private UI_Dice _currentDice;

    #endregion

    public void OnInit()
    {
        _startButton.OnInit(OnStartButtonClickedCallback);
        CreateDicesFromInventory();
        SelectDice(0, _negativeDices);

        float height = basicOffset +
                       ((_gridLayoutByTwo.childCount + 1) / 2 + _gridLayoutByOne.childCount) * lineOffset;
        _content.sizeDelta = new Vector2(460, height);
        //Debug.Log(height);

        for (int i = 0; i < _activedDiceSlots.Count; i++)
            _activedDices.Add(null);

        EnablePlayerInput();
    }

    public void EnablePlayerInput()
    {
        StartCoroutine("KeybordInputCheck");
    }

    public void Clear()
    {
        foreach (Dice_SuitType key in _usedSheets.Keys)
        {
            foreach (Transform child in _usedSheets[key].transform)
                Destroy(child.gameObject);
        }

        foreach (Transform diceSlot in _activedDiceSlots)
        {
            if (diceSlot.childCount > 1)
                Destroy(diceSlot.GetChild(1).gameObject);
        }

        _usedSheets.Clear();
        _negativeDices.Clear();
        _negativeDiceSlots.Clear();
        _activedDices.Clear();
        Result.Clear();
        _startButton.Clear();
        _currentDice = null;

        GetComponent<CanvasGroup>().blocksRaycasts = true;
        StopCoroutine("KeybordInputCheck");
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Clear();
    }

    private void CreateDicesFromInventory()
    {
        var inventory = GameKitCenter.Inventory.GetInventory(DiceInventory.current.Name);

        Transform targetGrid = null;
        Transform parent = null;
        UI_Dice dice = null;
        //生成骰子
        /*for (int i = 0; i < _tempDiceList.Count; i++)
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
        }*/
        List<UI_DiceData_SO> datas = new List<UI_DiceData_SO>();
        for (int i = 0; i < inventory.StockMap.Length; i++)
        {
            if (inventory.StockMap[i] == null) continue;
            datas.Add((UI_DiceData_SO)inventory.StockMap[i].Data);
        }

        for (int i = 0; i < datas.Count; i++)
        {
            //按照2121进行排列
            if ((i + 1) % 3 == 0)
                targetGrid = _gridLayoutByOne;
            else
                targetGrid = _gridLayoutByTwo;


            parent = Instantiate(_diceSlotPrefab, targetGrid).transform;
            dice = Instantiate(_dicePrefab, parent).OnInit(datas[i], i, OnConfirmKeyPressed, OnPointerEnterDice);

            _negativeDices.Add(dice);
            _negativeDiceSlots.Add(parent);
        }
    }


    #region InSelectingState

    public void OnDiceClicked(UI_Dice dice)
    {
        if (!_activedDices.Contains(dice))
        {
            DiceSelected(dice);
        }
        else
        {
            DiceUnSelected(dice);
        }

        bool canCheck = false;
        foreach (UI_Dice uiDice in _activedDices)
        {
            if (uiDice != null)
            {
                canCheck = true;
                break;
            }
        }

        if (canCheck)
            _startButton.Enable();
        else
            _startButton.Disable();
    }

    private void OnPointerEnterDice(int index)
    {
        UI_Dice targetDice = _negativeDices[index];
        if (targetDice == _currentDice) return;
        if (_activedDices.Contains(targetDice))
        {
            SelectDice(_activedDices.IndexOf(targetDice), _activedDices);
        }
        else
        {
            SelectDice(index, _negativeDices);
        }
    }

    private void DiceSelected(UI_Dice dice)
    {
        //if (_activedDices.Count == _activedDiceSlots.Count) return;
        //_negativeDices.Remove(dice);
        //_activedDices.Add(dice);
        for (int i = 0; i < _activedDiceSlots.Count; i++)
        {
            //if (_activedDiceSlots[i].childCount == 0)
            if (!_activedDiceSlots[i].GetComponentInChildren<UI_Dice>())
            {
                _activedDices[i] = dice;
                break;
            }
            else if (i == _activedDiceSlots.Count - 1)
            {
                Debug.Log("Filled");
                return;
            }
        }

        //替换材质，更改父物体
        dice.transform.SetParent(FindEmptyDiceSlot());
        dice.ChangeToDiceUIMaterial();

        //移动到目标格子
        dice.DOComplete();
        dice.enabled = false;
        dice.transform.DOMove(dice.transform.parent.position, 0.5f).OnComplete(() => { dice.enabled = true; });
    }

    private void DiceUnSelected(UI_Dice dice)
    {
        //_negativeDices.Add(dice);
        //_activedDices.Remove(dice);
        int index = _activedDices.IndexOf(dice);
        _activedDices[index] = null;

        dice.transform.SetParent(_negativeDiceSlots[dice.Index]);

        //自动卷动UI到指定位置
        float lineIndex = _currentDiceIndex;
        if (_currentDiceIndex % 3 == 1) --lineIndex;
        float normalizedHeight = (float)lineIndex / (float)_negativeDices.Count;
        _scrollView.normalizedPosition = new Vector2(0, 1 - normalizedHeight);

        //回到原本的位置
        dice.DOComplete();
        dice.enabled = false;
        dice.transform.DOMove(dice.transform.parent.position, 0.5f)
            .OnComplete(() =>
            {
                dice.ChangeToDiceMaskMaterial();
                dice.enabled = true;
                if (dice.transform.position != dice.transform.parent.position)
                    dice.transform.DOMove(dice.transform.parent.position, 0.2f);
            });
    }

    private Transform FindEmptyDiceSlot()
    {
        foreach (Transform slot in _activedDiceSlots)
        {
            if (slot.GetComponentInChildren<UI_Dice>() == null) return slot;
        }

        return null;
    }

    #endregion

    #region InRollingState

    private void OnStartButtonClickedCallback()
    {
        //ResetActivedDiceParent();
        RemoveUnSelectedDice();
    }

    private void RemoveUnSelectedDice()
    {
        foreach (Transform child in _gridLayoutByOne.transform)
            Destroy(child.gameObject);
        foreach (Transform child in _gridLayoutByTwo.transform)
            Destroy(child.gameObject);
    }

    //这一项是防止CanvasGroup淡出后，让骰子一起消失，因此先将骰子置于最上层
    private void ResetActivedDiceParent()
    {
        /*foreach (UI_Dice dice in _activedDices)
            dice.transform.SetParent(transform);*/
    }

    public void RollActivedDices()
    {
        //await Task.Delay(500);
        //TODO 这一行完全可以直接在动画中控制
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        _currentDice = null;

        foreach (UI_Dice dice in _activedDices)
        {
            if (dice == null) continue;
            dice.Roll();
        }

        GetComponent<CanvasGroup>().blocksRaycasts = false;
        StopCoroutine("KeybordInputCheck");
    }

    public bool CheckIfFinishRolling()
    {
        foreach (UI_Dice dice in _activedDices)
        {
            if (dice == null) continue;
            if (!dice.Stopped) return false;
        }

        return true;
    }

    public bool CheckIfFinishReseting()
    {
        foreach (UI_Dice dice in _activedDices)
        {
            if (dice == null) continue;
            if (!dice.IsComplete) return false;
        }

        return true;
    }

    #endregion

    #region AfterRolling

    //骰子归位
    public void ResetDicePosition()
    {
        foreach (UI_Dice dice in _activedDices)
        {
            if (dice == null) continue;
            ProvideSheet(dice.Result);
            dice.ResetTransform(_usedSheets[dice.Result]);
        }
    }

    //UpdateInfo 原本用一个移除一个的方法不可行，已修复
    private void ProvideSheet(Dice_SuitType type)
    {
        if (_usedSheets.ContainsKey(type))
            return;
        else
        {
            if (_diceSheets.Count == _usedSheets.Count)
                Debug.LogError("Lack Of Sheets");
            _usedSheets.Add(type, _diceSheets[_usedSheets.Count]);
            //BUG 这里会影响复用
            //_diceSheets.RemoveAt(0);
        }
    }

    //先将各个结果的数据存储起来，以备结算
    public void AddDiceFaceToResultList()
    {
        foreach (UI_Dice dice in _activedDices)
        {
            if (dice == null) continue;
            Result.Push(dice.GetResult());
        }
    }

    public Dice_Result CaculateFinalResult()
    {
        Result.EffectsProcess();
        Debug.Log(Result.ToString());
        return Result;
    }

    #endregion

    #region FsmSimulate

    /*private IEnumerator WaitForFadeIn()
    {
        OnInit();

        //diceSystemAnimator.SetTrigger("Fadein");
        yield return 0;
        
        //while (!diceSystemAnimator.IsComplete())
        {
            yield return 0;
        }
    }*/

    /*private IEnumerator WaitForFadeOut()
    {
        //diceSystemAnimator.SetTrigger("Fadeout");
        yield return 0;
        
        //while (!diceSystemAnimator.IsComplete())
        {
            yield return 0;
        }
        
        yield return Rolling();
    }*/

    //暂时代替状态机与update
    /*private IEnumerator Rolling()
    {
        RollActivedDices();
        while (!CheckIfFinishRolling())
            yield return 0;
        
        AddDiceFaceToResultList();
        ResetDicePosition();
        yield return new WaitForSeconds(0.5f);
        CaculateFinalResult();
    }*/

    #endregion

    #region Keybord Input

    //键盘输入检测的接口
    private IEnumerator KeybordInputCheck()
    {
        while (true)
        {
            if (diceAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != ("An_DiceSystem_On"))
            {
                yield return 0;
                continue;
            }

            if (InputManager.instance.GetKeyDown(KeyCode.LeftShift))
                OnStartKeyPressed();
            if (InputManager.instance.GetKeyUp(KeyCode.LeftShift))
                OnStartKeyReleased();

            if (!InputManager.instance.GetKey(KeyCode.LeftShift))
            {
                if (InputManager.instance.GetKeyDown(KeyCode.W))
                    OnUpKeyPressed();
                if (InputManager.instance.GetKeyDown(KeyCode.S))
                    OnDownKeyPressed();
                if (InputManager.instance.GetKeyDown(KeyCode.A))
                    OnLeftKeyPressed();
                if (InputManager.instance.GetKeyDown(KeyCode.D))
                    OnRightKeyPressed();
                if (InputManager.instance.GetKeyDown(KeyCode.Space))
                    OnConfirmKeyPressed();
            }

            yield return 0;
        }
    }

    //Info CurrentList & CurrentIndex 均在这里改变
    private void SelectDice(int index, List<UI_Dice> list)
    {
        //Debug.Log(index + ", " + list.Count);

        if (_currentDice != null)
            _currentDice.OnDisSelected();

        _currentDice = list[index];
        _currentDice.OnSelected();

        _currentDiceIndex = index;
        _currentList = list;

        if (_currentList == _negativeDices)
        {
            float lineIndex = _currentDiceIndex;
            if (_currentDiceIndex % 3 == 1) --lineIndex;
            float normalizedHeight = (float)lineIndex / (float)_negativeDices.Count;
            _scrollView.normalizedPosition = new Vector2(0, 1 - normalizedHeight);
        }
    }

    //TODO 优化逻辑，重复的代码块较多
    private void OnUpKeyPressed()
    {
        int edge = 0;
        int targetIndex = _currentDiceIndex;
        if (_currentList == _negativeDices)
        {
            if ((_currentDiceIndex + 1) % 3 == 0)
                targetIndex = _currentDiceIndex - 1;
            else //if((_currentDiceIndex + 1) % 3 != 0)
                targetIndex = _currentDiceIndex - (_currentDiceIndex + 1) % 3;

            while (targetIndex >= edge && _negativeDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                --targetIndex;
        }
        else if (_currentList == _activedDices)
        {
            targetIndex = _currentDiceIndex - (_currentDiceIndex + 1) % 3;
            while (targetIndex >= edge && _activedDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                --targetIndex;
        }

        if (targetIndex < edge)
            Debug.Log("Top of list");
        else
            SelectDice(targetIndex, _currentList);
    }

    private void OnDownKeyPressed()
    {
        int edge = 0;
        int targetIndex = _currentDiceIndex;
        if (_currentList == _negativeDices)
        {
            edge = _negativeDiceSlots.Count;

            if ((_currentDiceIndex + 1) % 3 == 0)
                targetIndex = _currentDiceIndex + 1;
            else //if((_currentDiceIndex + 1) % 3 != 0)
                targetIndex = _currentDiceIndex + (3 - (_currentDiceIndex + 1) % 3);

            while (targetIndex < _negativeDiceSlots.Count &&
                   _negativeDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                ++targetIndex;
        }
        else if (_currentList == _activedDices)
        {
            edge = _activedDiceSlots.Count;

            targetIndex = _currentDiceIndex + (3 - (_currentDiceIndex + 0) % 3);
            while (targetIndex < _activedDiceSlots.Count &&
                   _activedDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                ++targetIndex;
        }

        if (targetIndex >= _currentList.Count)
            Debug.Log("Buttom of list");
        else
            SelectDice(targetIndex, _currentList);
    }

    private void OnRightKeyPressed()
    {
        int edge = 0;
        int targetIndex = _currentDiceIndex;
        var targetList = _currentList;

        if (_currentList == _negativeDices)
        {
            if ((_currentDiceIndex + 1) % 3 == 1)
            {
                targetIndex = _currentDiceIndex + 1;
                if (targetIndex >= _currentList.Count ||
                    _negativeDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                {
                    /*targetIndex = 0;
                    targetList = _activedDices;*/
                    SwitchList();
                    return;
                }
            }
            else
            {
                /*targetIndex = 0;
                targetList = _activedDices;*/
                SwitchList();
                return;
            }

            edge = _negativeDices.Count;
        }
        else
        {
            edge = _activedDices.Count;
            if ((_currentDiceIndex + 1) % 3 == 0)
            {
                targetIndex = _currentDiceIndex;
                targetList = _currentList;
            }
            else
            {
                targetIndex = _currentDiceIndex + 1;
            }

            while (targetIndex < edge && _activedDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
            {
                ++targetIndex;
            }
        }

        if (targetList.Count <= 0 || targetIndex >= edge)
            Debug.Log("Nothing in TargetList");
        else
            SelectDice(targetIndex, targetList);
    }

    private void OnLeftKeyPressed()
    {
        int edge = 0;
        int targetIndex = _currentDiceIndex;
        var targetList = _currentList;
        if (_currentList == _negativeDices)
        {
            edge = 0;
            if ((_currentDiceIndex + 1) % 3 == 2)
            {
                targetIndex = _currentDiceIndex - 1;
                if (_negativeDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                    targetIndex = _currentDiceIndex;
            }
            else { }
        }
        else
        {
            if ((_currentDiceIndex + 1) % 3 == 1)
            {
                /*targetIndex = 0;
                targetList = _negativeDices;*/
                SwitchList();
                return;
            }
            else
                targetIndex = _currentDiceIndex - 1;

            while (_activedDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
            {
                if ((targetIndex + 1) % 3 == 1)
                {
                    targetIndex = 0;
                    targetList = _negativeDices;
                    break;
                }

                --targetIndex;
            }

            edge = targetIndex - 1;
        }

        if (targetList.Count <= 0 || targetIndex < edge)
            Debug.Log("Nothing in TargetList");
        else
            SelectDice(targetIndex, targetList);
    }

    private void OnConfirmKeyPressed(UI_Dice dice = null)
    {
        if (_currentList == null) return;

        var currentDice = _currentList[_currentDiceIndex];
        if (currentDice != null)
            OnDiceClicked(currentDice);

        //currentList不可能等于0
        if (CheckIfSomeListEmpty())
            SwitchList(currentDice);
        else
        {
            int targetIndex = _currentDiceIndex;
            if (_currentList == _negativeDices)
            {
                while (targetIndex < _currentList.Count &&
                       _negativeDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                    ++targetIndex;

                if (targetIndex >= _currentList.Count)
                {
                    targetIndex = _currentDiceIndex - 1;
                    while (targetIndex >= 0 &&
                           _negativeDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                        --targetIndex;
                }
            }
            else
            {
                //Bug activedDice并不是按照视觉上的顺序排列的，因此会出错
                //Key 最直接的解决方案或许是将视觉与逻辑同步，需要修改的将是DiceSelected()方法
                while (targetIndex < _currentList.Count &&
                       _activedDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                    ++targetIndex;

                if (targetIndex >= _currentList.Count)
                {
                    targetIndex = _currentDiceIndex - 1;
                    while (targetIndex >= 0 && _activedDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                        --targetIndex;
                }
            }

            SelectDice(targetIndex, _currentList);
        }
    }

    private void OnStartKeyPressed() => _startButton.OnButtonPressed();
    private void OnStartKeyReleased() => _startButton.OnButtonReleased();

    private bool CheckIfSomeListEmpty()
    {
        if (_currentList == _activedDices)
        {
            foreach (Transform slot in _activedDiceSlots)
                if (slot.GetComponentInChildren<UI_Dice>() != null)
                    return false;
        }
        else
        {
            foreach (Transform slot in _negativeDiceSlots)
                if (slot.GetComponentInChildren<UI_Dice>() != null)
                    return false;
        }

        return true;
    }

    private void SwitchList(UI_Dice currentDice = null)
    {
        int targetIndex = 0;
        if (_currentList == _activedDices)
        {
            if (currentDice == null)
            {
                while (targetIndex < _negativeDiceSlots.Count &&
                       _negativeDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                    ++targetIndex;
                if (targetIndex >= _negativeDiceSlots.Count) return;
            }
            else
                targetIndex = _negativeDices.IndexOf(currentDice);

            SelectDice(targetIndex, _negativeDices);
        }
        else
        {
            while (targetIndex < _activedDiceSlots.Count &&
                   _activedDiceSlots[targetIndex].GetComponentInChildren<UI_Dice>() == null)
                ++targetIndex;
            if (targetIndex >= _activedDiceSlots.Count) return;
            SelectDice(targetIndex, _activedDices);
        }
    }

    #endregion

    public void AddStartButtonCallback(UnityAction callback)
    {
        _startButton.AddCallBack(callback);
    }
}

//用于存储与输出结果，同时承担了结算的作用
public class Dice_Result
{
    //TODO 需要最终传输给对话系统，可以考虑做成静态的
    public Dictionary<Dice_SuitType, int> sum { get; private set; } = null;

    private Dictionary<string, int> m_SerializableSum = null;
    private List<Dice_SuitType> m_CachedAttributeTypes;

    public Dictionary<string, int> SerializableSum
    {
        get
        {
            if (m_SerializableSum == null)
                m_SerializableSum = new Dictionary<string, int>();

            if (m_SerializableSum.Count == 0)
            {
                foreach (var diceResult in sum)
                {
                    m_SerializableSum.Add(System.Enum.GetName(typeof(Dice_SuitType), diceResult.Key), diceResult.Value);
                }
            }

            return m_SerializableSum;
        }
    }

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
        m_CachedAttributeTypes = new List<Dice_SuitType>()
        { Dice_SuitType.SWORD, Dice_SuitType.GRAIL, Dice_SuitType.STARCOIN, Dice_SuitType.WAND };
    }

    public void Push(UI_DiceFaceBase_SO face)
    {
        results.Add(face);

        //给优先级排序 此处后续可优化
        results.Sort((x, y) => { return x.Priority.CompareTo(y.Priority); });
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

        // Debug.Log(this.ToString());
        results.Clear(); //防止重复调用时结果出错
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

    public void Clear()
    {
        for (int i = 0; i < m_CachedAttributeTypes.Count; i++)
        {
            sum[m_CachedAttributeTypes[i]] = 0;
        }

        if (m_SerializableSum != null)
            m_SerializableSum.Clear();
    }
}