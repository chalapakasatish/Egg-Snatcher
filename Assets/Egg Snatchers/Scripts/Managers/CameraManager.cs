using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Unity.Netcode;
public class CameraManager : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] private CinemachineTargetGroup targetGroup;
    private bool configured;
    public List<PlayerController> playerControllers = new List<PlayerController>();
    private void Start()
    {
        NetworkManager.OnServerStarted += ServerStartedCallback;
    }

    private void ServerStartedCallback()
    {
        if (!IsServer)
            return;
        NetworkManager.OnClientConnectedCallback += ClientConnectedCallback;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (!IsServer)
            return;
        NetworkManager.OnClientConnectedCallback -= ClientConnectedCallback;
    }
    private void ClientConnectedCallback(ulong clientId)
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        if(playerCount < 2)
        {
            return;
        }
        StorePlayersRpc();
        UpdateCameraTargetGroupRpc();
    }
    [Rpc(SendTo.Everyone)]
    private void UpdateCameraTargetGroupRpc()
    {
        configured = true;
        foreach (PlayerController playerController in playerControllers) 
        {
            float weight = 1;
            if(playerController.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                weight = 10;
            }
            targetGroup.AddMember(playerController.transform, weight, 2);
        }
    }
    [Rpc(SendTo.Everyone)]
    private void StorePlayersRpc()
    {
        PlayerController[] playerControllersArray = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        
        playerControllers = new List<PlayerController>(playerControllersArray);
    }
}
