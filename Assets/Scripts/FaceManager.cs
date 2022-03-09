using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSVhelper;
using System;
using UnityEngine.Video;


public class FaceManager : MonoBehaviour
{    
    public Renderer DisplayMouth;
    public Renderer DisplayEyeR;
    public Renderer DisplayEyeL;

    public Material[] viseme;
    public Material[] blinkingStatesR;
    public Material[] blinkingStatesL;

    //[Range(0f, 1f)]
    private float timeScaleMouth = 0.95f;//0.93f;
    //[Range(0f, 1f)]
    private float timeScaleEye = 0.95f;//0.93f;

    private bool startMouthMovement = false;
    private int currentFrameMouth = 0;
    private int totalFramesMouth = 8;

    private bool startEyeMovementR = false;
    private int currentFrameEyeR = 0;
    private int totalFramesEyeR = 8;

    private bool startEyeMovementL = false;
    private int currentFrameEyeL = 0;
    private int totalFramesEyeL = 8;

    private int[,] sequenceMouth;
    private int[,] sequenceEyeR;
    private int[,] sequenceEyeL;
    private Color color;


    private void Start()
    {
        timeScaleMouth *= 0.0001f;
        timeScaleEye *= 0.0001f;
    }

    public void Play(int[,] mouth, int[,] eye, string emotion)
    {
        StopCoroutine(ChangeMouth());
        StopCoroutine(ChangeEyeR());
        StopCoroutine(ChangeEyeL());

        sequenceMouth = mouth;
        sequenceEyeR = eye;
        sequenceEyeL = eye;
        totalFramesMouth = sequenceMouth.GetLength(0);
        totalFramesEyeR = sequenceEyeR.GetLength(0);
        totalFramesEyeL = sequenceEyeL.GetLength(0);

        if (emotion == "neutral")
            color = new Color(174, 224, 255);
        else if (emotion == "positive")
            color = new Color(212, 214, 102);
        else
            color = new Color(223, 102, 105);

        startMouthMovement = true;
        startEyeMovementR = true;
        startEyeMovementL = true;

        StartCoroutine(ChangeMouth());
        StartCoroutine(ChangeEyeR());
        StartCoroutine(ChangeEyeL());
    }

    IEnumerator ChangeMouth ()
    {        
        while (startMouthMovement & currentFrameMouth < totalFramesMouth)
        {
            currentFrameMouth++;

            DisplayMouth.material = viseme[sequenceMouth[currentFrameMouth, 1]];
            DisplayMouth.material.SetColor("_Color", color * 0.005f);
            DisplayMouth.material.SetColor("_EmissionColor", color * 0);

            int delta = (sequenceMouth[currentFrameMouth, 0] - sequenceMouth[currentFrameMouth - 1, 0]);
            float delay = delta * timeScaleMouth;

            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ChangeEyeR()
    {
        while (startEyeMovementR & currentFrameEyeR < totalFramesEyeR)
        {
            currentFrameEyeR++;

            DisplayEyeR.material = blinkingStatesR[sequenceEyeR[currentFrameEyeR, 1]];
            DisplayEyeR.material.SetColor("_Color", color * 0.005f);
            DisplayEyeR.material.SetColor("_EmissionColor", color * 0);

            int delta = (sequenceEyeR[currentFrameEyeR, 0] - sequenceEyeR[currentFrameEyeR - 1, 0]);
            float delay = delta * timeScaleEye;

            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ChangeEyeL()
    {
        while (startEyeMovementL & currentFrameEyeL < totalFramesEyeL)
        {
            currentFrameEyeL++;

            DisplayEyeL.material = blinkingStatesL[sequenceEyeL[currentFrameEyeL, 1]];
            DisplayEyeL.material.SetColor("_Color", color * 0.005f);
            DisplayEyeL.material.SetColor("_EmissionColor", color * 0);

            int delta = (sequenceEyeL[currentFrameEyeL, 0] - sequenceEyeL[currentFrameEyeL - 1, 0]);
            float delay = delta * timeScaleEye;

            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(0.5f);
    }

    private void Update()
    {
        if (currentFrameMouth == totalFramesMouth - 1)
        {
            startMouthMovement = false;
            StopCoroutine(ChangeMouth());
            currentFrameMouth = 0;
        }

        if (currentFrameEyeR == totalFramesEyeR - 1)
        {
            startEyeMovementR = false;
            StopCoroutine(ChangeEyeR());
            currentFrameEyeR = 0;
        }

        if (currentFrameEyeL == totalFramesEyeL - 1)
        {
            startEyeMovementL = false;
            StopCoroutine(ChangeEyeL());
            currentFrameEyeL = 0;
        }
    }
}