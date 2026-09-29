// HazardClassifier.cs
//
// Starter script for running your Teachable Machine model (model.tflite +
// labels.txt, placed in Assets/StreamingAssets/) against camera frames.
//
// This won't compile until:
//   1. This folder is opened as a real project in Unity Hub/Editor at
//      least once (generates the binary ProjectSettings files this repo
//      doesn't include)
//   2. A TFLite-for-Unity plugin is installed via Package Manager
//      (e.g. https://github.com/asus4/tf-lite-unity-sample)
//   3. model.tflite and labels.txt are placed in Assets/StreamingAssets/
//
// Method names below match a typical TFLite-for-Unity plugin's API shape —
// check your installed plugin's own sample scene for its exact API, since
// this varies by plugin version.

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using TensorFlowLite;

public class HazardClassifier : MonoBehaviour
{
    [SerializeField] private string modelFileName = "model.tflite";
    [SerializeField] private string labelsFileName = "labels.txt";
    [SerializeField, Range(0f, 1f)] private float confidenceThreshold = 0.7f;

    private Interpreter interpreter;
    private string[] labels;

    void Start()
    {
        var modelPath = Path.Combine(Application.streamingAssetsPath, modelFileName);
        var labelsPath = Path.Combine(Application.streamingAssetsPath, labelsFileName);

        // On Android, StreamingAssets lives inside the compiled APK and
        // can't be read with File.ReadAllBytes directly — use the
        // plugin's FileUtil helper (or UnityWebRequest) instead. Check
        // your installed plugin for the exact loader it ships with.
        interpreter = new Interpreter(FileUtil.LoadFile(modelPath));
        labels = File.ReadAllLines(labelsPath);
    }

    /// <summary>
    /// Runs one classification pass against the given image.
    /// inputImage must already be resized to the model's expected input
    /// size (Teachable Machine models are typically 224x224).
    /// </summary>
    public (string label, float confidence) Classify(Texture2D inputImage)
    {
        float[] output = new float[labels.Length];

        // TODO: set the interpreter's input tensor from inputImage's pixel
        // data before calling Invoke() — the exact call depends on your
        // plugin version (commonly interpreter.SetInputTensorData(0, ...)).

        interpreter.Invoke();
        interpreter.GetOutputTensorData(0, output);

        int bestIndex = Array.IndexOf(output, output.Max());
        return (labels[bestIndex], output[bestIndex]);
    }

    /// <summary>
    /// Only treat a classification as a real hazard flag above the
    /// confidence threshold — see the conversation notes on why a raw
    /// top-guess without a threshold isn't trustworthy enough for a
    /// safety-framed feature.
    /// </summary>
    public bool IsConfidentHazard(string label, float confidence, string hazardLabel)
    {
        return label == hazardLabel && confidence >= confidenceThreshold;
    }

    void OnDestroy()
    {
        interpreter?.Dispose();
    }
}
