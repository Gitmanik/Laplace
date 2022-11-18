using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatrixGenerator : MonoBehaviour
{
    public GameObject MatrixElementPrefab;

    private GridLayoutGroup glg;

    private double[,] matrix;

    private void Start()
    {
        if (MatrixElementPrefab == null)
        {
            Debug.LogError("MatrixElementPrefab is null!");
            return;
        }
        glg = GetComponent<GridLayoutGroup>();

        matrix = GenerateRandomMatrix(4, 4, false);
        Debug.Log(MatrixToS());
        CreateMatrixObject(transform);
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

    private void CreateMatrixObject(Transform matrixHolder)
    {
        glg.constraintCount = matrix.GetLength(0);
        for (int xIdx = 0; xIdx < matrix.GetLength(1); xIdx++)
        {
            for (int yIdx = 0; yIdx < matrix.GetLength(0); yIdx++)
            {
                GameObject me = Instantiate(MatrixElementPrefab, matrixHolder);
                var meS = me.GetComponent<MatrixElement>();
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

    internal double GetFor(Vector2Int pos) => matrix[pos.y, pos.x];
}