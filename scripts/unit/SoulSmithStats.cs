
using System;
using System.Collections.Generic;

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
        public StatModifier(List<StatType> stats, int flatMod = 0, double additiveMod = 0f, double multiplicativeMod = 1f)
        {
            Stats = stats;
            FlatMod = flatMod;
            AdditiveMod = additiveMod;
            MultiplicativeMod = multiplicativeMod;
        }

        public StatModifier(StatType stat, int flatMod = 0, double additiveMod = 0f, double multiplicativeMod = 1f)
        {
            Stats = new List<StatType> { stat };
            FlatMod = flatMod;
            AdditiveMod = additiveMod;
            MultiplicativeMod = multiplicativeMod;
        }

        /*public StatModifier(Stat stat1, Stat stat2, int flatMod = 0, double additiveMod = 0f, double multiplicativeMod = 1f)
        {
            Stats = new List<Stat>();
            Stats.Add(stat1);
            Stats.Add(stat2);
            FlatMod = flatMod;
            AdditiveMod = additiveMod;
            MultiplicativeMod = multiplicativeMod;
        }

        public StatModifier(Stat stat1, Stat stat2, Stat stat3, int flatMod = 0, double additiveMod = 0f, double multiplicativeMod = 1f)
        {
            Stats = new List<Stat>();
            Stats.Add(stat1);
            Stats.Add(stat2);
            Stats.Add(stat3);
            FlatMod = flatMod;
            AdditiveMod = additiveMod;
            MultiplicativeMod = multiplicativeMod;
        }*/

        public StatModifier()
        {
            Stats = new List<StatType>();
            FlatMod = 0;
            AdditiveMod = 0f;
            MultiplicativeMod = 1f;
        }

        public List<StatType> Stats { get; }
        public int FlatMod { get; }
        public double AdditiveMod { get; }
        public double MultiplicativeMod {  get; }
    }
}
