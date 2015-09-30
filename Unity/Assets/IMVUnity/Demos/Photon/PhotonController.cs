// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using System;
using UnityEngine;

namespace IMVU {
    public class PhotonController : PhotonAssetLoader {
        public override void Start() {
            base.Start();
            if (photonView.isMine) {
                Imvu.Login().Then(
                    userModel => Load(userModel)
                ).Catch(
                    error => Debug.Log(error)
                );
            }
        }
    }
}
