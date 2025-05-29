using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.UnitStats;
using Microsoft.Xna.Framework.Graphics;
using System;
using SoulSmith.Battle.Effect.Visualization;

namespace SoulSmith.Battle.Effect;
public class Effect
{
    public Effect(Func<GenerateEffectRequestArgs, EffectRequest> generateEffectRequest, EffectTargetingStyle targetingStyle, IEnumerable<Effect> childEffects = null, EffectVisualizationTemplate visualizationTemplate = null, float visDelay = 0, bool priority = false, bool swapSenderAndTarget = false)
    {
        _generateEffectRequest = generateEffectRequest;
        _targetingStyle = targetingStyle;
        _visualizationTemplate = visualizationTemplate;
        _visualizationDelay = visDelay;
        _childEffects = childEffects?.ToList().AsReadOnly();
        _requiresPriority = priority;
        _swapSenderAndTarget = swapSenderAndTarget;
    }

    /// <summary>
	/// Used statically to instantiate effect with no visualization and no child effects
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public static Effect InstantiateNakedEffect(EffectTemplate template)
    {
        Effect effect = new Effect(template.GenerateEffectRequest,
            template.TargetingStyle,
            null,
            null,
            template.VisualizationDelay,
            template.RequiresPriority,
            template.SwapSenderAndTarget);

        return effect;
    }

    private EffectTargetingStyle _targetingStyle;
    private Func<GenerateEffectRequestArgs, EffectRequest> _generateEffectRequest;

    //For child effects
    private bool _requiresPriority = false;
    private bool _swapSenderAndTarget = false;
    private ReadOnlyCollection<Effect> _childEffects;

    //For visualizations 
    private EffectVisualizationTemplate _visualizationTemplate = null;
    private float _visualizationDelay = 0f;

    public Func<GenerateEffectRequestArgs, EffectRequest> GenerateEffectRequest { get { return _generateEffectRequest; } }
    public EffectVisualizationTemplate VisualizationTemplate { get { return _visualizationTemplate; } }
    public EffectTargetingStyle TargetingStyle { get { return _targetingStyle; } }
    public double VisualizationDelay { get { return _visualizationDelay; } }
    public ReadOnlyCollection<Effect> ChildEffects { get { return _childEffects; } }
    public bool RequiresPriority { get { return _requiresPriority; } }
    public bool SwapSenderAndTarget { get { return _swapSenderAndTarget; } }
}
