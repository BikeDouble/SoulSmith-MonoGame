using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoulSmithMoves;
using SoulSmithStats;

public partial class MoveSelector_Console : MoveSelector
{
	public override void SelectMoveInput(Team thisTeam, Team enemyTeam)
	{
		base.SelectMoveInput(thisTeam, enemyTeam);
		SelectSender();
	}
	
	public override void SelectSender()
	{
		ReadOnlyCollection<IReadOnlyUnit> activeUnits = Team.GetActiveUnitsAsReadOnly();
		if (activeUnits.Count == 0)
		{
			PassTurn();
			return;
		}

		Console.WriteLine("Write number to select unit to act");
		WriteUnits(activeUnits);

		int selection = 0;
		bool repeat = true;
		while (repeat)
		{
			selection = Convert.ToInt32(Console.ReadLine());
			if (selection >= activeUnits.Count)
			{
				Console.WriteLine("Invalid selection, please try again");
			}
            else
            {
                repeat = false;
            }
        }

		ReceiveSender(activeUnits[selection]);
		SelectMove();
    }
	
	public override void SelectMove()
	{
		IReadOnlyUnit userUnit = GetUser();
        ReadOnlyCollection<Move> moveSet = userUnit.MoveSet;
        if (moveSet.Count == 0)
        {
            PassTurn();
			return;
        }

        Console.WriteLine("Write number to select move to use");
        WriteMoves(moveSet);

        int selection = 0;
        bool repeat = true;
        while (repeat)
        {
            selection = Convert.ToInt32(Console.ReadLine());
            if (selection >= moveSet.Count)
            {
                Console.WriteLine("Invalid selection, please try again");
            }
            else
            {
                repeat = false;
            }
        }

        ReceiveMove(moveSet[selection]);
        SelectTarget();
    }
	
	public override void SelectTarget()
	{
		List<IReadOnlyUnit> viableTargets = GetViableTargets();
        if (viableTargets.Count == 0)
        {
            PassTurn();
			return;
        }

        Console.WriteLine("Write number to select target");
        WriteUnits(viableTargets.AsReadOnly());

        int selection = 0;
        bool repeat = true;
        while (repeat)
        {
            selection = Convert.ToInt32(Console.ReadLine());
            if (selection >= viableTargets.Count)
            {
                Console.WriteLine("Invalid selection, please try again");
            }
            else
            {
                repeat = false;
            }
        }

		ReceiveTarget(viableTargets[selection]);
        ReturnMoveInputToCombatManager();
    }

	private static void WriteUnits(ReadOnlyCollection<IReadOnlyUnit> unitList)
	{
		int i = 0;
        foreach (Unit unit in unitList)
        {

            Console.WriteLine(
                "[" + i + "]: " + unit.FriendlyName + ", " + unit.GetBaseStat(StatType.CurHealth) + "/" + unit.GetBaseStat(StatType.MaxHealth) + " HP");
            i++;
        }
    }

    private static void WriteMoves(ReadOnlyCollection<Move> moveList)
    {
        int i = 0;
        foreach (Move move in moveList)
        {

            Console.WriteLine(
                "[" + i + "]: " + move.FriendlyName + ": " + move.Description);
            i++;
        }
    }
}
