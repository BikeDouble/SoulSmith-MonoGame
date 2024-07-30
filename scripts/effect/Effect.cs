using SoulSmithMoves;
using SoulSmithEmotions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmithStats;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using System;

public class Effect 
{
    public Effect(Func<GenerateEffectRequestArgs, EffectRequest> generateEffectRequest, EffectTargetingStyle targetingStyle, IEnumerable<Effect> childEffects = null, EffectVisualization visualization = null, float visDelay = 0, bool priority = false, bool swapSenderAndTarget = false) 
    {
        _generateEffectRequest = generateEffectRequest;
        _targetingStyle = targetingStyle;
        _visualizationDelay = visDelay;
        _childEffects = childEffects?.ToList().AsReadOnly();
        _requiresPriority = priority;
        _swapSenderAndTarget = swapSenderAndTarget;
    }

    private EffectTargetingStyle _targetingStyle;
    private Func<GenerateEffectRequestArgs, EffectRequest> _generateEffectRequest;

    //For child effects
    private bool _requiresPriority = false;
    private bool _swapSenderAndTarget = false;
    private ReadOnlyCollection<Effect> _childEffects;

    //For visualizations 
    private EffectVisualization _visualization = null;
    private float _visualizationDelay = 0f;

    public Func<GenerateEffectRequestArgs, EffectRequest> GenerateEffectRequest { get { return _generateEffectRequest; } }
    public EffectVisualization Visualization { get { return _visualization; } }
    public EffectTargetingStyle TargetingStyle { get { return _targetingStyle; } }
    public double VisualizationDelay { get { return _visualizationDelay; } }
    public ReadOnlyCollection<Effect> ChildEffects { get { return _childEffects; } }
    public bool RequiresPriority { get { return _requiresPriority; } }
    public bool SwapSenderAndTarget { get { return _swapSenderAndTarget; } }
}
