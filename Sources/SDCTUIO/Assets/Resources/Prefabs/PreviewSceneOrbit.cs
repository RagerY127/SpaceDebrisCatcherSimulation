using System;
using UnityEngine;

public class PreviewSceneOrbit : MonoBehaviour
{
    public LineRenderer tiltAxis;
    public LineRenderer ascendingNodeAxis;
    public LineRenderer positionAxis;

    void Start()
    {
        // Seule la position a un affichage qui ne change jamais
        var posLineRenderer = positionAxis;
        
        var posPointCount = 128;
        Vector3[] orbitPoints = new Vector3[posPointCount];
        for (int i = 0; i < posPointCount; i++)
        {
            float ratio = ((float)i / posPointCount) * MathF.PI * 2f;
            orbitPoints[i] = new Vector3(MathF.Cos(ratio) * 1.1f, 0.04f, MathF.Sin(ratio) * 1.1f);
        }

        posLineRenderer.SetPositions(orbitPoints);
        posLineRenderer.positionCount = posPointCount;
    }

    void Update()
    {
        
    }

    public void UpdatePreview()
    {
        
    }
}
