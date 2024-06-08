﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SampleGraphSceneDirector : Director
{
    [Header("Scene parameters")]
    //These are instantiated from prefabs linked to Director class
    private Graph graph;

    private PrimerText text1;

    protected override void Awake() {
        base.Awake();
        camRig.Swivel = Quaternion.Euler(16, 0, 0);
        camRig.SwivelOrigin = new Vector3(0, 3, 0);
        camRig.Distance = 11;

        text1 = Instantiate(primerTextPrefab, camRig.transform);
        text1.transform.localScale = Vector3.zero;
    }

    //Define event actions
    IEnumerator Appear() { 
        graph = Instantiate(primerGraphPrefab);
        graph.transform.localPosition = new Vector3(-6, 0, 0);
        graph.transform.localRotation = Quaternion.Euler(0, 0, 0); 
        graph.Initialize(
            xTicStep: 2,
            xMax: 10,
            yMax: 12,
            zTicStep: 5,
            zMin: -5,
            zMax: 5,
            xAxisLength: 3,
            yAxisLength: 3,
            zAxisLength: 2,
            scale: 2, //True length is the axis length times scale. Scale controls thickness
            xAxisLabelPos: "along"
        );
        graph.ScaleUpFromZero();
        yield return new WaitForSeconds(1);

        text1.tmpro.text = "Sample text";
        text1.tmpro.alignment = TextAlignmentOptions.Center;
        text1.transform.localPosition = new Vector3(4.5f, 2, 0);
        text1.SetIntrinsicScale(1f);
        text1.ScaleUpFromZero();
        yield return new WaitForSeconds(1);
    }

    IEnumerator CurveExamples() {
        StartCoroutine(ChangeText(text1, "Curves"));
        yield return new WaitForSeconds(1);
        float ExampleFunctionCurve(float x) => x * x / 10;
        float ExampleFunctionCurve2(float x) => 5 + 4 * Mathf.Sin(x);
        CurveData efc = graph.AddCurve(ExampleFunctionCurve, "EFC");
        efc.DrawLineAnimation(ease: EaseMode.Cubic, duration: 0.5f);
        yield return new WaitForSeconds(1);
        
        //Curves defined by data just fill the x range.
        //So make sure your data and the graph scale/units match.
        List<int> ExampleDataCurve = new List<int>() {
            5, 6, 5, 3, 6, 4
        };
        CurveData edc = graph.AddCurve(ExampleDataCurve, "EDC");
        edc.SetColor(new Color(1, 0, 0, 1));
        edc.DrawLineAnimation(duration: 0.5f);
        yield return new WaitForSeconds(1);

        efc.AnimateToNewCurve(ExampleFunctionCurve2, duration: 0.5f);
        yield return new WaitForSeconds(1);

        //Graphs can change range, but curves don't currently adjust domain properly
        graph.ChangeRangeX(0, 6);
        yield return new WaitForSeconds(1);

        efc.WipeCurveAnimation();
        yield return new WaitForSeconds(1f);
        edc.WipeCurveAnimation();
        yield return new WaitForSeconds(1);
    }

    IEnumerator SurfaceExamples() {
        StartCoroutine(ChangeText(text1, "Surfaces"));
        yield return new WaitForSeconds(1);
        float ExampleSurface(float x, float z) => 4 - x / 10f - z;
        SurfaceData es = graph.AddSurface(ExampleSurface, "ES");
        es.AnimateX(duration: 1);
        yield return new WaitForSeconds(2);

        es.WipeSurfaceX();
        yield return new WaitForSeconds(1);
    }

    IEnumerator PointExample() {
        StartCoroutine(ChangeText(text1, "Points"));
        yield return new WaitForSeconds(1);
        Vector3 pointPos = new Vector3 (2, 5, 0);
        PointController point = graph.pointData.AddPoint(pointPos, "point", scale: 0.3f);
        point.ActivatePoint(duration: 0.5f);

        yield return new WaitForSeconds(1);

        point.MovePoint(new Vector3(6, 3, 4));

        yield return new WaitForSeconds(1);
        point.DeactivatePoint(duration: 0.5f);
        yield return new WaitForSeconds(1);
    }

    IEnumerator StackedAreasExample() {
        //Stacked areas are pretty janky, tbh
        StartCoroutine(ChangeText(text1, "Stacked Areas"));
        yield return new WaitForSeconds(1);
        List<float> func1 = new List<float>() {1f, 2f, 1f, 2f, 1f, 2f};
        List<float> func2 = new List<float>() {1f, 2f, 3f, 4f, 5f, 6f};
        StackedAreaData stackedArea = graph.AddStackedArea();
        stackedArea.SetFunctions(func1, func2);
        var c1 = new Color(0, 0, 1, 0);
        var c2 = new Color(1, 0, 0, 0);
        stackedArea.SetColors(c1, c2);

        stackedArea.AnimateX();
        yield return new WaitForSeconds(2);
    }
    
    IEnumerator Disappear() {
        graph.ScaleDownToZero();
        text1.ScaleDownToZero();
        yield return new WaitForSeconds(1);
    }

    //Example of a custom IEnumerator for kicking off common animations you might not want
    //to wait for during SceneBlock IEnumerators
    IEnumerator ChangeText(PrimerText t, string newString) {
        t.ScaleDownToZero();
        yield return new WaitForSeconds(0.5f);
        t.tmpro.text = newString;
        t.ScaleUpFromZero();
    }
    
    //Construct schedule
    protected override void DefineSchedule() {
        // in this case, I don't care about the exact timings.
        // I just want them to happen one after another, so they 
        // are all flexible.
        new SceneBlock(0f, Appear, flexible: true);
        new SceneBlock(1f, CurveExamples, flexible: true);
        new SceneBlock(2f, SurfaceExamples, flexible: true);
        new SceneBlock(3f, PointExample, flexible: true);
        new SceneBlock(4f, StackedAreasExample, flexible: true);
        new SceneBlock(5f, Disappear);
    }
}
