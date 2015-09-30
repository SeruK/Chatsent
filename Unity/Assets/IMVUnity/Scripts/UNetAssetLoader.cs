// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace IMVU {
    using LodSettings = List<LodSetting>;

    [RequireComponent(typeof(LocalAssetLoader))]
    public class UNetAssetLoader : NetworkBehaviour {
        [SyncVar(hook = "OnAssetUrl")]
        public string assetUrl;

        void Start() {
            if (assetUrl != null && assetUrl != "") {
                OnAssetUrl(assetUrl);
            }
        }

        [Command]
        public void CmdAssetUrl(string assetUrl) {
            this.assetUrl = assetUrl;
        }

        private void OnAssetUrl(string url) {
            if (!isLocalPlayer) {
                Load(new Uri(url)).Then(_ => OnLoaded());
            }
        }

        public Promise<AssetInfo, Error> Load(Uri url, LodSettings lodSettings = null) {
            if (isLocalPlayer) {
                CmdAssetUrl(url.ToString());
            }
            var loadPromise = GetComponent<LocalAssetLoader>().Load(url, lodSettings);
            loadPromise.Then(_ => OnLoaded());
            return loadPromise;
        }

        public Promise<AssetInfo, Error> Load(UserModel userModel, LodSettings lodSettings = null) {
            return userModel.GetAvatar().Then(
                avatarModel => Load(avatarModel.info.assetUrl)
            );
        }

        public Promise<AssetInfo, Error> Load(OutfitModel outfitModel, LodSettings lodSettings = null) {
            return Look.Get(outfitModel).Then(
                look => Load(look.assetUrl)
            );
        }

        public virtual void OnLoaded() { }
    }
}
