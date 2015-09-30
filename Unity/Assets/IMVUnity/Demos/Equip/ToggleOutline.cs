// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;
using UnityEngine.UI;

namespace IMVU.Demos {
    [RequireComponent (typeof(Outline))]

    public class ToggleOutline : MonoBehaviour {

        public void Toggle () {
            if (GameObject.FindObjectOfType<Animator> () != null) {
                Outline outline = gameObject.GetComponent<Outline> ();
                outline.enabled = (outline.enabled) ? false : true;
            }
        }
    }
}
