using SoulSmithStats;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class Modifier_Stat : Modifier
{
    private StatModifier _statModifier;

    public Modifier_Stat(StatModifier statModifier, int duration = -1) : base(duration)
    { 
        _statModifier = statModifier;
    }

    public override StatModifier GetStatModifier(StatType stat)
    { 
        return _statModifier;
    }
}
