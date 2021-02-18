using Source.Code.Units;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Utils
{
    public class Faction
    {
        public int Index { get; private set; }
        public int Layer { get; private set; }
        public LayerMask EnemiesLayers { get; private set; }


        private Dictionary<Transform, Unit> unitsList; //UnitTransform, Unit

        public Faction(Dictionary<Transform, Unit> unitsList)
        {
            this.unitsList = unitsList;
            foreach (var unit in unitsList)
            {
                unit.Value.Initialize(this);
            }
        }

        public void Initialize(int index)
        {
            Index = index;

            var sessionSettings = SessionSettings.Instance;

            Layer = 20 + index;
            int enemiesLayers = 0;
            for (int i = 0; i < sessionSettings.Factions.Length; i++)
            {
                if (i == index) continue;
                enemiesLayers += 1 << (20 + i);
            }
            EnemiesLayers = enemiesLayers;

            foreach (var unit in unitsList)
            {
                unit.Value.gameObject.layer = Layer;
            }
        }
    }
}