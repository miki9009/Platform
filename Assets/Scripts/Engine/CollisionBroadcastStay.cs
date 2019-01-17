using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Engine
{
    class CollisionBroadcastStay
    {
        public event System.Action<Collision> CollisionEntered;
        void OnCollisionEnter(Collision collision)
        {
            CollisionEntered?.Invoke(collision);
        }
    }
}
