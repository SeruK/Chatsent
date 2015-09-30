// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using System.Collections;
using UnityEngine;

namespace IMVU {
    class FeedPostController : LocalAssetLoader {

        void Start() {
            Imvu.Login().Then(
                userModel => Load(userModel)
            ).Catch(
                error => Debug.Log(error.message)
            );
        }

        void OnGUI() {
            if (GUI.Button(new Rect(10, 10, 100, 20), "Screenshot")) {
                StartCoroutine(PostScreenshot());
            }
        }

        IEnumerator PostScreenshot() {
            yield return new WaitForEndOfFrame();

            var cam = Camera.current;
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            cam.targetTexture = renderTexture;
            cam.Render();
            cam.targetTexture = null;

            Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            screenshot.Apply();
            Debug.Log("Screenshot taken...");

            Imvu.Login().Then(
                userModel => userModel.GetPersonalFeed()
            ).Then(
                feedCollection => {
                    return feedCollection.PostImage(screenshot);
                }
            ).Then(
                _ => Debug.Log("Screenshot posted!")
            ).Catch(
                error => Debug.Log(error.message)
            );
        }
    }
}
