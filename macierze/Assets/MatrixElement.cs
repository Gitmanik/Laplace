using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatrixElement : MonoBehaviour
{
    public TMP_Text Text;
    private Vector2Int pos;
    private MatrixGenerator parent;

    internal void Setup(MatrixGenerator matrixGenerator, Vector2Int pos)
    {
        parent = matrixGenerator;
        this.pos = pos;
        Text.text = matrixGenerator.GetFor(pos).ToString();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}