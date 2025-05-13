using UnityEngine;

public class AnimalColoringHandler : MonoBehaviour
{
    [SerializeField] private Renderer render;

    void Start() {
        SetColor();
    }

    private void SetColor(){
        bool isFemale = GetComponent<AnimalBehaviour>().IsFemale();
        Color32 maleColor = new (70, 130, 180, 255);
        Color32 femaleColor = new (255, 105, 180, 255);
        Color32 color = isFemale ? femaleColor : maleColor;

        MaterialPropertyBlock block = new();
        block.SetColor("_BaseColor", color); 
        render.SetPropertyBlock(block);
    }
}
