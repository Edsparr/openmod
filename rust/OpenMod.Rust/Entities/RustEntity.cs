﻿using System;
using System.Numerics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Rust.Players;
using OpenMod.UnityEngine.Extensions;
using OpenMod.UnityEngine.Transforms;
using UVector3 = UnityEngine.Vector3;

namespace OpenMod.Rust.Entities
{
    public class RustEntity : IEntity, IDamageSource
    {
        public BaseEntity Entity { get; }

        public RustEntity(BaseEntity entity)
        {
            Entity = entity;
            Transform = new UnityTransform(entity.transform);
            EntityInstanceId = entity.GetInstanceID().ToString();

            // Rust todo: asset and state impl
            State = new RustEntityState(entity);
            Asset = null;
        }

        public IWorldTransform Transform { get; }

        public IEntityAsset Asset { get; }

        public IEntityState State { get; }

        public string EntityInstanceId { get; protected set; }

        public virtual Task<bool> SetPositionAsync(Vector3 position)
        {
            return SetPositionAsync(position, Transform.Rotation);
        }

        public virtual Task<bool> SetPositionAsync(Vector3 position, Vector3 rotation)
        {
            async UniTask<bool> TeleportTask()
            {
                await UniTask.SwitchToMainThread();

                var entity = Entity;
                var player = entity as BasePlayer;

                var combatEntity = entity as BaseCombatEntity;
                if (combatEntity != null && !combatEntity.IsAlive())
                {
                    return false;
                }

                return DoTeleport(position, rotation);
            }

            return TeleportTask().AsTask();
        }

        protected virtual bool DoTeleport(Vector3 destination, Vector3 rotation)
        {
            throw new NotImplementedException();
        }
    }
}