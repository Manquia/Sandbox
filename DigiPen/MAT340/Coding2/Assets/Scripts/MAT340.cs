using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAT340
{
    public static float[,] RaisePowerMatrix(float[,] matrix, int dimensions, int power)
    {
        Debug.Assert(dimensions * dimensions == matrix.Length, " dimensions and matrix length do not match");

        power -= 1;
        float[,] workingMatrix = matrix; // (float[,])matrix.Clone();
        for (int p = 0; p < power; ++p)
        {
            float[,] matrixOut = new float[dimensions, dimensions];
            for (int i = 0; i < dimensions; ++i)
            {
                for (int j = 0; j < dimensions; ++j)
                {
                    for (int k = 0; k < dimensions; ++k)
                        matrixOut[i, j] += (workingMatrix[i, k] * matrix[k, j]);
                }
            }
            workingMatrix = matrixOut;
        }

        return workingMatrix;
    }
}