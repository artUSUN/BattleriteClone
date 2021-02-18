using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code
{
    public class UIHandler : MonoBehaviour
    {
        [SerializeField] private Slider reloadSlider;

        public static UIHandler Instance { get; private set; }
        public Slider ReloadSlider => reloadSlider;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }
}