using System;
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

        public const string Version = "0.1";
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
        [SerializeField] private LayerMask ground;

        public LayerMask Walls => walls;
        public LayerMask Ground => ground;
    }

    [Serializable]
    public class Prefabs
    {
        [SerializeField] private GameObject unitBar;
        [SerializeField] private GameObject controlledUnitUnderline;

        public GameObject UnitBar => unitBar;
        public GameObject ControlledUnitUnderline => controlledUnitUnderline;
    }

}