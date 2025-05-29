using SoulSmith.Battle.Move;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle
{
    public interface IMoveSelector
    {
        public event EventHandler<OfferCompleteMoveInputEventArgs> OfferCompleteMoveInputEventHandler;

        public event EventHandler<ShowMoveSelectUIEventArgs> ShowMoveSelectUIEventHandler;

        public event EventHandler<ShowTargetSelectUIEventArgs> ShowTargetSelectUIEventHandler;

        public event EventHandler<OfferPassTurnEventArgs> OfferPassTurnEventHandler;

        public void ReceiveSender(IReadOnlyUnit unit);

        public void ReceiveMove(Move.Move move);

        public void ReceiveTarget(IReadOnlyUnit target);

        public void SelectMoveInput(IReadOnlyCombatTeam thisTeam, IReadOnlyCombatTeam enemyTeam); //TODO implement ICombatTeam

        public bool PlayerControlled { get; }
    }

    public class ShowTargetSelectUIEventArgs : EventArgs
    {
        public MoveTargetingStyle TargetingStyle;
        public IReadOnlyUnit Sender;
    }

    public class OfferCompleteMoveInputEventArgs : EventArgs
    {
        public MoveInput MoveInput;
    }

    public class ShowMoveSelectUIEventArgs : EventArgs
    {

    }

    public class OfferPassTurnEventArgs : EventArgs
    {

    }
}
