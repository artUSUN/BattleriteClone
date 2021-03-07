using Source.Code.Units;
using System;
using UnityEngine;

namespace Source.Code.Utils
{
    public class SessionSettings : MonoBehaviour
    {
        public static SessionSettings Instance { get; private set; }

        #region Constructor
        public static SessionSettings New(Transform parent, SessionSetup setupSettings)
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
            newSLS.SetupSettings = setupSettings;
            return newSLS;
        }
        #endregion

        #region Local player
        public int CurrentPlayerID { get; private set; } = -1;
        public int CurrentPlayerFactionID { get; private set; } = -1;
        public Unit ControlledUnit { get; private set; }
        #endregion

        #region Factions
        public Faction[] Factions { get; private set; }
        public LayerMask AllFactionLayers
        {
            get
            {
                if (allFactionLayers == default)
                {
                    foreach (var faction in Factions)
                    {
                        allFactionLayers += 1 << faction.Layer;
                    }
                    if (allFactionLayers == default) Debug.LogError("allFactionLayers is default");
                }
                return allFactionLayers;
            }
        }
        #endregion

        public Quaternion CamRotation { get; set; }

        public SessionSetup SetupSettings { get; private set; }
        public GlobalState GlobalState { get; private set; }
        public LocalState LocalState { get; private set; }

        #region Events
        public event Action<Unit> ControlledUnitSet;
        #endregion 

        public void SetControlledUnit(Unit unit)
        {
            if (ControlledUnit != null)
            {
                Debug.LogError("Controlled unit is already set", transform);
                return;
            }
            ControlledUnit = unit;
            ControlledUnitSet?.Invoke(unit);
        }

        public void SetCurrentPlayerData(int playerID, int factionId)
        {
            if (CurrentPlayerID != -1)
            {
                Debug.LogError("Player ID is already set", transform);
                return;
            }
            CurrentPlayerID = playerID;
            CurrentPlayerFactionID = factionId;
        }

        public void SetFactions(Faction[] factions)
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

        public void SetGameStates(GlobalState globalState, LocalState localState)
        {
            if (GlobalState != null)
            {
                Debug.LogError("Game states is already set", transform);
                return;
            }

            GlobalState = globalState;
            LocalState = localState;
        }

        private LayerMask allFactionLayers;
    }
}
