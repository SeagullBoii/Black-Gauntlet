using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(DecalProjector))]
public class BloodDecalRandomization : MonoBehaviour
{
    [SerializeField] bool randomizeOnStart = true;

    [Header("Material")]
    [SerializeField] string shaderString;

    #region Sprites
    [SerializeField]
    Texture mask;
    #endregion

    #region Colors
    [SerializeField]
    Color baseColor, darkestColor;
    #endregion

    #region Floats
    [SerializeField]
    float opacity, range, fuzziness,
         cellDensity, power, maskCellDensity,
         speed, maskMultiplier, maskOffset;
    #endregion

    private DecalProjector decalProjector;
    Material newMaterial;

    private void Start()
    {
        if (randomizeOnStart)
            CreateMaterial();
    }

    [ContextMenu("Randomize")]
    public void CreateMaterial()
    {
        decalProjector = GetComponent<DecalProjector>();

        newMaterial = new Material(Shader.Find(shaderString));
        SetValues();
        decalProjector.material = newMaterial;
    }

    public void SetValues()
    {
        float maskMultiplierRand = Random.Range(0, 0.5f);
        float maskCellDensityRand = Random.Range(-2f, 1f);
        float powerRand = Random.Range(-0.5f, 0);

        newMaterial.SetTexture("_Mask", mask);

        newMaterial.SetColor("_BaseColor", baseColor);
        newMaterial.SetColor("_DarkestColor", darkestColor);

        newMaterial.SetFloat("_Opacity", opacity);
        newMaterial.SetFloat("_Range", range);
        newMaterial.SetFloat("_Fuzziness", fuzziness);
        newMaterial.SetFloat("_CellDensity", cellDensity);
        newMaterial.SetFloat("_Power", power + powerRand);
        newMaterial.SetFloat("_MaskCellDensity", maskCellDensity + maskCellDensityRand);
        newMaterial.SetFloat("_Speed", speed);
        newMaterial.SetFloat("_MaskMultiplier", maskMultiplier + maskMultiplierRand);
        newMaterial.SetFloat("_MaskOffset", maskOffset);
    }
}
