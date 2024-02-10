using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageType : MonoBehaviour
{
    public enum DamageTypes
    {
        normal, critical
    }

    public static Color DamageColor(DamageTypes type)
    {
        Color baseColor = Color.white;
        Color critColor = new Color(255, 171, 0);

        switch (type)
        {
            case DamageTypes.critical: return critColor;
            default: return baseColor;
        }

    }
}
