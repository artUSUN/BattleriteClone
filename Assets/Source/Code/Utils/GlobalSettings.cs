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
        [SerializeField] private Colors colors;
        [SerializeField] private RollAbility rollAbility;

        public const string Version = "0.1";
        public InputSettings Input => input;
        public CameraSettigns Camera => camera;
        public LayersSettings Layers => layers;
        public Prefabs Prefabs => prefabs;
        public Colors Colors => colors;
        public RollAbility RollAbility => rollAbility;
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


    [Serializable]
    public class Colors
    {
        [SerializeField] private Color alliasColor = Color.blue;
        [SerializeField] private Color enemyColor = Color.red;
        [SerializeField] private Color ownerColor = Color.green;

        public Color AlliasColor => alliasColor;
        public Color EnemyColor => enemyColor;
        public Color OwnerColor => ownerColor;
    }

    [Serializable]
    public class RollAbility
    {
        [SerializeField] private float speed = 15;
        [SerializeField] private float duration = 0.4f;
        [SerializeField] private float cooldown = 2f;

        public float Speed => speed;
        public float Duration => duration;
        public float Cooldown => cooldown;
    }



    
}