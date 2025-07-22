using SoulSmith.Units;
using SoulSmith.Object.Canvas;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Trigger;

namespace SoulSmith.Combat;
public class TeamPosition : CanvasObject
{
	private Unit _unit;

	private bool _movedThisRound = false;
	private bool _containsUnit = false;
	private bool _playerControlled;

    public TeamPosition() : base()
    {
		
    }

    public TeamPosition(int x, int y) : base(x, y)
    {
		
    }

    public void AssignUnit(Unit unit)
    {
        _unit = unit;
        _containsUnit = true;
        _unit.PlayerControlled = _playerControlled;
        _unit.OfferMoveAndUserEventHandler += OnOfferMoveAndUser;
        _unit.OfferTargetEventHandler += OnOfferTarget;
        _unit.EnqueueEffectInputEventHandler += EnqueueEffect;
        _unit.UnitDeathCallEventHandler += OnUnitDeathCall;
		AddChild(unit);

        _unit.OnJoinCombat();
    }

    public void RemoveUnitFromCombat()
    {
        if (!_containsUnit)
        {
            return;
        }

        _unit.RemoveFromCombat();
		OnUnitLeaveCombat();
    }

    //
    // Listeners
    //

    public void ShowMoveSelectUI()
	{
		if (!_movedThisRound && _containsUnit)
		{
			_unit.ShowMoveSelectUI();
		}
	}
	
	public void HideMoveSelectUI()
	{
		if (_containsUnit)
		{
			_unit.HideMoveSelectUI();
		}
	}
	
	public void ShowTargetSelectUI()
	{
		if (_containsUnit)
		{
			_unit.ShowTargetSelectUI();
		}
	}

	public void HideTargetSelectUI()
	{
		if (_containsUnit)
		{
			_unit.HideTargetSelectUI();
		}
	}

    public event EventHandler<MoveButtonPressedEventArgs> OfferMoveAndUserEventHandler;

    //Connected to Unit
    private void OnOfferMoveAndUser(object sender, MoveButtonPressedEventArgs args)
	{
		OfferMoveAndUserEventHandler(this, args);
	}

    public event EventHandler<TargetButtonPressedEventArgs> OfferTargetEventHandler;

    //Connected to Unit
    private void OnOfferTarget(object sender, TargetButtonPressedEventArgs args)
	{
		OfferTargetEventHandler(this, args);
	}

	public event EventHandler<UnitDeathCallArgs> UnitDeathCallEventHandler;

	private void OnUnitDeathCall(object sender, UnitDeathCallArgs e)
	{
		e.CallingUnit = _unit;

		UnitDeathCallEventHandler(this, e);
	}
	
	public void OnUnitLeaveCombat()
	{
		_containsUnit = false;
		RemoveChild(_unit);
		_unit = null;
	}
	
	public void OnBeginRound()
	{
		_movedThisRound = false;
	}

    public event EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler;

    private void EnqueueEffect(object sender, EnqueueEffectInputEventArgs e)
    {
        EnqueueEffectInputEventHandler(this, e);
    }

	public EffectResult ExecuteEffectRequest(EffectRequest request)
	{
		EffectResult result = null;

        if (_containsUnit)
        {
            result = _unit.ExecuteEffectRequest(request);
        }

		return result;
    }

    public void ModifyEffectRequest(EffectRequest request)
    {
        if (_containsUnit)
		{
			_unit.ModifyEffectRequest(request);
		}
    }

	public void ReactToEffectResult(EffectResult result)
	{
		if (_containsUnit)
		{
			_unit.ReactToEffectResult(result);

            if (result.TriggerApplied == CombatTrigger.OnMoveBegin)
            {
                if (result.Sender == _unit)
                    _movedThisRound = true;
            }
        }
	}

    public Unit Unit { get { return _unit; } } //Make sure this contains unit first!
	public bool ContainsUnit {  get { return _containsUnit; } }
	public bool MovedThisRound { get { return _movedThisRound; } set { _movedThisRound = value; } }
	public bool PlayerControlled { get { return _playerControlled; } set { _playerControlled = value; } }
}
