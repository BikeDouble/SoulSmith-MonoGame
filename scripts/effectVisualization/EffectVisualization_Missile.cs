using Microsoft.Xna.Framework;
using System;

public partial class EffectVisualization_Missile : EffectVisualization
{
    private double _totalFlightDuration = 0.5f; //How long should this missile take to get from user to target?
    private double _remainingFlightDuration; 
    private Vector2 _target;
    private Vector2 _start;

    public EffectVisualization_Missile(EffectVisualization_Missile other) : base(other)
    {
        _totalFlightDuration += other._totalFlightDuration;
    }

    public override void OnProcess(double delta)
    {
        base.OnProcess(delta);
        _remainingFlightDuration -= delta;
        if (_remainingFlightDuration <= 0) 
        {
            EmitReadyEffect();
            EndVisualization();
        }
        MoveMissile(delta);
    }

    public override void BeginVisualization(Unit sender, Unit target, double delay)
    {
        base.BeginVisualization(sender, target, delay);
        _remainingFlightDuration = _totalFlightDuration;
        _target = target.Sprite.GetRandomBoundingPointGlobal(BoundingZoneType.EffectReceiver);
        _start = sender.Sprite.GetRandomBoundingPointGlobal(BoundingZoneType.EffectSender);
        Position.Set(_start);
    }

    public Vector2 CalculateOffsetToTarget()
    {
        Vector2 offset = _target - Position.Coordinates; 

        return offset; 
    }

    protected virtual void MoveMissile(double delta)
    {
        double interpolant = Math.Clamp(_remainingFlightDuration / _totalFlightDuration, 0, 1);
        Position.Set(((float)interpolant * _start) + ((float)(1 - interpolant) * _target));
        //MoveMissileTowardsTarget(delta);
    }
}
