
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SoulSmithStats
{
    static class StatConstants
    {
        public const int STANDARDDECAYRATE = 33;
    }

    public enum StatType
    {
        None = 0,
        MaxHealth,
        CurHealth,
        Attack,
        Defense,
        CurDecay,
        DecayRate
    }

    public readonly struct StatModifier
    {
        public StatModifier(StatType stat, int flatMod = 0, double additiveMod = 0f, double multiplicativeMod = 1f)
        {
            Stat = stat;
            FlatMod = flatMod;
            AdditiveMod = additiveMod;
            MultiplicativeMod = multiplicativeMod;
        }

        public StatModifier()
        {
            Stat = StatType.None;
            FlatMod = 0;
            AdditiveMod = 0f;
            MultiplicativeMod = 1f;
        }

        public StatType Stat { get; }
        public int FlatMod { get; }
        public double AdditiveMod { get; }
        public double MultiplicativeMod {  get; }
    }
}
