
using System;

public class UnitFactory
{
    public event EventHandler<PutUnitInCombatEventArgs> PutUnitInCombatEventHandler;

    protected void PutUnitInCombat(string unitType, int position)
    {
        Unit unit = MasterAssetLoader.InstantiateUnit(unitType);
    }

    protected void PutUnitInCombat(Unit unit, int position) 
    {
        PutUnitInCombatEventArgs e = new();
        e.Unit = unit;
        e.Position = position;
        PutUnitInCombatEventHandler?.Invoke(this, new PutUnitInCombatEventArgs());
    }

}

public class PutUnitInCombatEventArgs : EventArgs
{
    public Unit Unit;
    public int Position;
}