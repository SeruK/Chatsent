// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;

namespace IMVU.Demos {
    public class RagdollController : LocalAssetLoader {

        public Bone[] bones;
        public GameObject ui;

        void Start() {
            Imvu.Login().Then(
                userModel => Load(userModel)
            ).Then(
                _ => ui.SetActive(true)
            ).Catch(
                err => Debug.LogError(err)
            );
        }
        
        public void EnableRagdoll (bool enabled) {
            Animator animator = GetComponent<Animator> ();
            if (animator) {
                if (enabled) {
                    Rigidbody[] rbs = gameObject.GetComponentsInChildren<Rigidbody> ();
                    if (rbs.Length == 0) {
                        AddSkeletonColliders ();
                        AddSkeletonBones ();
                    }
                    animator.enabled = false;
                } else {
                    RemoveSkeletonBones ();
                    animator.enabled = true;
                }
            }
        }

        public void AddSkeletonColliders () {
            foreach (Bone bone in bones) {
                CapsuleCollider collider;

                GameObject go = FindInChildren (gameObject, bone.boneName);
                if (bone.collider) {
                    collider = go.AddComponent<CapsuleCollider> ();
                    collider.radius = bone.colliderRadius;
                    collider.height = bone.colliderHeight;
                    collider.direction = 0;
                    collider.center = bone.colliderCenter;
                }
            }
        }

        public void AddSkeletonBones () {
            foreach (Bone bone in bones) {
                Rigidbody rigidbody;
                CharacterJoint joint;
                
                GameObject go = FindInChildren (gameObject, bone.boneName);
                if (bone.rigidbody) {
                    rigidbody = go.AddComponent<Rigidbody> ();
                    rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
                    rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    rigidbody.mass = bone.mass;
                    rigidbody.isKinematic = false;
                }
                if (bone.joint) {
                    joint = go.AddComponent<CharacterJoint> ();
                    joint.enablePreprocessing = false;
                    joint.axis = bone.axis;
                    
                    SoftJointLimit lowLimit = new SoftJointLimit ();
                    lowLimit.limit = bone.lowBendLimit;
                    joint.lowTwistLimit = lowLimit; // unity calls this twist it's really bend
                    
                    SoftJointLimit highLimit = new SoftJointLimit ();
                    highLimit.limit = bone.highBendLimit;
                    joint.highTwistLimit = highLimit; // unity calls this twist it's really bend
                    
                    SoftJointLimit twistLimit = new SoftJointLimit ();
                    twistLimit.limit = bone.twistLimit;
                    joint.swing1Limit = twistLimit; // the twist is + or - this value
                    
                    joint.swing2Limit = new SoftJointLimit ();
                    joint.enablePreprocessing = true;
                }
            }
            
            // Need to traverse the list again after all the rigidbodies have been made.
            // This avoids relying on list order 
            foreach (Bone bone in bones) { 
                if (bone.joint) {
                    GameObject go = FindInChildren (gameObject, bone.boneName);
                    go.GetComponent<CharacterJoint> ().connectedBody = FindParentRigidbody (go);
                }
            }
        }

        public void RemoveSkeletonBones () {
            CharacterJoint[] joints = GetComponentsInChildren<CharacterJoint> ();
            foreach (CharacterJoint joint in joints) {
                Destroy (joint);
            }
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody> ();
            foreach (Rigidbody rigidbody in rigidbodies) {
                if (rigidbody.gameObject.name != gameObject.name) {
                    Destroy (rigidbody);
                }
            }
        }

        private GameObject FindInChildren (GameObject go, string name) {
            foreach (Transform t in go.GetComponentsInChildren<Transform>()) {
                if (t.name == name) {
                    return t.gameObject;
                }
            }
            
            return null;
        }

        private Rigidbody FindParentRigidbody (GameObject go) {
            GameObject parent = go.transform.parent.gameObject;
            Rigidbody rb = parent.GetComponent<Rigidbody> ();

            if (parent == null) {
                return null;
            } else if (rb == null) {
                rb = FindParentRigidbody (parent);
            }
            return (rb);
        }
    }
}
