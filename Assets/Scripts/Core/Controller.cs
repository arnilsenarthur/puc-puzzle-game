using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.Data;

namespace Game.Core
{
    public class Controller : MonoBehaviour
    {
        public UnityEvent onStart;

        public void Start()
        {
            if(onStart != null)
                onStart.Invoke();
        }
    }
}