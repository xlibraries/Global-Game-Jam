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

            if (CalculateIntersection())
                return;

            if (hit.collider.gameObject.tag == "Mirror")
                TraceLight(hit.point, Vector3.Reflect(Direction, hit.normal));
            else if(hit.collider.gameObject.tag == "Splitter") {
                Vector3 dir1 = Vector3.Normalize(hit.collider.gameObject.GetComponent<Splitter>().Direction1.position - hit.collider.transform.position);
                Vector3 dir2 = Vector3.Normalize(hit.collider.gameObject.GetComponent<Splitter>().Direction2.position - hit.collider.transform.position);
                lineRendererPoints[lineRendererPoints.Count - 1] = hit.collider.transform.position;
                TraceLight(hit.collider.transform.position, dir1);
                TraceLight(hit.collider.transform.position, dir2);
            }
            else if (hit.collider.gameObject.tag == "ColorableObject")
                hit.collider.GetComponentInParent<ColorableObject>().isColor = true;
        }
        else
        {
            lineRendererPoints.Add(StartPos + (Direction * 10f));
            if (CalculateIntersection())
                return;
        }
    }

    public bool CalculateIntersection()
    {
        Vector3 a = lineRendererPoints[lineRendererPoints.Count - 1];
        Vector3 b = lineRendererPoints[lineRendererPoints.Count - 2];

        for(int i=0; i<lineRendererPoints.Count-3; i++)
        {
            Vector3 c = lineRendererPoints[i];
            Vector3 d = lineRendererPoints[i+1];

            Vector3 r = b - a;
            Vector3 s = d - c;
            Vector3 q = a - c;

            float dotqr = Vector3.Dot(q, r);
            float dotqs = Vector3.Dot(q, s);
            float dotrs = Vector3.Dot(r, s);
            float dotrr = Vector3.Dot(r, r);
            float dotss = Vector3.Dot(s, s);

            float denom = dotrr * dotss - dotrs * dotrs;
            float numer = dotqs * dotrs - dotqr * dotss;

            float t = numer / denom;
            float u = (dotqs + t * dotrs) / dotss;

            Vector3 p0 = a + t * r;
            Vector3 p1 = c + u * s;

            bool onSegment = false;
            bool intersects = false;

            if (0 <= t && t <= 1 && 0 <= u && u <= 1)
                onSegment = true;
            if ((p0 - p1).magnitude <= 0.001f)
                intersects = true;

            if (onSegment && intersects)
            {
                lineRendererPoints[lineRendererPoints.Count - 1] = p0;
                return true;
            }
        }

        return false;
    }
}
