using System;
using System.Collections;
using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatrixGenerator : MonoBehaviour
{
    public static MatrixGenerator Instance;

    public Transform MatrixHolder;
    public GameObject MatrixElementPrefab;
    public Transform Congrats;
    public Button CongratsButton;
    public TMP_Text CongratsText;

    public Button poddajesie;
    private string zadanie;

    public TMP_Text wygrane;
    private int wygranec = 0;

    private MatrixElement editing;
    private GridLayoutGroup glg;
    private double[,] matrix;
    private MatrixElement[,] matrixElements;

    private Stack editHistory;

    private struct MatrixEdit
    {
        public double[,] matrix;
        public string edit;
    }

    private void Start()
    {
        if (MatrixElementPrefab == null)
        {
            Debug.LogError("MatrixElementPrefab is null!");
            return;
        }
        Instance = this;
        glg = MatrixHolder.GetComponent<GridLayoutGroup>();
        editHistory = new Stack();
        CongratsButton.onClick.AddListener(CongratsClicked);
        poddajesie.onClick.AddListener(PoddajeSieClicked);

        NewGame();
    }

    private void NewGame()
    {
        foreach (Transform child in MatrixHolder)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("new game");
        matrix = GenerateRandomMatrix(4, 4, false);
        CreateMatrixObject(MatrixHolder);
        Debug.Log(MatrixToS());

        int zero = UnityEngine.Random.Range(1, 4);

        //Zerowanie kolumny/wiersza
        bool alterCols = UnityEngine.Random.Range(0, 1) == 1;
        if (alterCols) //cols
        {
            AlterMatrix($"K{zero}-K{zero}");
        }
        else //rows
        {
            AlterMatrix($"W{zero}-W{zero}");
        }
        //

        //Uzupe³nianie jednego skrajnego miejsca liczb¹
        if (UnityEngine.Random.Range(0, 1) == 1)
        {
            if (alterCols)
                SetAt(new Vector2Int(zero - 1, 0), UnityEngine.Random.Range(-10, 10));
            else
                SetAt(new Vector2Int(0, zero - 1), UnityEngine.Random.Range(-10, 10));
        }
        else
        {
            if (alterCols)
                SetAt(new Vector2Int(zero - 1, matrix.GetLength(1) - 1), UnityEngine.Random.Range(-10, 10));
            else
                SetAt(new Vector2Int(matrix.GetLength(0) - 1, zero - 1), UnityEngine.Random.Range(-10, 10));
        }
        //

        //Operacje
        for (int a = 0; a < 1; a++) //hack
        {
            //hack

            int x2 = UnityEngine.Random.Range(1, 4);
            while (x2 == zero)
                x2 = UnityEngine.Random.Range(1, 4);
            //

            int m1 = 1;

            if (true) //harder
            {
                m1 = UnityEngine.Random.Range(1, 4);
                //int m2 = UnityEngine.Random.Range(1, 3);
            }

            string z;
            if (UnityEngine.Random.Range(0, 1) == 1) //cols
            {
                z = $"K{zero}{(UnityEngine.Random.Range(0, 1) == 1 ? "+" : "-")}{m1}K{x2}";
                zadanie = $"K{zero}{(UnityEngine.Random.Range(0, 1) == 1 ? "-" : "+")}{m1}K{x2}";
            }
            else //rows
            {
                z = $"W{zero}{(UnityEngine.Random.Range(0, 1) == 1 ? "+" : "-")}{m1}W{x2}";
                zadanie = $"W{zero}{(UnityEngine.Random.Range(0, 1) == 1 ? "-" : "+")}{m1}W{x2}";
            }
            AlterMatrix(z);
        }

        EditorManager.Instance.StartEdit("");
    }

    private void StopEditing()
    {
        editing?.SetEditingMode(false);
        editing = null;
        EditorManager.Instance.EndEdit();
    }

    private void UndoEdit()
    {
        var edit = (MatrixEdit)editHistory.Pop();
        matrix = edit.matrix;
        RefreshElements();
        Debug.Log($"Undid {edit.edit}\n{MatrixToS()}");
    }

    public void EditSubmitted(string arg)
    {
        editHistory.Push(new MatrixEdit
        {
            matrix = (double[,])matrix.Clone(),
            edit = arg
        }
        );
        AlterMatrix(arg, true);

        if (CheckWin())
        {
            Debug.Log("Wygrana G");
            StopEditing();
            wygranec++;
            wygrane.text = "Rozwi¹zane zadania: " + wygranec;

            CongratsText.text = "Gratulacje! Uda³o Ci siê rozwi¹zaæ zadanie! :)";
            Congrats.gameObject.SetActive(true);
        }
    }

    private void PoddajeSieClicked()
    {
        StopEditing();
        CongratsText.text = $"OdpowiedŸ: {zadanie}";
        Congrats.gameObject.SetActive(true);
    }

    private void CongratsClicked()
    {
        Congrats.gameObject.SetActive(false);
        NewGame();
    }

    private bool CheckWin()
    {
        for (int col = 0; col < matrix.GetLength(0); col++)
        {
            int w = 0;
            for (int row = 0; row < matrix.GetLength(1); row++)
            {
                if (GetAt(new Vector2Int(col, row)) == 0)
                    w++;
                else
                    w = 0;

                if (w == 3)
                    return true;
            }
        }
        for (int row = 0; row < matrix.GetLength(1); row++)
        {
            int w = 0;
            for (int col = 0; col < matrix.GetLength(0); col++)
            {
                if (GetAt(new Vector2Int(col, row)) == 0)
                    w++;
                else
                    w = 0;

                if (w == 3)
                    return true;
            }
        }

        return false;
    }

    private void AlterMatrix(string arg, bool userInput = false)
    {
        Debug.Log($"Altering matrix: {arg}\n{MatrixToS()}");
        void AlterColumn(int col1, double mul1, int col2, double mul2)
        {
            for (int yIdx = 0; yIdx < matrix.GetLength(1); yIdx++)
            {
                Vector2Int pos = new Vector2Int(col1, yIdx);
                double col1Val = mul1 * GetAt(new Vector2Int(col1, yIdx));
                double col2Val = mul2 * GetAt(new Vector2Int(col2, yIdx));
                SetAt(pos, col1Val + col2Val);
                matrixElements[pos.x, pos.y].UpdateElement();
                if (userInput)
                {
                    LeanTween.cancel(matrixElements[pos.x, pos.y].gameObject);
                    LeanTween.moveLocalY(matrixElements[pos.x, pos.y].gameObject, matrixElements[pos.x, pos.y].transform.localPosition.y - 5f, .5f).setEaseShake();
                }
            }
        }

        void AlterRow(int row1, double mul1, int row2, double mul2)
        {
            for (int xIdx = 0; xIdx < matrix.GetLength(0); xIdx++)
            {
                Vector2Int pos = new Vector2Int(xIdx, row1);
                double row1Val = mul1 * GetAt(new Vector2Int(xIdx, row1));
                double row2Val = mul2 * GetAt(new Vector2Int(xIdx, row2));
                SetAt(pos, row1Val + row2Val);
                matrixElements[pos.x, pos.y].UpdateElement();
                if (userInput)
                {
                    LeanTween.cancel(matrixElements[pos.x, pos.y].gameObject);
                    LeanTween.moveLocalY(matrixElements[pos.x, pos.y].gameObject, matrixElements[pos.x, pos.y].transform.localPosition.y - 5f, .5f).setEaseShake();
                }
            }
        }

        string editRegex = @"=?([\-|\+]*)([0-9]*\.*[0-9]*)(W|K)([1-9]+)(\-|\+)([0-9]*\.*[0-9]*)(W|K)([1-9]+)";
        var match = Regex.Match(arg, editRegex);
        if (match.Success)
        {
            string op1 = match.Groups[1].Value;
            double mul1 = double.Parse(match.Groups[2].Value == "" ? "1.0" : match.Groups[2].Value, CultureInfo.InvariantCulture);
            string n1 = match.Groups[3].Value;
            int num1 = int.Parse(match.Groups[4].Value);

            string op2 = match.Groups[5].Value;

            double mul2 = double.Parse(match.Groups[6].Value == "" ? "1.0" : match.Groups[6].Value, CultureInfo.InvariantCulture);
            string n2 = match.Groups[7].Value;
            int num2 = int.Parse(match.Groups[8].Value);

            if (n1 != n2)
            {
                //TODO: info
                Debug.Log("kolumny wiersze");
                return;
            }

            if (op1 == "-")
                mul1 = -mul1;

            if (op2 == "-")
                mul2 = -mul2;

            if (n1 == "W")
                AlterRow(num1 - 1, mul1, num2 - 1, mul2);
            else
                AlterColumn(num1 - 1, mul1, num2 - 1, mul2);
            Debug.Log($"Altered matrix:\n{MatrixToS()}");
        }
        else
        {
            Debug.LogWarning("Matrix not altered!");
        }
    }

    #region Matrix Managing

    private void RefreshElements()
    {
        for (int xIdx = 0; xIdx < matrix.GetLength(0); xIdx++)
        {
            for (int yIdx = 0; yIdx < matrix.GetLength(1); yIdx++)
            {
                matrixElements[xIdx, yIdx].UpdateElement();
            }
        }
    }

    public double GetAt(Vector2Int pos) => matrix[pos.x, pos.y];

    public void SetAt(Vector2Int pos, double val)
    {
        Debug.Log($"{pos} -> {val}");
        matrix[pos.x, pos.y] = val;
    }

    private string MatrixToS()
    {
        string x = "[";
        for (int yIdx = 0; yIdx < matrix.GetLength(1); yIdx++)
        {
            for (int xIdx = 0; xIdx < matrix.GetLength(0); xIdx++)
            {
                x += $"{matrix[xIdx, yIdx]} {(xIdx == matrix.GetLength(0) ? "" : ", ")}";
            }
            x += "\n";
        }
        x += "]";
        return x;
    }

    #endregion Matrix Managing

    #region Matrix Generation

    private void CreateMatrixObject(Transform matrixHolder)
    {
        matrixElements = new MatrixElement[matrix.GetLength(0), matrix.GetLength(1)];
        glg.constraintCount = matrix.GetLength(1); //columns
        for (int yIdx = 0; yIdx < matrix.GetLength(1); yIdx++)
        {
            for (int xIdx = 0; xIdx < matrix.GetLength(0); xIdx++)
            {
                GameObject me = Instantiate(MatrixElementPrefab, matrixHolder);
                MatrixElement meS = me.GetComponent<MatrixElement>();
                meS.Setup(this, new Vector2Int(xIdx, yIdx));
                me.SetActive(true);
                matrixElements[xIdx, yIdx] = meS;
            }
        }
    }

    private double[,] GenerateRandomMatrix(int y, int x, bool frac)
    {
        double[,] matrix = new double[x, y];

        for (int yIdx = 0; yIdx < y; yIdx++)
        {
            for (int xIdx = 0; xIdx < x; xIdx++)
            {
                if (frac)
                    matrix[xIdx, yIdx] = UnityEngine.Random.Range(-10f, 10f);
                else
                    matrix[xIdx, yIdx] = UnityEngine.Random.Range(-10, 10);
            }
        }

        return matrix;
    }

    #endregion Matrix Generation
}