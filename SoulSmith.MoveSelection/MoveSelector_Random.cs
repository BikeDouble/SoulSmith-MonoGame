using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoulSmith.Battle.Moves;
using SoulSmith.Combats;
using SoulSmith.Core;
using SoulSmith.Units;
using SoulSmith.Battle;

namespace SoulSmith.MoveSelection;
public partial class MoveSelector_Random : MoveSelector
{
	public override void SelectMoveInput(IReadOnlyCombatTeam thisTeam, IReadOnlyCombatTeam enemyTeam)
	{
		base.SelectMoveInput(thisTeam, enemyTeam);
		_triedSender.Clear();
		_triedMoves.Clear();
        SelectSender();
	}

	private List<IReadOnlyUnit> _triedSender = new List<IReadOnlyUnit>();

    public override void SelectSender()
	{
		List<IReadOnlyUnit> activeUnits = Team.GetActiveUnitsAsReadOnly();
		foreach (IReadOnlyUnit triedSender in _triedSender) activeUnits.Remove(triedSender);

        if (activeUnits.Count == 0)
		{
			PassTurn();
            _triedSender.Clear();
            _triedMoves.Clear();
            return;
		}
		int selectedIndex = Rand.RandInt(activeUnits.Count);
		IReadOnlyUnit selectedUnit = activeUnits[selectedIndex];
        ReceiveSender(selectedUnit);
		_triedSender.Add(selectedUnit);
        SelectMove();
    }

	private List<Move> _triedMoves = new List<Move>();

    public override void SelectMove()
	{
		IReadOnlyUnit userUnit = GetSender();
        List<Move> moveSet = new List<Move>(userUnit.MoveSet);
		foreach (Move triedMove in _triedMoves) moveSet.Remove(triedMove);
        if (moveSet.Count == 0)
        {
            _triedMoves.Clear();
			SelectSender();
			return;
        }
        int selectedIndex = Rand.RandInt(moveSet.Count);
		Move selectedMove = moveSet[selectedIndex];
		_triedMoves.Add(selectedMove);
        ReceiveMove(selectedMove);
        SelectTarget();
    }
	
	public override void SelectTarget()
	{
		List<IReadOnlyUnit> viableTargets = GetViableTargets();
        if (viableTargets.Count == 0)
        {
			SelectMove();
			return;
        }
        int selectedIndex = Rand.RandInt(viableTargets.Count);
		ReceiveTarget(viableTargets[selectedIndex]);
        ReturnMoveInputToCombatManager();
        _triedSender.Clear();
        _triedMoves.Clear();
    }
}
