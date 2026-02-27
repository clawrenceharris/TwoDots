

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DotProbability{
    public DotType type;
    public float value;
}


public class DotSpawner : MonoBehaviour
{
    [SerializeField] private DotProbability[] dotProbabilities;
    private System.Random random = new();



    public DotsObject GetRandomDot(List<DotsObject> dotsToSpawn)
    {
        var types = dotsToSpawn.Select(d => LevelLoader.FromJsonType<DotType>(d.Type));
        var filteredProbabilities = dotProbabilities.Where(p => types.Contains(p.type));

        if (!filteredProbabilities.Any())
        {
            throw new InvalidOperationException("No valid dot types found for spawning.");
        }

        float totalProbability = filteredProbabilities.Sum(p => p.value);

        foreach (var p in filteredProbabilities)
        {
            p.value /= totalProbability;
        }

        float randomNumber = (float)random.NextDouble();
        float cumulativeProbability = 0;

        foreach (var kvp in filteredProbabilities)
        {

            cumulativeProbability += kvp.value;
            if (randomNumber < cumulativeProbability)
            {
                return dotsToSpawn.First(d => d.Type == LevelLoader.ToJsonDotType(kvp.type));
            }
        }

        throw new InvalidOperationException("Unexpected error: Probability calculations might be incorrect."); // This shouldn't happen, but a failsafe
    }
}