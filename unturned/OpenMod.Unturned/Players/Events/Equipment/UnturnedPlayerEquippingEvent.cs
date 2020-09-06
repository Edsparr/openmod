﻿using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Items;

namespace OpenMod.Unturned.Players.Events.Equipment
{
    public abstract class UnturnedPlayerEquippingEvent : UnturnedPlayerEvent, IPlayerItemEquippingEvent
    {
        public UnturnedItem Item { get; }

        IItem IItemEvent.Item => Item;

        public bool IsCancelled { get; set; }

        protected UnturnedPlayerEquippingEvent(UnturnedPlayer player, UnturnedItem item) : base(player)
        {
            Item = item;
        }
    }
}