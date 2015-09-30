// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using UnityEngine;
using IMVU;

public class FriendAvatarLoader : LocalAssetLoader {
    void Start() {
        Imvu.Login().Then(
            user => user.GetFriends()
        ).Then(
            friends => friends.info.items[Random.Range(0, friends.info.items.Count - 1)]
        ).Then(
            friend => {
                Debug.Log("Loading avatar for " + friend.info.username);
                return Load(friend);
            }
        ).Catch(
            err => Debug.Log(err.message)
        );
    }
}
