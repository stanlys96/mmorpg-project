using UnityEngine;
using System.Collections.Generic;

namespace RPG.Stats
{
  [CreateAssetMenu(fileName = "Progression", menuName = "Stats/Progression", order = 0)]
  public class Progression : ScriptableObject
  {
    [SerializeField] ProgressionCharacterClass[] characterClasses = null;

    Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

    public float GetStat(Stat stat, CharacterClass characterClass, int level)
    {
      BuildLookup();
      float[] levels = lookupTable[characterClass][stat];

      if (levels.Length < level)
      {
        return 0;
      }

      return levels[level - 1];
      // foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
      // {
      //   if (progressionCharacterClass.characterClass != characterClass) continue;

      //   foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
      //   {
      //     if (progressionStat.stat != stat) continue;

      //     if (progressionStat.levels.Length < level) continue;

      //     return progressionStat.levels[level - 1];
      //   }
      // }
      // return 10;
    }

    public int GetLevels(Stat stat, CharacterClass characterClass)
    {
      BuildLookup();
      float[] levels = lookupTable[characterClass][stat];
      return levels.Length;
    }

    private void BuildLookup()
    {
      if (lookupTable != null) return;

      lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

      foreach (ProgressionCharacterClass progressionClass in characterClasses)
      {
        var statLookupTable = new Dictionary<Stat, float[]>();

        foreach (ProgressionStat progressionStat in progressionClass.stats)
        {
          statLookupTable[progressionStat.stat] = progressionStat.levels;
        }

        lookupTable[progressionClass.characterClass] = statLookupTable;
      }
    }

    [System.Serializable]
    class ProgressionCharacterClass
    {
      public CharacterClass characterClass;
      public ProgressionStat[] stats;
    }

    [System.Serializable]
    class ProgressionStat
    {
      public Stat stat;
      public float[] levels;
    }
  }
}