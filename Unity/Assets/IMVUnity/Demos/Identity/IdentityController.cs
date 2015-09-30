// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;

namespace IMVU.Demos {
    public class IdentityController : LocalAssetLoader {
        public GetUserInfo UserInfoUI;
        public Canvas userInfo;
        
        void Start() {
            Imvu.Login().Then(
                userModel => {
                    UserInfoUI.UpdateInfo(userModel);
                    userInfo.enabled = true;
                    return Load(userModel);
                }
            ).Catch(
                err => Debug.LogError(err)
            );
        }
    }
}
