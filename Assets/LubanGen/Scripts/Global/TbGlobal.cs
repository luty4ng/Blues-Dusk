//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace LubanConfig.Global
{

public sealed partial class TbGlobal
{
    private readonly Dictionary<int, Global.GlobalVariable> _dataMap;
    private readonly List<Global.GlobalVariable> _dataList;
    
    public TbGlobal(JSONNode _json)
    {
        _dataMap = new Dictionary<int, Global.GlobalVariable>();
        _dataList = new List<Global.GlobalVariable>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = Global.GlobalVariable.DeserializeGlobalVariable(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.GuildOpenLevel, _v);
        }
        PostInit();
    }

    public Dictionary<int, Global.GlobalVariable> DataMap => _dataMap;
    public List<Global.GlobalVariable> DataList => _dataList;

    public Global.GlobalVariable GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public Global.GlobalVariable Get(int key) => _dataMap[key];
    public Global.GlobalVariable this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    
    partial void PostInit();
    partial void PostResolve();
}

}