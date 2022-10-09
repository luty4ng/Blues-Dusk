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

public sealed partial class GameConfig :  Bright.Config.BeanBase, System.ICloneable 
{
    public GameConfig(JSONNode _json) 
    {
        { if(!_json["is_david_dead"].IsBoolean) { throw new SerializationException(); }  IsDavidDead = _json["is_david_dead"]; }
        { if(!_json["is_rebellion_start"].IsBoolean) { throw new SerializationException(); }  IsRebellionStart = _json["is_rebellion_start"]; }
        { if(!_json["current_day"].IsNumber) { throw new SerializationException(); }  CurrentDay = _json["current_day"]; }
        { if(!_json["current_stage"].IsNumber) { throw new SerializationException(); }  CurrentStage = _json["current_stage"]; }
        { if(!_json["wear_tie"].IsBoolean) { throw new SerializationException(); }  WearTie = _json["wear_tie"]; }
        { if(!_json["d13_talk_to_zora"].IsBoolean) { throw new SerializationException(); }  D13TalkToZora = _json["d13_talk_to_zora"]; }
        { if(!_json["test"].IsBoolean) { throw new SerializationException(); }  Test = _json["test"]; }
        PostInit();
    }

    public GameConfig(bool is_david_dead, bool is_rebellion_start, int current_day, int current_stage, bool wear_tie, bool d13_talk_to_zora, bool test ) 
    {
        this.IsDavidDead = is_david_dead;
        this.IsRebellionStart = is_rebellion_start;
        this.CurrentDay = current_day;
        this.CurrentStage = current_stage;
        this.WearTie = wear_tie;
        this.D13TalkToZora = d13_talk_to_zora;
        this.Test = test;
        PostInit();
    }

    public static GameConfig DeserializeGameConfig(JSONNode _json)
    {
        return new DataTable.GameConfig(_json);
    }

    public bool IsDavidDead { get; private set; }
    public bool IsRebellionStart { get; private set; }
    public int CurrentDay { get; private set; }
    public int CurrentStage { get; private set; }
    public bool WearTie { get; private set; }
    public bool D13TalkToZora { get; private set; }
    public bool Test { get; private set; }

    public const int __ID__ = -2012855298;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "IsDavidDead:" + IsDavidDead + ","
        + "IsRebellionStart:" + IsRebellionStart + ","
        + "CurrentDay:" + CurrentDay + ","
        + "CurrentStage:" + CurrentStage + ","
        + "WearTie:" + WearTie + ","
        + "D13TalkToZora:" + D13TalkToZora + ","
        + "Test:" + Test + ","
        + "}";
    }

    public object Clone()
    {
        return new GameConfig(this.IsDavidDead, this.IsRebellionStart, this.CurrentDay, this.CurrentStage, this.WearTie, this.D13TalkToZora, this.Test);
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
