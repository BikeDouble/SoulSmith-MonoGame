
using System;
using System.Collections.Generic;
using SoulSmith.Object.Canvas;

namespace SoulSmith.Camp;
public class Camp : CanvasObject
{
	private const int BASEFOUNDATIONCOUNT = 3;

	private List<CampFoundation> _foundations;

	public Camp() 
	{ 
		Initialize();
	}

	public void Initialize()
	{
		InitializeInventories();
		InitializeFoundations();
	}

	private void InitializeFoundations()
	{
		_foundations = new List<CampFoundation>(BASEFOUNDATIONCOUNT);

	}

	private void InitializeInventories()
	{

	}

	public void OnRoundEnd()
	{
		foreach (CampFoundation foundation in _foundations)
		{
			foundation.TickTurn();
		}
	}
}
