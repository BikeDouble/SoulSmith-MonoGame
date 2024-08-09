using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public class EffectVisualizationTemplate
{
    private TrackedResource<CanvasItem> _trackedSprite = null;
    private Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> _beginVisualization = (args) => (new());
    private Func<EffectVisualizationProcessArgs, EffectVisualizationProcessOutput> _processVisualization = (args) => (new());
    private float _lifespan = 0;

    public EffectVisualizationTemplate(
        TrackedResource<CanvasItem> trackedSprite,
        Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> begin,
        Func<EffectVisualizationProcessArgs, EffectVisualizationProcessOutput> process,
        float lifespan)
    {
        _trackedSprite = trackedSprite;
        _beginVisualization = begin;
        _processVisualization = process;
        _lifespan = lifespan;
    }

    public EffectVisualization Instantiate()
    {
        return InstantiateInternal(this);
    }

    private static EffectVisualization InstantiateInternal(EffectVisualizationTemplate template)
    {
        return new EffectVisualization(
            (CanvasItem)template._trackedSprite.Resource.DeepClone(),
            template._beginVisualization,
            template._processVisualization,
            template._lifespan);
    }

    public static EffectVisualizationTemplate StraightMissile(
        TrackedResource<CanvasItem> sprite,
        float lifespan)
    {
        return new EffectVisualizationTemplate(
            sprite,
            BeginRandomSenderStartAndTargetEndFunc,
            ProcessMoveStraightTowardsEndPointFunc,
            lifespan);
    }

    public static Func<EffectVisualizationProcessArgs, EffectVisualizationProcessOutput> ProcessMoveStraightTowardsEndPointFunc = (args) =>
    {
        EffectVisualizationProcessOutput output = new();

        double interpolant = Math.Clamp( args.ElapsedLifeSpan / args.TotalLifeSpan, 0, 1);
        Vector2 difference = args.EndingPoint - args.StartingPoint;
        Vector2 desiredPosition = args.StartingPoint + ((float)(1 - interpolant) * difference);
        Vector2 translation = desiredPosition - args.CurrentPosition.Coordinates;

        output.Transformation = new Position(translation);

        return output;
    };

    public static Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> BeginRandomSenderStartAndTargetEndFunc = (args) =>
    {
        EffectVisualizationBeginOutput output = new EffectVisualizationBeginOutput();
        output.StartingPoint = args.Sender.GetRandomBoundingPointGlobal(BoundingZoneType.EffectSender);
        output.EndingPoint = args.Target.GetRandomBoundingPointGlobal(BoundingZoneType.EffectReceiver);
        return output;
    };

}
public class EffectVisualizationBeginArgs
{
    public IReadOnlyUnit Sender;
    public IReadOnlyUnit Target;
}

public class EffectVisualizationBeginOutput
{
    public Vector2 StartingPoint = Vector2.Zero;
    public Vector2 EndingPoint = Vector2.Zero;
    public List<float> Params = null;
}

public class EffectVisualizationProcessArgs
{
    public Position CurrentPosition = null;
    public Vector2 StartingPoint = Vector2.Zero;
    public Vector2 EndingPoint = Vector2.Zero;
    public float ElapsedLifeSpan = 0f;
    public float TotalLifeSpan = 0f;
    public List<float> Params = null;
}

public class EffectVisualizationProcessOutput
{
    public Position Transformation = null;
}
