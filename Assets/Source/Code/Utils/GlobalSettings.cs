﻿using System;
using UnityEngine;

namespace Source.Code.Utils
{
    [CreateAssetMenu(fileName = "Global Settings Preset", menuName = "Global Settigns", order = 99)]
    public class GlobalSettings : ScriptableObject
    {
        [SerializeField] private InputSettings input;
        [SerializeField] private CameraSettigns camera;
        [SerializeField] private LayersSettings layers;
        [SerializeField] private Prefabs prefabs;


        public InputSettings Input => input;
        public CameraSettigns Camera => camera;
        public LayersSettings Layers => layers;
        public Prefabs Prefabs => prefabs;
    }

    [Serializable]
    public class InputSettings
    {
        [SerializeField] private GameObject lookPivotPrefab;
        [SerializeField] private Vector2 maxLookPivotDistance = new Vector2(8.5f, 5);

        public GameObject LookPivotPrefab => lookPivotPrefab;
        //добавить отдельные значения для x и y и привязать к 1920*1080
        public Vector2 MaxLookPivotDistance => maxLookPivotDistance;
    }

    [Serializable]
    public class CameraSettigns
    {
        [SerializeField] private GameObject virtualCameraPrefab;

        public GameObject VirtualCameraPrefab => virtualCameraPrefab;
    }

    [Serializable]
    public class LayersSettings
    {
        [SerializeField] private LayerMask walls;

        public LayerMask Walls => walls;
    }

    [Serializable]
    public class Prefabs
    {
        [SerializeField] private GameObject unitBar;

        public GameObject UnitBar => unitBar;
    }

}