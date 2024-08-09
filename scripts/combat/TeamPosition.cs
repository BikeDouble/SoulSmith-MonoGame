using System;
using SoulSmithMoves;
using SoulSmithModifiers;

public partial class TeamPosition : CanvasItem
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
        _unit.SendEffectEventHandler += SendEffect;
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

    public void DeleteUnit()
	{
		if (_containsUnit)
		{
			OnUnitLeaveCombat();
        }
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

	public event EventHandler<SendEffectEventArgs> SendEffectEventHandler;

	//Connected to Unit
	private void SendEffect(object sender, SendEffectEventArgs e)
	{
		SendEffectEventHandler(this, e);
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

    public EffectResult ExecuteEffect(EffectRequest request)
    {
        if (_containsUnit)
		{
			if (request.Trigger == EffectTrigger.OnMoveBegin)
			{
				if (request.Sender == _unit)
					_movedThisRound = true;
			}

			return _unit.ExecuteEffect(request);
		}
		else
		{
			return new EffectResult();
		}
    }

	public void ReceiveEffectResult(EffectResult result)
	{
		if (_containsUnit)
		{
			_unit.ReceiveEffectResult(result);
		}
	}

    public Unit Unit { get { return _unit; } } //Make sure this contains unit first!
	public bool ContainsUnit {  get { return _containsUnit; } }
	public bool MovedThisRound { get { return _movedThisRound; } set { _movedThisRound = value; } }
	public bool PlayerControlled { get { return _playerControlled; } set { _playerControlled = value; } }
}
