// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;
using UnityEngine.UI;

namespace IMVU.Demos {
    class FriendListItem : MonoBehaviour {
        public Text text = null;
        private UserModel userModel;
        private FriendListLoader loader;

        public void Setup(UserModel model, FriendListLoader loader) {
            text.text = model.info.username;
            userModel = model;
            this.loader = loader;
        }

        public void OnClick() {
            loader.LoadFriend(userModel);
        }
    }
}
