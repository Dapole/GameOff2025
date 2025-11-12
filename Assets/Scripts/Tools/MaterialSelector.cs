using UnityEngine;

public class MaterialSelector : MonoBehaviour
{
    [Header("Materials List")]
    public Material[] materials;

    [Header("selectedIndex")]
    [Range(0, 10)]
    public int selectedIndex = 0;

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        ApplyMaterial();
    }

    void OnValidate()
    {
        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        if (materials == null || materials.Length == 0) return;
        if (_renderer == null) _renderer = GetComponent<Renderer>();

        selectedIndex = Mathf.Clamp(selectedIndex, 0, materials.Length - 1);
        _renderer.sharedMaterial = materials[selectedIndex];
    }
}
