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

public sealed partial class UIConfig :  Bright.Config.BeanBase, System.ICloneable 
{
    public UIConfig(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["asset_name"].IsString) { throw new SerializationException(); }  AssetName = _json["asset_name"]; }
        { if(!_json["ui_group_name"].IsString) { throw new SerializationException(); }  UiGroupName = _json["ui_group_name"]; }
        { if(!_json["allow_multi_instance"].IsBoolean) { throw new SerializationException(); }  AllowMultiInstance = _json["allow_multi_instance"]; }
        { if(!_json["pause_covered_ui_form"].IsBoolean) { throw new SerializationException(); }  PauseCoveredUiForm = _json["pause_covered_ui_form"]; }
        PostInit();
    }

    public UIConfig(int id, string asset_name, string ui_group_name, bool allow_multi_instance, bool pause_covered_ui_form ) 
    {
        this.Id = id;
        this.AssetName = asset_name;
        this.UiGroupName = ui_group_name;
        this.AllowMultiInstance = allow_multi_instance;
        this.PauseCoveredUiForm = pause_covered_ui_form;
        PostInit();
    }

    public static UIConfig DeserializeUIConfig(JSONNode _json)
    {
        return new DataTable.UIConfig(_json);
    }

    /// <summary>
    /// 界面编号
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 资源名
    /// </summary>
    public string AssetName { get; private set; }
    /// <summary>
    /// 界面组名称
    /// </summary>
    public string UiGroupName { get; private set; }
    /// <summary>
    /// 是否允许多个界面实例
    /// </summary>
    public bool AllowMultiInstance { get; private set; }
    /// <summary>
    /// 是否暂停被其覆盖的界面
    /// </summary>
    public bool PauseCoveredUiForm { get; private set; }

    public const int __ID__ = 1503010688;
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
        + "Id:" + Id + ","
        + "AssetName:" + AssetName + ","
        + "UiGroupName:" + UiGroupName + ","
        + "AllowMultiInstance:" + AllowMultiInstance + ","
        + "PauseCoveredUiForm:" + PauseCoveredUiForm + ","
        + "}";
    }

    public object Clone()
    {
        return new UIConfig(this.Id, this.AssetName, this.UiGroupName, this.AllowMultiInstance, this.PauseCoveredUiForm);
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
