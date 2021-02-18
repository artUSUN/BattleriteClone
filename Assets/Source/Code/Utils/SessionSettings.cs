using Source.Code.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Utils
{
    public class SessionSettings : MonoBehaviour
    {
        public static SessionSettings Instance { get; private set; }

        #region Constructor
        public static SessionSettings New(Transform parent)
        {
            if (Instance != null)
            {
                Debug.LogError("SessionLocalSettings is already created");
                return Instance;
            }

            var newEmpty = new GameObject("Session Local Settings");
            newEmpty.transform.SetParent(parent);
            var newSLS = newEmpty.AddComponent<SessionSettings>();
            Instance = newSLS;
            return newSLS;
        }
        #endregion

        public int CurrentPlayerID { get; private set; } = -1;
        public Unit ControlledUnit { get; private set; }
        public Faction[] Factions { get; private set; }
        public Quaternion CamRotation { get; set; }



        public void InitControlledUnit(Unit unit)
        {
            if (ControlledUnit != null)
            {
                Debug.LogError("Controlled unit is already set", transform);
                return;
            }
            ControlledUnit = unit;
        }

        public void InitPlayerID(int playerID)
        {
            if (CurrentPlayerID != -1)
            {
                Debug.LogError("Player ID is already set", transform);
                return;
            }
            CurrentPlayerID = playerID;
        }

        public void InitFactions(Faction[] factions)
        {
            if (Factions != null)
            {
                Debug.LogError("Faction is already initialized", transform);
                return;
            }

            Factions = factions;

            for (int i = 0; i < Factions.Length; i++)
            {
                Factions[i].Initialize(i);
            }
        }
    }
}
