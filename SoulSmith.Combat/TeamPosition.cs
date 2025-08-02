using SoulSmith.Units;
using SoulSmith.Object.Canvas;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle;

namespace SoulSmith.Combat;
public class TeamPosition : CanvasObject, IReadOnlyTeamPosition
{
    public const string DEPLOYUNITBUTTONIDLERESOURCEKEY = "ZonedResources/UI/Units/Moves/TargetButtonIdle";
    public const string DEPLOYUNITBUTTONHOVEREDRESOURCEKEY = "ZonedResources/UI/Units/Moves/TargetButtonHovered";

    // Children
	private Unit _unit;
    private ButtonObject _deployUnitButton;

	private bool _movedThisRound = false;
	private bool _containsUnit = false;
    private readonly bool _playerControlled;

    public TeamPosition(float x, float y, bool playerControlled, float width = 1, float height = 1) : base(new Core.Position(x, y, width, height))
    {
        _playerControlled = playerControlled;
        InitializeDeployUnitButton();
    }

    private void InitializeDeployUnitButton()
    {
        _deployUnitButton = new ButtonObject(DEPLOYUNITBUTTONIDLERESOURCEKEY, DEPLOYUNITBUTTONHOVEREDRESOURCEKEY);
        _deployUnitButton.ButtonPressedEventHandler += OnDeployUnitButtonPressed;
        AddChild(_deployUnitButton);
        _deployUnitButton.Hide();
    }

    public void AssignUnit(Unit unit)
    {
        _unit = unit;
        _containsUnit = true;
        _unit.OfferMoveAndUserEventHandler += OnOfferMoveAndUser;
        _unit.OfferTargetEventHandler += OnOfferTarget;
        _unit.EnqueueEffectInputEventHandler += EnqueueEffect;
        _unit.UnitDeathCallEventHandler += OnUnitDeathCall;
		_unit.UnitRetreatCallEventHandler += OnUnitRetreatCall;
        AddChild(unit);

        _unit.OnJoinCombat();
        _deployUnitButton.Hide();
    }

    public void RetreatUnit()
    {
        if (!_containsUnit)
        {
            return;
        }

        _unit.OnRetreat();
		OnUnitLeaveCombat();
    }

    public void KillUnit()
    {
        if (!_containsUnit)
        {
            return;
        }

        _unit.OnDeath();
        OnUnitLeaveCombat();
    }

    //
    // Listeners
    //

    public void ShowDeployUnitUI()
    {
        _deployUnitButton.Show();
    }

    public void HideDeplotUnitButton()
    {
        _deployUnitButton.Hide();
    }

    public void ShowMoveSelectUI()
	{
        if (_containsUnit)
        {
            if (!_movedThisRound) _unit.ShowMoveSelectUI();
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

    //Connected to Unit
	public event EventHandler<UnitDeathCallArgs> UnitDeathCallEventHandler;

	private void OnUnitDeathCall(object sender, UnitDeathCallArgs e)
	{
		e.CallingUnit = _unit;

		UnitDeathCallEventHandler(this, e);
	}

    //Connected to Unit
    public event EventHandler<UnitRetreatCallArgs> UnitRetreatCallEventHandler;

    private void OnUnitRetreatCall(object sender, UnitRetreatCallArgs e)
    {
        UnitRetreatCallEventHandler?.Invoke(this, e);
    }

    public EventHandler<DeployUnitButtonPressedEventArgs> DeployUnitButtonPressedEventHandler;

    private void OnDeployUnitButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        DeployUnitButtonPressedEventArgs e2 = new DeployUnitButtonPressedEventArgs();
        e2.CallingPosition = this;
        DeployUnitButtonPressedEventHandler?.Invoke(this, e2);
    }

    public void OnUnitLeaveCombat()
	{
        _unit.OfferMoveAndUserEventHandler -= OnOfferMoveAndUser;
        _unit.OfferTargetEventHandler -= OnOfferTarget;
        _unit.EnqueueEffectInputEventHandler -= EnqueueEffect;
        _unit.UnitDeathCallEventHandler -= OnUnitDeathCall;
        _unit.UnitRetreatCallEventHandler -= OnUnitRetreatCall;
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

	public Result ExecutePayload(Payload payload)
	{
		Result result = null;

        if (_containsUnit)
        {
            result = _unit.ExecuteEffectRequest(payload);
        }

		return result;
    }

    public void ModifyEffectRequest(Payload request)
    {
        if (_containsUnit)
		{
			_unit.ModifyEffectRequest(request);
		}
    }

	public void ReactToPayloadResult(Result result)
	{
		if (_containsUnit)
		{
            switch (result)
            {
                case TriggerResult triggerResult:
                    if (triggerResult.Sender == _unit)
                        if (triggerResult.Trigger == CombatTrigger.OnMoveBegin)
                            _movedThisRound = true; //TODO move to unit
                    break;
            }

            _unit.ReactToPayloadResult(result);
        }
	}

    public Unit Unit { get { return _unit; } } //Make sure this contains unit first!
	public bool ContainsUnit {  get { return _containsUnit; } } //TODO make this a Unit != null check
	public bool MovedThisRound { get { return _movedThisRound; } set { _movedThisRound = value; } }
}

public class DeployUnitButtonPressedEventArgs : EventArgs
{
    public IReadOnlyTeamPosition CallingPosition;
}