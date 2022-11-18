using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatrixElement : MonoBehaviour, IPointerDownHandler
{
    private Color baseColor;
    [SerializeField] private Color editingColor;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text Text;

    private MatrixGenerator parent;
    private Vector2Int pos;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Debug.Log($"Clicked {pos}");
        parent.Interacted(this);
    }

    public void SetEditingMode(bool editingMode)
    {
        image.color = editingMode ? editingColor : baseColor;
    }

    public void UpdateElement(string newText)
    {
        Text.text = newText;
    }

    internal string GetValue() => Text.text;

    internal void Setup(MatrixGenerator matrixGenerator, Vector2Int pos)
    {
        baseColor = image.color;
        parent = matrixGenerator;
        this.pos = pos;
        UpdateElement(matrixGenerator.GetFor(pos).ToString());
    }
}