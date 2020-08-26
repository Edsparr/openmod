﻿using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Events.Chat
{
    internal class ChatEventsListener : UnturnedEventsListener
    {
        public ChatEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            ChatManager.onChatted += OnChatted;
            ChatManager.onServerSendingMessage += OnServerSendingMessage;
        }

        public override void Unsubscribe()
        {
            ChatManager.onChatted -= OnChatted;
            ChatManager.onServerSendingMessage -= OnServerSendingMessage;
        }

        private void OnChatted(SteamPlayer steamPlayer, EChatMode mode, ref Color color, ref bool isRich, string text, ref bool isVisible)
        {
            UnturnedPlayer player = GetUnturnedPlayer(steamPlayer);

            UnturnedPlayerChatEvent @event = new UnturnedPlayerChatEvent(player, mode, color, isRich, text);

            Emit(@event);

            color = @event.Color;
            isRich = @event.IsRich;
            isVisible = !@event.IsCancelled;
        }

        private void OnServerSendingMessage(ref string text, ref Color color, SteamPlayer nativeFromPlayer, SteamPlayer nativeToPlayer, EChatMode mode, ref string iconURL, ref bool useRichTextFormatting)
        {
            // If nativeToPlayer is null, this event will be called again for each player
            if (nativeToPlayer == null) return;

            UnturnedPlayer fromPlayer = GetUnturnedPlayer(nativeFromPlayer);
            UnturnedPlayer toPlayer = GetUnturnedPlayer(nativeToPlayer);

            UnturnedServerSendMessageEvent @event = new UnturnedServerSendMessageEvent(fromPlayer, toPlayer, text, color, mode, iconURL, useRichTextFormatting);

            Emit(@event);

            text = @event.Text;
            color = @event.Color;
            iconURL = @event.IconUrl;
            useRichTextFormatting = @event.IsRich;
        }
    }
}
