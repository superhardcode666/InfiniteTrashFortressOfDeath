﻿using UnityEngine;

namespace Bloodbath
{
    public class SelfDestruct : MonoBehaviour
    {
        public float selfdestruct_in = 4; // Setting this to 0 means no selfdestruct.

        private void Start()
        {
            if (selfdestruct_in != 0) Destroy(gameObject, selfdestruct_in);
        }
    }
}