using System;
using UnityEngine;

/// <summary>Phase 6: One shawarma type (Classic, Spicy, etc.). Unlock by TotalEarnings and/or ChefStars.</summary>
[Serializable]
public class ShawarmaTypeDefinition
{
    public string id;
    public string displayName;
    public float baseValue;
    /// <summary>Unlock when TotalEarnings >= this (0 = always).</summary>
    public float unlockTotalEarnings;
    /// <summary>Unlock when ChefStars >= this (0 = always).</summary>
    public int unlockChefStars;
}
