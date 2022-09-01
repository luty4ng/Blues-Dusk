//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using SimpleJSON;

namespace LubanConfig
{
   
public sealed partial class Tables
{
    public DataTable.TbItem TbItem {get; }
    public DataTable.TbDice TbDice {get; }
    public DataTable.TbCard TbCard {get; }
    public DataTable.TbDefaultSetting TbDefaultSetting {get; }
    public DataTable.TbGameConfig TbGameConfig {get; }
    public DataTable.TbUIConfig TbUIConfig {get; }
    public DataTable.TbEntityConfig TbEntityConfig {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbItem = new DataTable.TbItem(loader("datatable_tbitem")); 
        tables.Add("DataTable.TbItem", TbItem);
        TbDice = new DataTable.TbDice(loader("datatable_tbdice")); 
        tables.Add("DataTable.TbDice", TbDice);
        TbCard = new DataTable.TbCard(loader("datatable_tbcard")); 
        tables.Add("DataTable.TbCard", TbCard);
        TbDefaultSetting = new DataTable.TbDefaultSetting(loader("datatable_tbdefaultsetting")); 
        tables.Add("DataTable.TbDefaultSetting", TbDefaultSetting);
        TbGameConfig = new DataTable.TbGameConfig(loader("datatable_tbgameconfig")); 
        tables.Add("DataTable.TbGameConfig", TbGameConfig);
        TbUIConfig = new DataTable.TbUIConfig(loader("datatable_tbuiconfig")); 
        tables.Add("DataTable.TbUIConfig", TbUIConfig);
        TbEntityConfig = new DataTable.TbEntityConfig(loader("datatable_tbentityconfig")); 
        tables.Add("DataTable.TbEntityConfig", TbEntityConfig);
        PostInit();

        TbItem.Resolve(tables); 
        TbDice.Resolve(tables); 
        TbCard.Resolve(tables); 
        TbDefaultSetting.Resolve(tables); 
        TbGameConfig.Resolve(tables); 
        TbUIConfig.Resolve(tables); 
        TbEntityConfig.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbItem.TranslateText(translator); 
        TbDice.TranslateText(translator); 
        TbCard.TranslateText(translator); 
        TbDefaultSetting.TranslateText(translator); 
        TbGameConfig.TranslateText(translator); 
        TbUIConfig.TranslateText(translator); 
        TbEntityConfig.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}
