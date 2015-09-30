// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;
using UnityEngine.UI;

namespace IMVU {
    public class FeedLoader : MonoBehaviour {
        public RectTransform textPrefab;
        public RectTransform imagePrefab;

        void Start() {
            Imvu.Login().Then(
                userModel => userModel.GetSubscribedFeed()
            ).Then(
                feedElementCollection => Promise<FeedElementModel, Error>.All(feedElementCollection.info.items)
            ).Then(
                feedElementModels => {
                    foreach (var element in feedElementModels) {
                        var info = element.info;
                        var elementUI = GameObject.Instantiate(info.type == "text" ? textPrefab : imagePrefab);
                        elementUI.SetParent(transform, false);
                        elementUI.Find("Message").GetComponent<Text>().text = info.payload.message;
                        elementUI.Find("Date").GetComponent<Text>().text = info.time;
                        if (info.payload.url != null) 
                        {
                            ServiceProvider.Get().textureLoader.Load(info.payload.url).Then(
                            image => elementUI.Find("Image").GetComponent<Image>().sprite = Sprite.Create(image, new Rect(0,0,image.width,image.height),Vector2.zero,1f),
                            err => Debug.Log(err)
                            );
                        }
                        element.GetUser().Then(
                            userModel => elementUI.Find("Name").GetComponent<Text>().text = userModel.info.username,
                            error => Debug.Log("Failed to load user for feed element: " + error.message)
                        );
                    }
                }
            ).Catch(
                error => Debug.Log("Failed to load feed: " + error.message)
            );
        }
    }
}
