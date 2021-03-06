﻿using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Items;
using SDG.Unturned;
using Steamworks;
using System;
using System.Security.Cryptography.Xml;

namespace OpenMod.Unturned.Players.Equipment.Events
{
    internal class PlayerEquipmentEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerEquipmentEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            OnItemEquipped += Events_OnItemEquipped;
            OnItemUnequipped += Events_OnItemUnequipped;
        }

        public override void Unsubscribe()
        {
            OnItemEquipped -= Events_OnItemEquipped;
            OnItemUnequipped -= Events_OnItemUnequipped;
        }

        public override void SubscribePlayer(Player player)
        {
            player.equipment.onEquipRequested += OnEquipRequested;
            player.equipment.onDequipRequested += OnDequipRequested;
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.equipment.onEquipRequested -= OnEquipRequested;
            player.equipment.onDequipRequested -= OnDequipRequested;
        }

        private void Events_OnItemEquipped(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            Item item = new Item(
                nativePlayer.equipment.itemID, 
                1, 
                nativePlayer.equipment.quality,
                nativePlayer.equipment.state);

            UnturnedPlayerItemEquippedEvent
                @event = new UnturnedPlayerItemEquippedEvent(player, new UnturnedItem(item));

            Emit(@event);
        }

        private void Events_OnItemUnequipped(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemUnequippedEvent @event = new UnturnedPlayerItemUnequippedEvent(player);

            Emit(@event);
        }

        private void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(equipment.player);

            UnturnedPlayerItemEquippingEvent @event = new UnturnedPlayerItemEquippingEvent(player, new UnturnedItem(jar.item));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(equipment.player);

            PlayerInventory inv = player.Player.inventory;

            byte page = equipment.equippedPage;

            byte index = inv.getIndex(page, equipment.equipped_x, equipment.equipped_y);

            ItemJar jar = inv.getItem(page, index);

            if (jar?.item == null) return;

            UnturnedPlayerItemUnequippingEvent @event = new UnturnedPlayerItemUnequippingEvent(player, new UnturnedItem(jar.item));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private delegate void ItemEquipped(Player player);
        private static event ItemEquipped OnItemEquipped;

        private delegate void ItemUnequipped(Player player);
        private static event ItemUnequipped OnItemUnequipped;

        [HarmonyPatch]
        private static class Patches
        {
            // ReSharper disable InconsistentNaming
            [HarmonyPatch(typeof(PlayerEquipment), "tellEquip")]
            [HarmonyPrefix]
            private static void PreTellEquip(PlayerEquipment __instance, out ushort __state)
            {
                __state = __instance.itemID;
            }

            [HarmonyPatch(typeof(PlayerEquipment), "tellEquip")]
            [HarmonyPostfix]
            private static void PostTellEquip(PlayerEquipment __instance, ushort __state, Transform[] ___thirdSlots,
                CSteamID steamID, ushort id)
            {
                if (!__instance.channel.checkServer(steamID)) return;

                if (___thirdSlots == null) return;

                if (__state == 0 && id == 0) return;

                if (__state != 0)
                {
                    OnItemUnequipped?.Invoke(__instance.player);
                }

                if (id != 0 && __instance.asset != null)
                {
                    Type type = Assets.useableTypes.getType(__instance.asset.useable);

                    if (type != null && typeof(Useable).IsAssignableFrom(type))
                    {
                        OnItemEquipped?.Invoke(__instance.player);
                    }
                }
            }
            // ReSharper restore InconsistentNaming
        }
    }
}
