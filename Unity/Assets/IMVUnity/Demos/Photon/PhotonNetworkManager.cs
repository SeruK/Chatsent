// Copyright 2015 IMVU, Inc. All Rights Reserved.
// This is unpublished proprietary source code provided to a licensee under commercial terms.
// Permission is granted to re-publish derivative works only under such terms as granted in 
// writing in that license mutually agreed by IMVU, Inc and the licensee.

using System.Collections.Generic;
using UnityEngine;
using IMVU;
using Photon;

public class PhotonNetworkManager : PunBehaviour {
    private string roomName = "PhotonDemo";
    private string playerPrefabName = "PhotonPlayer";
    public List<GameObject> spawnPoints;

    void Start () {
        if (!PhotonNetwork.autoJoinLobby) {
            Debug.LogError("Demo requires Auto-Join Lobby to set in Window->Photon Unity Networking->Highlight Server Settings");
            return;
        }
        Imvu.Login().Then(
            _ => PhotonConnect(),
            err => Debug.LogError(err)
        );
    }

    const string VERSION = "v0.0.1";

    Result<Unit, Error> PhotonConnect() {
        if (PhotonNetwork.ConnectUsingSettings(VERSION)) {
            return Result<Unit, Error>.Ok(null);
        } else {
            return Result<Unit, Error>.Err(new Error("Photon failed to connect.", "PD-01"));
        }
    }

     new void OnJoinedLobby() {
        RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    new void OnJoinedRoom() {
        Transform spawnPoint = spawnPoints[(PhotonNetwork.playerList.Length-1) % spawnPoints.Count].transform;
        PhotonNetwork.Instantiate(playerPrefabName, spawnPoint.position, spawnPoint.rotation, 0);
    }
}
