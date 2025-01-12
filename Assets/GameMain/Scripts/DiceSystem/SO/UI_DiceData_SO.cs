using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//似乎不需要配表了 继续沿用这个用法？
//Info 目前直接将这个数据存入背包，通过DataPool来获取，以文件名为键值
[CreateAssetMenu(fileName = "New Dice",menuName = "GameMain/DiceData/DiceAsset")]
public class UI_DiceData_SO : ScriptableObject
{
	public string DiceName;
	
	[TextArea]
	public string description;
	public string type => consume ? "闪念" : "定势";

	[Space]
	[Tooltip("定势/闪念")]
	public bool consume = false;
	
	[Space]
	[Tooltip("Up,Down,Right,Left,Forward,Back")]
	public List<UI_DiceFaceBase_SO> faceDatas = new List<UI_DiceFaceBase_SO>();

	/*public UI_DiceFace_SO up;
	public UI_DiceFace_SO down;
	public UI_DiceFace_SO right;
	public UI_DiceFace_SO left;
	public UI_DiceFace_SO forward;
	public UI_DiceFace_SO back;*/
}
