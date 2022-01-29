using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReflection : MonoBehaviour
{
    public LayerMask HitTarget;

    private LineRenderer lineRenderer;
    private List<Vector3> lineRendererPoints;
    private int ReflectionCount = 0;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRendererPoints = new List<Vector3>();
    }

    void Update()
    {
        //Reset All Colorable Objects
        GameManager.instance.ResetColorableObjects();

        //Update the Light Tracing
        lineRendererPoints.Clear();
        lineRendererPoints.Add(transform.position);
        ReflectionCount = 0;
        TraceLight(transform.position, transform.forward);
        lineRenderer.positionCount = lineRendererPoints.Count;
        for(int i=0; i<lineRendererPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, lineRendererPoints[i]);
        }

        //Update All Colorable Objects
        GameManager.instance.UpdateColorableObjects();
    }

    void TraceLight(Vector3 StartPos, Vector3 Direction)
    {
        ReflectionCount += 1;
        if(ReflectionCount >= 100) {
            lineRendererPoints.Add(StartPos);
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(StartPos, Direction, out hit, Mathf.Infinity, HitTarget))
        {
            lineRendererPoints.Add(hit.point);
            if (hit.collider.gameObject.tag == "Mirror")
                TraceLight(hit.point, Vector3.Reflect(Direction, hit.normal));
            else if (hit.collider.gameObject.tag == "ColorableObject")
                hit.collider.GetComponentInParent<ColorableObject>().isColor = true;
        }
        else
            lineRendererPoints.Add(StartPos + (Direction * 10f));
    }
}
