using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatrixGenerator : MonoBehaviour, IPointerDownHandler
{
    public static MatrixGenerator Instance;

    public Transform MatrixHolder;
    public GameObject MatrixElementPrefab;

    private MatrixElement editing;
    private GridLayoutGroup glg;
    private double[,] matrix;

    private void Start()
    {
        if (MatrixElementPrefab == null)
        {
            Debug.LogError("MatrixElementPrefab is null!");
            return;
        }
        Instance = this;
        glg = MatrixHolder.GetComponent<GridLayoutGroup>();

        matrix = GenerateRandomMatrix(4, 4, false);
        Debug.Log(MatrixToS());
        CreateMatrixObject(MatrixHolder);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Debug.Log("Background clicked, stopping editing");
        StopEditing();
    }

    internal void Edited(string arg0)
    {
        editing?.UpdateElement(arg0);
    }

    internal double GetFor(Vector2Int pos) => matrix[pos.y, pos.x];

    internal void Interacted(MatrixElement matrixElement)
    {
        if (editing != null && editing != matrixElement)
            editing.SetEditingMode(false);

        editing = matrixElement;
        editing.SetEditingMode(true);
        EditorManager.Instance.StartEdit(editing.GetValue());
    }

    private string MatrixToS()
    {
        string x = "[";
        for (int yIdx = 0; yIdx < matrix.GetLength(0); yIdx++)
        {
            for (int xIdx = 0; xIdx < matrix.GetLength(1); xIdx++)
            {
                x += $"{matrix[yIdx, xIdx]} {(xIdx == matrix.GetLength(1) ? "" : ", ")}";
            }
            x += "\n";
        }
        x += "]";
        return x;
    }

    private void StopEditing()
    {
        editing?.SetEditingMode(false);
        editing = null;
        EditorManager.Instance.EndEdit();
    }

    #region Matrix Generation

    private void CreateMatrixObject(Transform matrixHolder)
    {
        glg.constraintCount = matrix.GetLength(0);
        for (int xIdx = 0; xIdx < matrix.GetLength(1); xIdx++)
        {
            for (int yIdx = 0; yIdx < matrix.GetLength(0); yIdx++)
            {
                GameObject me = Instantiate(MatrixElementPrefab, matrixHolder);
                MatrixElement meS = me.GetComponent<MatrixElement>();
                meS.Setup(this, new Vector2Int(yIdx, xIdx));
                me.SetActive(true);
            }
        }
    }

    private double[,] GenerateRandomMatrix(int y, int x, bool frac)
    {
        double[,] matrix = new double[y, x];

        for (int yIdx = 0; yIdx < y; yIdx++)
        {
            for (int xIdx = 0; xIdx < x; xIdx++)
            {
                if (frac)
                    matrix[yIdx, xIdx] = UnityEngine.Random.Range(-10f, 10f);
                else
                    matrix[yIdx, xIdx] = UnityEngine.Random.Range(-10, 10);
            }
        }

        return matrix;
    }

    #endregion Matrix Generation
}