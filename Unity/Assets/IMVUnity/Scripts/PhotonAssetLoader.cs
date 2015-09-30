// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using System;
using System.Collections.Generic;
using UnityEngine;
using Photon;

namespace IMVU {
    using LodSettings = List<LodSetting>;

    [RequireComponent(typeof(LocalAssetLoader))]
    public class PhotonAssetLoader : PunBehaviour {
        [NonSerialized]
        private LocalAssetLoader assetLoader;

        public virtual void Start() {
            assetLoader = GetComponent<LocalAssetLoader>();
            String assetUrl = photonView.owner.customProperties["assetUrl"] as String;
            if (assetUrl != null && assetUrl.Length > 0) {
                assetLoader.Load(new Uri(assetUrl)).Then(_ => OnLoaded());
            }
        }

        public Promise<AssetInfo, Error> Load(Uri url, LodSettings lodSettings = null) {
            PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "assetUrl", url.ToString() } });
            photonView.RPC("LoadPhoton", PhotonTargets.Others, url.ToString());
            var loadPromise = assetLoader.Load(url, lodSettings);
            loadPromise.Then(_ => OnLoaded());
            return loadPromise;
        }

        public Promise<AssetInfo, Error> Load(UserModel userModel, LodSettings lodSettings = null) {
            return userModel.GetAvatar().Then(
                avatarModel => Load(avatarModel.info.assetUrl, lodSettings: lodSettings)
            );
        }

        public Promise<AssetInfo, Error> Load(OutfitModel outfitModel, LodSettings lodSettings = null) {
            return Look.Get(outfitModel).Then(
                look => Load(look.assetUrl, lodSettings: lodSettings)
            );
        }

        public virtual void OnLoaded() { }

        [PunRPC]
        protected void LoadPhoton(string url) {
            assetLoader.Load(new Uri(url)).Catch(
                 err => Debug.Log(err)
            );
        }
    }
}
