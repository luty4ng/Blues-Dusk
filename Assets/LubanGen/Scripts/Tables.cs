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
    public Global.TbGlobal TbGlobal {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbItem = new DataTable.TbItem(loader("datatable_tbitem")); 
        tables.Add("DataTable.TbItem", TbItem);
        TbDice = new DataTable.TbDice(loader("datatable_tbdice")); 
        tables.Add("DataTable.TbDice", TbDice);
        TbCard = new DataTable.TbCard(loader("datatable_tbcard")); 
        tables.Add("DataTable.TbCard", TbCard);
        TbGlobal = new Global.TbGlobal(loader("global_tbglobal")); 
        tables.Add("Global.TbGlobal", TbGlobal);
        PostInit();

        TbItem.Resolve(tables); 
        TbDice.Resolve(tables); 
        TbCard.Resolve(tables); 
        TbGlobal.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbItem.TranslateText(translator); 
        TbDice.TranslateText(translator); 
        TbCard.TranslateText(translator); 
        TbGlobal.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}