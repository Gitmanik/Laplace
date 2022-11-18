using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatrixElement : MonoBehaviour
{
    private Color baseColor;
    [SerializeField] private Color editingColor;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text Text;

    private MatrixGenerator parent;
    private Vector2Int pos;

    public void SetEditingMode(bool editingMode)
    {
        image.color = editingMode ? editingColor : baseColor;
    }

    public void UpdateElement()
    {
        Text.text = parent.GetAt(pos).ToString();
    }

    internal string GetValue() => Text.text;

    internal void Setup(MatrixGenerator matrixGenerator, Vector2Int pos)
    {
        baseColor = image.color;
        parent = matrixGenerator;
        this.pos = pos;
        UpdateElement();
    }
}