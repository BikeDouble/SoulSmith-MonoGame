
using System;
using System.Collections.Generic;

public class CampManager : SoulSmithObject
{
	private const int BASEFOUNDATIONCOUNT = 3;

	private List<CampFoundation> _foundations;

	public CampManager() 
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
