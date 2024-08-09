
using System;
using System.Collections.Generic;

public class CampManager : SoulSmithObject
{
	private const int BASEFOUNDATIONCOUNT = 3;

	private List<CampFoundation> _foundations;

	private List<CampBuilding> _buildingInventory;
	private List<Unit> _unitInventory;

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
		_buildingInventory = new List<CampBuilding>();
		_unitInventory = new List<Unit>();
	}

	public void AddUnitToInventory(Unit unit)
	{
		_unitInventory.Add(unit);
	}

	public void OnRoundEnd()
	{
		foreach (CampFoundation foundation in _foundations)
		{
			foundation.TickTurn();
		}
	}
}
