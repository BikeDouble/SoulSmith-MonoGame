using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Drawing;
using SoulSmith.Asset;

public class EffectVisualizationTemplate
{
    private TrackedAsset<CanvasItem> _trackedSprite = null;
    private Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> _beginVisualization = (args) => (new());
    private Action<EffectVisualizationProcessArgs> _processVisualization = null;
    private float _lifespan = 0;
    private float _effectActivationTimer = 0;

    public EffectVisualizationTemplate(
        TrackedAsset<CanvasItem> trackedSprite,
        Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> begin,
        Action<EffectVisualizationProcessArgs> process,
        float lifespan,
        float effectActivationTimer = -1)
    {
        _trackedSprite = trackedSprite;
        _beginVisualization = begin;
        _processVisualization = process;
        _lifespan = lifespan;
        _effectActivationTimer = effectActivationTimer;
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
            template._lifespan,
            template._effectActivationTimer);
    }

    public static EffectVisualizationTemplate StraightMissile(
        TrackedAsset<CanvasItem> sprite,
        float lifespan)
    {
        return new EffectVisualizationTemplate(
            sprite,
            BeginRandomSenderStartAndTargetEndFunc,
            ProcessMoveStraightTowardsEndPointFunc,
            lifespan);
    }

    public static EffectVisualizationTemplate GrowAndFadeOnTarget(
        TrackedAsset<CanvasItem> sprite,
        float lifespan,
        float effectActivationTimer = -1)
    {
        return new EffectVisualizationTemplate(
            sprite,
            BeginStationaryGrowAndFadeStartFunc,
            ProcessStationaryGrowAndFadeFunc,
            lifespan,
            effectActivationTimer);
    }

    public static Action<EffectVisualizationProcessArgs> ProcessMoveStraightTowardsEndPointFunc = (args) =>
    {
        ITransformable item = args.Transformables[0];

        double interpolant = Math.Clamp( args.ElapsedLifeSpan / args.TotalLifeSpan, 0, 1);
        Vector2 difference = args.EndingPoint - args.StartingPoint;
        Vector2 desiredPosition = args.StartingPoint + ((float)interpolant * difference);

        item.Set(desiredPosition);
    };

    public static Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> BeginRandomSenderStartAndTargetEndFunc = (args) =>
    {
        EffectVisualizationBeginOutput output = new EffectVisualizationBeginOutput();
        output.StartingPoint = args.Sender.GetRandomBoundingPointGlobal(BoundingZoneType.EffectSender);
        args.Transformables[0].Set(output.StartingPoint);
        output.EndingPoint = args.Target.GetRandomBoundingPointGlobal(BoundingZoneType.EffectReceiver);
        return output;
    };

    public static Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> BeginStationaryGrowAndFadeStartFunc = (args) =>
    {
        EffectVisualizationBeginOutput output = new EffectVisualizationBeginOutput();
        Vector2 startingPoint = args.Target.GetRandomBoundingPointGlobal(BoundingZoneType.EffectSender);
        output.StartingPoint = startingPoint;
        ITransformable item = args.Transformables[0];
        item.Set(startingPoint);
        item.ScaleMultiplicative(Vector2.Zero);
        return output;
    };

    public static Action<EffectVisualizationProcessArgs> ProcessStationaryGrowAndFadeFunc = (args) =>
    {
        ITransformable item = args.Transformables[0];
        float progress = (float)(args.Delta / args.TotalLifeSpan);

        float alphaChange = -(255f * progress);

        item.ChangeTintAdditive(0, 0, 0, alphaChange);

        item.ScaleAdditive(new Vector2(4 * progress));
    };

}
public class EffectVisualizationBeginArgs
{
    public IReadOnlyUnit Sender;
    public IReadOnlyUnit Target;
    public ReadOnlyCollection<ITransformable> Transformables = null;
}

public class EffectVisualizationBeginOutput
{
    public Vector2 StartingPoint = Vector2.Zero;
    public Vector2 EndingPoint = Vector2.Zero;
    public List<float> Params = null;
}

public class EffectVisualizationProcessArgs
{
    public Vector2 StartingPoint = Vector2.Zero;
    public Vector2 EndingPoint = Vector2.Zero;
    public Vector4 ColorChange = Vector4.Zero;
    public ReadOnlyCollection<ITransformable> Transformables = null;
    public float ElapsedLifeSpan = 0f;
    public float TotalLifeSpan = 0f;
    public double Delta = 0f;
    public List<float> Params = null;
}

