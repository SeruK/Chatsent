// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;

namespace IMVU.Demos {
    [System.Serializable]
    public class Bone {

        public string boneName;
        public bool collider = false;
        public float colliderRadius;
        public float colliderHeight;
        public Vector3 colliderCenter;
        public bool rigidbody = false;
        public float mass;
        public bool joint = false;
        public Vector3 axis;
        public float lowBendLimit;
        public float highBendLimit;
        public float twistLimit;
    }
}
