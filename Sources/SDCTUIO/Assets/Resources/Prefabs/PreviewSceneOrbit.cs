using System;
using UnityEditor;
using UnityEngine;

public class PreviewSceneOrbit : MonoBehaviour
{
    public LineRenderer tiltAxis;
    public LineRenderer ascendingNodeAxis;
    public LineRenderer positionAxis;
    public GameObject globe;
    public GameObject debris;

    private float tiltAngle;
    private float ascendingNodeAngle;
    private float positionAngle;
    private float distance;
    private DebrisShape shape;

    public float TiltAngle {
        set{
            tiltAngle = value;
            UpdatePreview();
        }
        get => tiltAngle;
    }

    public float AscendingNodeAngle {
        set{
            ascendingNodeAngle = value;
            UpdatePreview();
        }
        get => ascendingNodeAngle;
    }

    public float PositionAngle {
        set{
            positionAngle = value;
            UpdatePreview();
        }
        get => positionAngle;
    }

    public float Distance {
        set{
            distance = value;
            UpdatePreview();
        }
        get => distance;
    }

    public DebrisShape Shape
    {
        set
        {
            shape = value;
            UpdateShape();
        }
        get => shape;
    }

    protected void FillLineRenderer(LineRenderer lineRenderer, float angle, bool z = false)
    {
        lineRenderer.positionCount = 0;

        var pointCount = (int) SDC.Maths.Map(angle, 0f, 360f, 2f, 64f);
        Vector3[] points = new Vector3[pointCount + 1];
        for (int i = 0; i <= pointCount; i++)
        {
            float ratio = angle / pointCount * i * Mathf.Deg2Rad;
            if (z)
                points[i] = new Vector3(MathF.Cos(ratio) * .5f, MathF.Sin(ratio) * .5f, 0f);
            else
                points[i] = new Vector3(MathF.Sin(ratio) * .5f, 0f, MathF.Cos(ratio) * .5f);
        }

        lineRenderer.positionCount = pointCount + 1;
        lineRenderer.SetPositions(points);
    }

    void Start()
    {
        // Seule la position a un affichage qui ne change jamais
        var posLineRenderer = positionAxis;
        FillLineRenderer(posLineRenderer, 360f);
    }

    void Update()
    {
        var posLineRenderer = positionAxis;
        posLineRenderer.transform.localRotation =
            Quaternion.Euler(0f, ascendingNodeAngle, 0f) *
            Quaternion.Euler(tiltAngle, 0f, 0f) * 
            Quaternion.Euler(0f, positionAngle + 180f, 0f);    // position du débris, l'axe est déjà sur le Y local
    }

    public void UpdatePreview()
    {
        // update tilt
        var tiltLineRenderer = tiltAxis;
        FillLineRenderer(tiltLineRenderer, tiltAngle, true);
        tiltLineRenderer.transform.localRotation = Quaternion.Euler(0f, ascendingNodeAngle + 90f, 0f);
        // update ascending node
        FillLineRenderer(ascendingNodeAxis, ascendingNodeAngle);
        ascendingNodeAxis.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        // update distance
        globe.transform.localScale = Vector3.one / MathF.Pow(2f, distance / SimulationManager.EARTH_RADIUS_KM);
    }

    private void UpdateShape()
    {
        string file = "";
        Vector3 scale = Vector3.one / 10;

        switch (Shape)
        {
            case DebrisShape.Cube:
                file = "Cube";
                break;
            case DebrisShape.Cylinder:
                file = "Cylinder";
                scale /= 2;
                break;
        }
        debris.GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>(file + ".fbx");
        debris.transform.localScale = scale;
    }
}
