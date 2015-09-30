// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;
using System.Collections.Generic;

namespace IMVU.Demos {
    public class EquipController : LocalAssetLoader {

        public GameObject item;
        public GameObject ui;
        private AssetInfo assetInfo = null;
        Dictionary<string, GameObject> equipment = new Dictionary<string, GameObject>();

        public void Equip(string boneName) {
            Transform bone = assetInfo.skeletons[0].GetBone(boneName);
            if (bone != null && item != null) {
                GameObject child = null;
                if (equipment.TryGetValue(boneName, out child)) {
                    Destroy(child);
                    equipment.Remove(boneName);
                } else {
                    GameObject go = Instantiate(item) as GameObject;
                    equipment.Add(boneName, go);
                    go.transform.parent = bone;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    switch (boneName) { // hard coded offsets
                    case "lfHand":
                        go.transform.localPosition = new Vector3(-0.15f, 0.1f, -0.05f);
                        go.transform.localRotation = Quaternion.Euler(new Vector3 (90, -90, 0));
                        break;
                    case "rtHand":
                        go.transform.localPosition = new Vector3(-0.15f, -0.1f, -0.05f);
                        go.transform.localRotation = Quaternion.Euler(new Vector3 (-90, 90, 0));
                        break;
                    case "lfThigh":
                        go.transform.localPosition = new Vector3(0.0f, 0.075f, 0.1f);
                        go.transform.localRotation = Quaternion.Euler(new Vector3 (10, 40, 75));
                        break;
                    case "rtThigh":
                        go.transform.localPosition = new Vector3(0.0f, -0.075f, 0.1f);
                        go.transform.localRotation = Quaternion.Euler(new Vector3 (-10, 40, 75));
                        break;
                    }
                }
            }
        }
        
        void Start() {
            Imvu.Login().Then(
                userModel => Load(userModel)
            ).Then(
                assetInfo => {
                    this.assetInfo = assetInfo;
                    ui.SetActive(true);
                }
            ).Catch(
                err => Debug.LogError(err)
            );
        }
    }
}
