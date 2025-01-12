using System;
using System.Collections.Generic;
using GameKit.DataNode;

namespace GameKit.Dialog
{
    public sealed class DialogDataNodeVariable : DataNodeVariableBase
    {
        public string Speaker;
        public string Contents;
        public string MoodName;
        public bool IsFunctional = false;
        public bool IsLocalDivider = false;
        public bool IsLocalCompleter = false;
        public bool IsGlobalDivider = false;
        public bool IsGlobalCompleter = false;
        public bool IsConditionalBranch = false;
        public bool IsDiceCheckBranch = false;
        public bool IsDiceCheckOption = false;
        public bool IsDiceDefaultOption = false;
        public bool IsInventoryCheckOption = false;
        public string CachedInventoryName;
        public List<string> CachedStockConditions;
        public List<string> DividerConditions;
        public List<string> CompleteConditons;
        public List<string> GlobalCompleteConditons;
        public List<string> GlobalDividerConditions;
        public Dictionary<string, int> DiceConditions;
        public DialogNodeCallback m_OnEnter, m_OnUpdate, m_OnExit;

        public DialogDataNodeVariable()
        {
            this.Speaker = "<Default>";
            this.Contents = "<Default>";
            this.MoodName = "<Default>";
            this.CachedInventoryName = "<Default>";
            this.DividerConditions = new List<string>();
            this.CompleteConditons = new List<string>();
            this.GlobalCompleteConditons = new List<string>();
            this.GlobalDividerConditions = new List<string>();
            this.CachedStockConditions = new List<string>();
            this.DiceConditions = new Dictionary<string, int>();
        }

        public override Type Type
        {
            get
            {
                return typeof(DialogDataNodeVariable);
            }
        }

        public static DialogDataNodeVariable Create(string speaker, string contents, string moodName = "Default")
        {
            DialogDataNodeVariable dialogVariable = ReferencePool.Acquire<DialogDataNodeVariable>();
            dialogVariable.Speaker = speaker;
            dialogVariable.Contents = contents;
            dialogVariable.MoodName = moodName;
            return dialogVariable;
        }

        public void OnEnter()
        {
            m_OnEnter?.Invoke();
        }

        public void OnUpdate()
        {
            m_OnUpdate?.Invoke();
        }
        public void OnExit()
        {
            m_OnExit?.Invoke();
        }

        public override void Clear()
        {
            m_OnEnter = (DialogNodeCallback)System.Delegate.RemoveAll(m_OnEnter, m_OnEnter);
            m_OnUpdate = (DialogNodeCallback)System.Delegate.RemoveAll(m_OnUpdate, m_OnUpdate);
            m_OnExit = (DialogNodeCallback)System.Delegate.RemoveAll(m_OnExit, m_OnExit);
            DiceConditions.Clear();
            CompleteConditons.Clear();
            DividerConditions.Clear();
            GlobalDividerConditions.Clear();
            GlobalCompleteConditons.Clear();
            CachedStockConditions.Clear();
            IsFunctional = false;
            IsLocalDivider = false;
            IsLocalCompleter = false;
            IsDiceCheckBranch = false;
            IsConditionalBranch = false;
            IsDiceCheckOption = false;
            IsDiceDefaultOption = false;
            IsGlobalDivider = false;
            IsGlobalCompleter = false;
            IsInventoryCheckOption = false;
            Speaker = "<Default>";
            Contents = "<Default>";
            MoodName = "<Default>";
        }

        public static Dictionary<string, int> GetDefaultDiceConditions()
        {
            return new Dictionary<string, int>()
            {
                {"SWORD",0},
                {"GRAIL",0},
                {"STARCOIN",0},
                {"WAND",0}
            };
        }

        public override string ToString()
        {
            return Speaker + ":" + Contents;
        }
    }
}
