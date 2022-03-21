using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using TMPro;

public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;

    private static event Action<string> OnMessage;

    public override void OnStartAuthority()
    {
        chatUI.SetActive(true);

        OnMessage += HandleNewMessage;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!hasAuthority)
            return;

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message)
    {
        if (message != "")
            chatText.text += message;
        else
            chatText.text = string.Empty;
    }

    [Client]
    public void Send(string message)
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;

        if (string.IsNullOrWhiteSpace(message))
            return;

        CmdSendMessage(inputField.text);
        
        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        RpcHandleMessage($"[{connectionToClient.identity.gameObject.GetComponent<NetworkGamePlayerLobby>().displayName}]: {message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }

    [Server]
    public void ServerSend(string message)
    {
        RpcHandleMessage($"{message}");
    }

    [Server]
    public void ServerClear()
    {
        RpcClear();
    }

    [ClientRpc]
    private void RpcClear()
    {
        OnMessage?.Invoke($"");
    }
}
