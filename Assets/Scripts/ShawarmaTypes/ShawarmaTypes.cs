using System.Collections.Generic;
using UnityEngine;

/// <summary>Phase 6: Static definitions for shawarma types. Doc: Classic $50 → Spicy $60 → Premium $75 → Gourmet $100 → Signature $150.</summary>
public static class ShawarmaTypes
{
    private static List<ShawarmaTypeDefinition> _definitions;

    public const string IdClassic = "classic";
    public const string IdSpicy = "spicy";
    public const string IdPremium = "premium";
    public const string IdGourmet = "gourmet";
    public const string IdSignature = "signature";

    public static IReadOnlyList<ShawarmaTypeDefinition> Definitions
    {
        get
        {
            if (_definitions == null) BuildDefinitions();
            return _definitions;
        }
    }

    private static void BuildDefinitions()
    {
        _definitions = new List<ShawarmaTypeDefinition>
        {
            new ShawarmaTypeDefinition { id = IdClassic, displayName = "Classic", baseValue = 50f, unlockTotalEarnings = 0f, unlockChefStars = 0 },
            new ShawarmaTypeDefinition { id = IdSpicy, displayName = "Spicy", baseValue = 60f, unlockTotalEarnings = 10000f, unlockChefStars = 0 },
            new ShawarmaTypeDefinition { id = IdPremium, displayName = "Premium", baseValue = 75f, unlockTotalEarnings = 50000f, unlockChefStars = 0 },
            new ShawarmaTypeDefinition { id = IdGourmet, displayName = "Gourmet", baseValue = 100f, unlockTotalEarnings = 500000f, unlockChefStars = 0 },
            new ShawarmaTypeDefinition { id = IdSignature, displayName = "Signature", baseValue = 150f, unlockTotalEarnings = 2000000f, unlockChefStars = 0 }
        };
    }

    public static float GetBaseValue(string typeId)
    {
        var d = GetDefinition(typeId);
        return d != null ? d.baseValue : 50f;
    }

    public static ShawarmaTypeDefinition GetDefinition(string typeId)
    {
        if (string.IsNullOrEmpty(typeId)) typeId = IdClassic;
        foreach (var d in Definitions)
            if (d.id == typeId) return d;
        return Definitions.Count > 0 ? Definitions[0] : null;
    }

    public static bool IsUnlocked(string typeId)
    {
        var d = GetDefinition(typeId);
        if (d == null) return false;
        if (PlayerProgress.Instance == null) return d.unlockChefStars == 0 && d.unlockTotalEarnings <= 0;
        if (d.unlockChefStars > 0 && PlayerProgress.Instance.ChefStars < d.unlockChefStars) return false;
        if (d.unlockTotalEarnings > 0 && PlayerProgress.Instance.TotalEarnings < d.unlockTotalEarnings) return false;
        return true;
    }

    public static string GetUnlockDescription(string typeId)
    {
        var d = GetDefinition(typeId);
        if (d == null) return "";
        if (d.unlockChefStars == 0 && d.unlockTotalEarnings <= 0) return "Unlocked";
        var parts = new List<string>();
        if (d.unlockTotalEarnings > 0) parts.Add($"${d.unlockTotalEarnings:N0} total earned");
        if (d.unlockChefStars > 0) parts.Add($"{d.unlockChefStars} Chef Star(s)");
        return string.Join(" or ", parts);
    }
}
