// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;
using UnityEngine.UI;
using System;

namespace IMVU.Demos {
    public class GetUserInfo : MonoBehaviour {
        public Text username;
        public Text gender;
        public Text country;
        public Text state;
        public Image userPic;

        // Update is called once per frame
        public void UpdateInfo(UserModel userModel) {
            if (userModel != null) {
                username.text = "Username: " + userModel.info.username;
                if (userModel.info.gender == "m") {
                    gender.text = "Gender: Male";
                } else if (userModel.info.gender == "f") {
                    gender.text = "Gender: Female";
                } else {
                    gender.text = "Gender: " + userModel.info.gender;
                }
                country.text = "Country: " + userModel.info.country;
                state.text = "State: " + userModel.info.state;

                if (userModel.info.thumbnailUrl != null) {
                    GetUserPic(userModel.info.thumbnailUrl);
                }
            } else {
                username.text = "";
                gender.text = "";
                country.text = "";
                state.text = "";
                userPic.sprite = null;
            }
        }

        void GetUserPic(Uri url) {
            ServiceProvider.Get().http.Get(url).Then(response => {
                Sprite sprite = new Sprite();
                sprite = Sprite.Create(response.bodyTexture, new Rect(0, 0, 140, 180), new Vector2(0, 0), 100.0f);
                userPic.sprite = sprite;
            });
        }
    }
}
