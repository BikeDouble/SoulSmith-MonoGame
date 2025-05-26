
using System.Collections.Generic;

public class EnemySpawnSelector
{
    private SoulSmithWeightedList<string> _activeSpawnList;

    /// <summary>
    /// Sort by activation round, lowest to highest
    /// </summary>
    private List<(SoulSmithWeightedList<string> List, string Name, int ActivationRound)> _spawnLists;

    public EnemySpawnSelector()
    {
        _spawnLists = MasterAssetLoader.LoadMasterEnemySpawnList();
        UpdateRound(1);
    }

    public string Select()
    {
        return _activeSpawnList?.Next();
    }

    private void SortSpawnLists()
    {
        //TODO
    }

    public void UpdateRound(int newRoundNumber)
    {
        _activeSpawnList = SelectListByRound(newRoundNumber);
    }

    private SoulSmithWeightedList<string> SelectListByRound(int roundNumber)
    {
        SoulSmithWeightedList<string> retList = null;

        foreach (var list in _spawnLists)
        {
            if (list.ActivationRound <= roundNumber)
            {
                retList = list.List;
            }
            else if (retList != null)
            {
                return retList;
            }
        }

        return retList;
    }

    private SoulSmithWeightedList<string> SelectListByName(string name)
    {
        foreach (var list in _spawnLists)
        {
            if (string.Compare(list.Name, name, true) == 0)
            {
                return list.List;
            }
        }

        return null;
    }
}

