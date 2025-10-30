
using System;
using SoulSmith.Units;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SoulSmith.Camps;
public partial class CampBuilding 
{
    private List<Unit> _inputUnits;
    private int _inputSlots;
    private List<Unit> _outputUnits;
    private int _outputSlots;
    private int _remainingDuration = 0;
    private bool _working = false;

    public CampBuilding(int inputSlots, int outputSlots)
    {
        _inputSlots = inputSlots;
        _inputUnits = new List<Unit>(inputSlots);
        _outputSlots = outputSlots;
        _outputUnits = new List<Unit>(outputSlots);
    }

    public virtual void Tick()
    {
        _remainingDuration--;
        if (_remainingDuration <= 0) 
        {
            Complete();
        }
    }

    public void DisposeInputUnits()
    {
        foreach (var unit in _inputUnits)
        {
            unit.Dispose();
        }

        _inputUnits.Clear();
    }

    public virtual void Begin()
    {
        _remainingDuration = CalculateWorkTime();
        _working = true;
    }

    public virtual void Complete()
    {
        _working = false;
    }

    public void Cancel()
    {
        _working = false;
    }

    public virtual int CalculateWorkTime()
    {
        return 1;
    }

    /// <summary>
    /// Places a unit into an output slot. Throws an exception if no output slots are available.
    /// </summary>
    /// <param name="unit">Unit to output.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void OutputUnit(Unit unit)
    {
        if (_outputUnits.Count < _outputSlots)
        {
            _outputUnits.Add(unit);
        }
        else
        {
            throw new InvalidOperationException("Output slots are full.");
        }
    }

    public bool Working { get { return _working; } }
    public int InputCapacity { get { return _inputSlots; } }
    public int OutputCapacity { get { return _outputSlots; } }
    public ReadOnlyCollection<Unit> InputUnits { get { return _inputUnits.AsReadOnly(); } }
    public ReadOnlyCollection<Unit> OutputUnits { get { return _outputUnits.AsReadOnly(); } }
}
