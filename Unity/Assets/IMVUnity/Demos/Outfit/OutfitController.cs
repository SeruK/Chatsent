// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;
using UnityEngine.UI;
using IMVU;

public class OutfitController : MonoBehaviour {
    public Image imagePrefab;
    public RectTransform scrollRegion;
    public GameObject avatarPrefab;
    private GameObject avatar;

    void Start() {
        Imvu.Login().Then(
            userModel => userModel.GetOutfits()
        ).Then(
            outfitCollection => {
                foreach (var outfit in outfitCollection.info.items) {
                    Image image = GameObject.Instantiate(imagePrefab);
                    image.rectTransform.SetParent(scrollRegion);
                    outfit.Then(
                        outfitModel => ServiceProvider.Get().textureLoader.Load(outfitModel.info.image).Then(
                            texture => {
                                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100f);
                                image.GetComponent<LayoutElement>().minHeight = (texture.height / (float)texture.width) * 130;
                                image.GetComponent<Button>().onClick.AddListener(
                                    () => {
                                        var obj = GameObject.Instantiate(avatarPrefab);
                                        obj.name = "Outfit " + outfitModel.info.name;
                                        obj.SetActive(false);
                                        obj.AddComponent<LocalAssetLoader>().Load(outfitModel).Then(
                                            _ => {
                                                if (avatar != null) {
                                                    GameObject.Destroy(avatar);
                                                }
                                                avatar = obj;
                                                avatar.SetActive(true);
                                            }
                                        );
                                    }
                                );
                            }
                        )
                    ).Catch(
                        err => Debug.LogError(err)
                    );
                }
            }
        );
    }
}
