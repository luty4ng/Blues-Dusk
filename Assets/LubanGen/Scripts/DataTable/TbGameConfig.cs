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



namespace LubanConfig.DataTable
{

public sealed partial class TbGameConfig
{

     private readonly DataTable.GameConfig _data;

    public TbGameConfig(JSONNode _json)
    {
        if(!_json.IsArray)
        {
            throw new SerializationException();
        }
        if (_json.Count != 1) throw new SerializationException("table mode=one, but size != 1");
        _data = DataTable.GameConfig.DeserializeGameConfig(_json[0]);
        PostInit();
    }

     public bool IsDavidDead
     {
        get
        {
            return _data.IsDavidDead;
        }
     } 
     public bool IsRebellionStart
     {
        get
        {
            return _data.IsRebellionStart;
        }
     } 
     public int CurrentDay
     {
        get
        {
            return _data.CurrentDay;
        }
     } 
     public int CurrentWeekday
     {
        get
        {
            return _data.CurrentWeekday;
        }
     } 
     public int CurrentWeek
     {
        get
        {
            return _data.CurrentWeek;
        }
     } 

    public void Resolve(Dictionary<string, object> _tables)
    {
        _data.Resolve(_tables);
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        _data.TranslateText(translator);
    }

    
    partial void PostInit();
    partial void PostResolve();
}

}