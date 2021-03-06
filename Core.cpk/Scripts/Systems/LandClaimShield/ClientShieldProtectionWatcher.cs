﻿namespace AtomicTorch.CBND.CoreMod.Systems.LandClaimShield
{
    using System;
    using AtomicTorch.CBND.CoreMod.Helpers.Client;
    using AtomicTorch.CBND.CoreMod.StaticObjects.Structures.LandClaim;
    using AtomicTorch.CBND.CoreMod.Systems.LandClaim;
    using AtomicTorch.CBND.CoreMod.Systems.Notifications;
    using AtomicTorch.CBND.CoreMod.UI;
    using AtomicTorch.CBND.CoreMod.UI.Controls.Game.HUD.Notifications;
    using AtomicTorch.CBND.GameApi.Data.Logic;
    using AtomicTorch.CBND.GameApi.Scripting;
    using AtomicTorch.GameEngine.Common.Primitives;
    using JetBrains.Annotations;

    [UsedImplicitly]
    public class ClientShieldProtectionWatcher
    {
        private static HudNotificationControl currentNotification;

        private static HudNotificationControl CreateNotification()
        {
            return NotificationSystem.ClientShowNotification(
                title: CoreStrings.ShieldProtection_NotificationBaseUnderShield_Title,
                // ReSharper disable once CanExtractXamlLocalizableStringCSharp
                message: "placeholder",
                autoHide: false,
                playSound: false,
                onClick: NotificationClickHandler);
        }

        private static ILogicObject GetAreasGroupNearPlayerCharacter()
        {
            var position = ClientCurrentCharacterHelper.Character?.TilePosition ?? Vector2Ushort.Zero;
            return LandClaimSystem.SharedGetLandClaimAreasGroup(position,    addGracePadding: false)
                   ?? LandClaimSystem.SharedGetLandClaimAreasGroup(position, addGracePadding: true);
        }

        private static bool IsOwner(ILogicObject areasGroup)
        {
            if (areasGroup.ClientHasPrivateState)
            {
                return true;
            }

            var areas = LandClaimSystem.ClientGetKnownAreasForGroup(areasGroup);
            foreach (var area in areas)
            {
                if (LandClaimSystem.ClientIsOwnedArea(area))
                {
                    return true;
                }
            }

            return false;
        }

        private static void NotificationClickHandler()
        {
            var areasGroup = GetAreasGroupNearPlayerCharacter();
            if (areasGroup is null
                || !IsOwner(areasGroup))
            {
                return;
            }

            LandClaimShieldProtectionSystem.ClientDeactivateShield(areasGroup);
        }

        private static void Refresh()
        {
            var areasGroup = GetAreasGroupNearPlayerCharacter();
            if (areasGroup is null)
            {
                currentNotification?.Hide(quick: true);
                currentNotification = null;
                return;
            }

            var status = LandClaimAreasGroup.GetPublicState(areasGroup).Status;
            if (status == ShieldProtectionStatus.Inactive)
            {
                currentNotification?.Hide(quick: true);
                currentNotification = null;
                return;
            }

            var isOwner = IsOwner(areasGroup);

            if (currentNotification is null
                || currentNotification.IsHiding)
            {
                currentNotification = CreateNotification();
            }

            var publicState = LandClaimAreasGroup.GetPublicState(areasGroup);
            var time = Api.Client.CurrentGame.ServerFrameTimeApproximated;

            string message, title;
            if (status == ShieldProtectionStatus.Active)
            {
                var timeRemains = publicState.ShieldEstimatedExpirationTime - time;
                title = CoreStrings.ShieldProtection_NotificationBaseUnderShield_Title;

                message = string.Format(CoreStrings.ShieldProtection_NotificationBaseUnderShield_Message_Format,
                                        ClientTimeFormatHelper.FormatTimeDuration(timeRemains, appendSeconds: false));

                if (isOwner)
                {
                    message += "[br][br]"
                               + CoreStrings.ShieldProtection_NotificationBaseUnderShield_MessageOwner;
                }

                message += "[br][br]"
                           + CoreStrings.ShieldProtection_Description_2
                           + "[br]"
                           + CoreStrings.ShieldProtection_Description_3;
            }
            else // activating
            {
                var timeRemains = publicState.ShieldActivationTime - time;
                timeRemains = Math.Max(0, timeRemains);

                title = CoreStrings.ShieldProtection_NotificationBaseActivatingShield_Title;
                message = string.Format(CoreStrings.ShieldProtection_NotificationBaseActivatingShield_Message_Format,
                                        ClientTimeFormatHelper.FormatTimeDuration(timeRemains));

                if (isOwner)
                {
                    message += "[br][br]"
                               + CoreStrings.ShieldProtection_NotificationBaseUnderShield_MessageOwner;
                }
            }

            currentNotification.Title = title;
            currentNotification.Message = message;
        }

        private static void Update()
        {
            // schedule next update
            ClientTimersSystem.AddAction(delaySeconds: 0.5, Update);
            Refresh();
        }

        private class Bootstrapper : BaseBootstrapper
        {
            public override void ClientInitialize()
            {
                ClientTimersSystem.AddAction(delaySeconds: 1, Update);
            }
        }
    }
}