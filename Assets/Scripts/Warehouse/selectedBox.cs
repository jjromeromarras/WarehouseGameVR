using UnityEngine;

public class selectedBox : MonoBehaviour
{
    // Start is called before the first frame update
    public Material highlighMaterial;
    public bool isSelected;

    private Material originalMaterial;
    private Renderer _render;

    void Start()
    {
       
        _render = GetComponent<Renderer>();
        originalMaterial = _render.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            _render.material = highlighMaterial;
        } else
        {
            _render.material = originalMaterial;
        }
    }
}
