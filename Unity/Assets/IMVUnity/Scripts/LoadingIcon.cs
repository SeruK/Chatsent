// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;

public class LoadingIcon : MonoBehaviour {
    
    void Start () {
        InvokeRepeating ("RotateIcon", 0, 0.15f);
    }

    void RotateIcon () {
        transform.Rotate (0, 0, -45);
    }
}
