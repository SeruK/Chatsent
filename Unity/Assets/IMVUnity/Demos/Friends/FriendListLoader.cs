// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IMVU.Demos {
    class FriendListLoader : MonoBehaviour {
        public GameObject avatarPrefab = null;
        public RectTransform listElementPrefab = null;
        public Text RangeLabel = null;
        public Button nextButton = null;
        public Button prevButton = null;
        public RectTransform loadingPanel = null;

        private FriendCollection friends = null;
        private int pageStart;
        private int pageCount;
        private int totalCount;
        private List<RectTransform> listElements = new List<RectTransform>();
        private GameObject avatarObj = null;

        void Start() {
            Imvu.Login().Then(
                userModel => userModel != null ? userModel.GetFriends(10) : new Promise<FriendCollection, Error>(new Error("User refused to login", "FLL-1"))
            ).Then(
                friendCollection => {
                    friendCollection.add += OnAdd;
                    friends = friendCollection;
                    pageStart = 0;
                    pageCount = friendCollection.info.items.Count;
                    totalCount = friendCollection.info.totalCount;

                    BuildList();
                }
            ).Catch(
                err => Debug.Log(err)
            );
        }

        public void OnNext() {
            if (pageStart + pageCount >= totalCount) {
                return;
            }

            nextButton.gameObject.SetActive(false);
            pageStart += pageCount;
            Promise<Unit, Error> pagePromise;
            if (friends.info.items.Count < Mathf.Min(pageStart + pageCount, totalCount)) {
                loadingPanel.gameObject.SetActive(true);
                pagePromise = friends.NextPage();
            } else {
                pagePromise = new Promise<Unit, Error>((Unit)null);
            }
            pagePromise.Then(
                _ => BuildList(),
                err => Debug.Log(err)
            );
        }

        public void OnPrev() {
            if (pageStart - pageCount < 0) {
                return;
            }

            prevButton.gameObject.SetActive(false);
            pageStart -= pageCount;
            BuildList();
        }

        public void OnAdd(UserModel friend) {
            Debug.Log("Adding friend " + friend.info.username);
        }

        public void LoadFriend(UserModel model) {
            if (avatarObj != null) {
                GameObject.Destroy(avatarObj);
            }
            avatarObj = GameObject.Instantiate(avatarPrefab);
            avatarObj.AddComponent<LocalAssetLoader>().Load(model).Catch(
                error => Debug.LogError(error)
            );
        }

        private void BuildList() {
            RangeLabel.text = pageStart + " to " + Mathf.Min(pageStart + pageCount, totalCount) + " of " + totalCount;
            foreach (var elem in listElements) {
                Object.Destroy(elem.gameObject);
            }
            listElements.Clear();

            loadingPanel.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(pageStart + pageCount < totalCount);
            prevButton.gameObject.SetActive(pageStart > 0);

            var page = friends.info.items.GetRange(pageStart, Mathf.Min(pageCount, totalCount - pageStart));
            int friendCount = 0;
            foreach (var friendPromise in page) {
                var listElement = GameObject.Instantiate(listElementPrefab);
                listElements.Add(listElement);
                listElement.SetParent(transform, false);
                var pos = listElement.anchoredPosition;
                pos.y = -(friendCount * 40 + 10);
                listElement.anchoredPosition = pos;
                friendCount++;

                friendPromise.Then(
                    userModel => listElement.GetComponent<FriendListItem>().Setup(userModel, this)
                ).Catch(
                    err => Debug.LogError(err)
                );
            }
        }
    }
}
