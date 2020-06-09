﻿using System;
using System.Collections.Generic;
using OpenMod.API.Eventing;

namespace OpenMod.Core.Eventing
{
    public abstract class EventBase : IEvent
    {
        public string Name { get; }

        internal EventBase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Event name must not be null or empty");
            }

            Name = name;
        }

        internal EventBase()
        {
            Name = GetType().Name;
        }

        public abstract Dictionary<string, object> Arguments { get; }
    }
}