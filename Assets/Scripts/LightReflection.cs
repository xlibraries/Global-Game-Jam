using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReflection : MonoBehaviour
{
    struct LightLine
    {
        public Vector3 a;
        public Vector3 b;
    }

    public LayerMask HitTarget;

    private LineRenderer lineRenderer;
    private List<Vector3> lineRendererPoints;

    private List<LightLine> lines;
    private int ReflectionCount = 0;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lines = new List<LightLine>();

        lineRendererPoints = new List<Vector3>();
    }

    void Update()
    {
        //Reset All Colorable Objects
        GameManager.instance.ResetColorableObjects();

        //Update the Light Tracing
        lineRendererPoints.Clear();
        lines.Clear();
        lineRendererPoints.Add(transform.position);
        ReflectionCount = 0;
        TraceLight(transform.position, transform.forward);
        //lineRenderer.positionCount = lineRendererPoints.Count;
        //for(int i=0; i<lineRendererPoints.Count; i++)
        //{
        //    lineRenderer.SetPosition(i, lineRendererPoints[i]);
        //}

        lineRenderer.positionCount = lines.Count * 3;
        int LineIndex = 0;
        if(Input.GetKeyDown(KeyCode.M))
        {
            foreach (LightLine l in lines)
                Debug.Log("A: " + l.a + " ,B: "+ l.b);
        }
        for(int i=0; i< lineRenderer.positionCount; i+=3)
        {
            lineRenderer.SetPosition(i + 0, lines[LineIndex].a);
            lineRenderer.SetPosition(i + 1, lines[LineIndex].b);
            lineRenderer.SetPosition(i + 2, lines[LineIndex].a);
            LineIndex++;
        }

        //Update All Colorable Objects
        GameManager.instance.UpdateColorableObjects();
    }

    void TraceLight(Vector3 StartPos, Vector3 Direction)
    {
        LightLine l = new LightLine();
        l.a = StartPos;
        ReflectionCount += 1;
        if(ReflectionCount >= 100) {
            lineRendererPoints.Add(StartPos);
            l.b = StartPos;
            lines.Add(l);
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(StartPos, Direction, out hit, Mathf.Infinity, HitTarget))
        {
            lineRendererPoints.Add(hit.point);
            l.b = hit.point;
            lines.Add(l);
            //bool i = false;
            //Vector3 intersection = CalculateIntersection(out i);
            //if (i)
            //{
            //    lines.RemoveAt(lines.Count - 1);
            //    l.b = intersection;
            //    lines.Add(l);
            //    return;
            //}

            if (hit.collider.gameObject.tag == "Mirror")
            {
                Vector3 dir = Vector3.Reflect(Direction, hit.normal);
                dir.y = 0;
                TraceLight(hit.point, dir);
            }
            else if (hit.collider.gameObject.tag == "Splitter")
            {
                Vector3 dir1 = Vector3.Normalize(hit.collider.gameObject.GetComponent<Splitter>().Direction1.position - hit.collider.transform.position);
                Vector3 dir2 = Vector3.Normalize(hit.collider.gameObject.GetComponent<Splitter>().Direction2.position - hit.collider.transform.position);
                dir1.y = 0;
                dir2.y = 0;
                lineRendererPoints[lineRendererPoints.Count - 1] = hit.collider.transform.position;
                Vector3 newStartingPos = new Vector3(hit.collider.transform.position.x, hit.point.y, hit.collider.transform.position.z);
                TraceLight(newStartingPos, dir1);
                TraceLight(newStartingPos, dir2);
            }
            else if (hit.collider.gameObject.tag == "ColorableObject")
                hit.collider.GetComponentInParent<ColorableObject>().isColor = true;
        }
        else
        {
            lineRendererPoints.Add(StartPos + (Direction * 10f));
            l.b = (StartPos + (Direction * 10f));
            lines.Add(l);
            //bool i = false;
            //Vector3 intersection = CalculateIntersection(out i);
            //if (i)
            //{
            //    lines.RemoveAt(lines.Count - 1);
            //    l.b = intersection;
            //    lines.Add(l);
            //    return;
            //}
        }
    }

    public Vector3 CalculateIntersection(out bool intersects)
    {

        LightLine l = lines[lines.Count - 1];
        Vector3 a = l.a;// lineRendererPoints[lineRendererPoints.Count - 1];
        Vector3 b = l.b;// lineRendererPoints[lineRendererPoints.Count - 2];

        //for(int i=0; i<lineRendererPoints.Count-3; i++)
        for(int i=0; i<lines.Count-1; i++)
        {
            LightLine m = lines[i];
            Vector3 c = m.a;// lineRendererPoints[i];
            Vector3 d = m.b;// lineRendererPoints[i+1];

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
            bool connects = false;

            if (0 <= t && t <= 1 && 0 <= u && u <= 1)
                onSegment = true;
            if ((p0 - p1).magnitude <= 0.001f)
                connects = true;

            if (onSegment && connects)
            {
                lineRendererPoints[lineRendererPoints.Count - 1] = p0;
                intersects = true;
                return p0;
            }
        }
        intersects = false;
        return Vector3.zero;
    }
}
