using GameNetcodeStuff;
using UnityEngine;

namespace FairAI.Patches
{
    internal class EnemyAIPatch
    {
        private static void BaseHitEnemy(EnemyAI ai, int force, PlayerControllerB playerWhoHit, bool playHitSFX, int hitID)
        {
            if (ai == null)
            {
                return;
            }

            if (playHitSFX && ai.enemyType.hitBodySFX != null && !ai.isEnemyDead)
            {
                ai.creatureSFX.PlayOneShot(ai.enemyType.hitBodySFX);
                WalkieTalkie.TransmitOneShotAudio(ai.creatureSFX, ai.enemyType.hitBodySFX);
            }

            if (ai.creatureVoice != null)
            {
                ai.creatureVoice.PlayOneShot(ai.enemyType.hitEnemyVoiceSFX);
            }

            if (ai.debugEnemyAI)
            {
                Debug.Log($"Enemy #{ai.thisEnemyIndex} was hit with force of {force}");
            }

            if (playerWhoHit != null)
            {
                Debug.Log($"Client #{playerWhoHit.playerClientId} hit enemy {ai.agent.transform.name} with force of {force}.");
            }
        }

        public static bool BrackenHitEnemyPatch(ref FlowermanAI __instance, int force = 1, PlayerControllerB playerWhoHit = null, bool playHitSFX = false, int hitID = -1)
        {
            BaseHitEnemy(__instance, force, playerWhoHit, playHitSFX, hitID);
            if (__instance.isEnemyDead)
            {
                return false;
            }

            __instance.enemyHP -= force;
            if (__instance.IsOwner)
            {
                if (__instance.enemyHP <= 0)
                {
                    __instance.KillEnemyOnOwnerClient();
                    return false;
                }

                if (!playerWhoHit)
                {
                    Plugin.logger.LogDebug("Bracken was hit by another enemy. Not changing its anger meter");
                    return false;
                }

                __instance.angerMeter = 11f;
                __instance.angerCheckInterval = 1f;
                __instance.AddToAngerMeter(0.1f);
            }

            return false;
        }
        /*
        public static void DoAIIntervalPatch(ref EnemyAI __instance)
        {
            if (__instance is ForestGiantAI)
            {
                ForestGiantAI giant = (ForestGiantAI)__instance;
                if (StartOfRound.Instance.livingPlayers == 0 || giant.isEnemyDead)
                {
                    return;
                }

                switch (giant.currentBehaviourStateIndex)
                {
                    case 1:
                        giant.investigating = false;
                        giant.hasBegunInvestigating = false;
                        if (giant.roamPlanet.inProgress)
                        {
                            giant.StopSearch(giant.roamPlanet, clear: false);
                        }

                        if (giant.lostPlayerInChase)
                        {
                            if (!giant.searchForPlayers.inProgress)
                            {
                                Debug.Log("Forest giant starting search for players routine");
                                giant.searchForPlayers.searchWidth = 25f;
                                giant.StartSearch(giant.lastSeenPlayerPositionInChase, giant.searchForPlayers);
                                Debug.Log("Lost player in chase; beginning search where the player was last seen");
                            }
                        }
                        else
                        {
                            if (giant.searchForPlayers.inProgress)
                            {
                                giant.StopSearch(giant.searchForPlayers);
                                Debug.Log("Found player during chase; stopping search coroutine and moving after target player");
                            }

                            giant.SetMovingTowardsTargetPlayer(giant.chasingPlayer);
                        }
                        break;
                }
            }
        }
        public static bool OnCollideWithEnemyPatch(ref EnemyAI __instance, Collider other)
        {
            if (__instance is ForestGiantAI)
            {
                System.Type typ = typeof(ForestGiantAI);
                ForestGiantAI giant = (ForestGiantAI)__instance;
                FieldInfo eat_anim_type = typ.GetField("inEatingPlayerAnimation", BindingFlags.NonPublic | BindingFlags.Instance);
                bool eat_anim_value = (bool)eat_anim_type.GetValue(giant);
                //eat_anim_type.SetValue(__instance, TurretMode.Detection);
                if (giant.inSpecialAnimationWithPlayer != null || eat_anim_value || giant.stunNormalizedTimer >= 0f)
                {
                    return false;
                }
                PlayerControllerB component = other.gameObject.GetComponent<PlayerControllerB>();
                if (!(component != null) || !(component == GameNetworkManager.Instance.localPlayerController))
                {
                    return false;
                }
                Vector3 vector = Vector3.Normalize((giant.centerPosition.position - (GameNetworkManager.Instance.localPlayerController.transform.position + Vector3.up * 1.5f)) * 1000f);
                if (!Physics.Linecast(giant.centerPosition.position + vector * 1.7f, GameNetworkManager.Instance.localPlayerController.transform.position + Vector3.up * 1.5f, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore) && ((!StartOfRound.Instance.shipIsLeaving && StartOfRound.Instance.shipHasLanded) || !GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom) && !(component.inAnimationWithEnemy != null))
                {
                    if (component.inSpecialInteractAnimation && component.currentTriggerInAnimationWith != null)
                    {
                        component.currentTriggerInAnimationWith.CancelAnimationExternally();
                    }
                    FieldInfo chase_touch_type = typ.GetField("triggerChaseByTouchingDebounce", BindingFlags.NonPublic | BindingFlags.Instance);
                    bool chase_touch_value = (bool)chase_touch_type.GetValue(giant);
                    if (giant.currentBehaviourStateIndex == 0 && !chase_touch_value)
                    {
                        chase_touch_type.SetValue(giant, true);
                        //giant.triggerChaseByTouchingDebounce = true;
                        MethodInfo target_method = typ.GetMethod("BeginChasingNewPlayerServerRpc", BindingFlags.NonPublic | BindingFlags.Instance);
                        target_method.Invoke(__instance, new object[] { (int)component.playerClientId });
                        //giant.BeginChasingNewPlayerServerRpc((int)component.playerClientId);
                    }
                    else
                    {
                        giant.GrabPlayerServerRpc((int)component.playerClientId);
                    }
                }
                return false;
            }
            return true;
        }
        */
    }
}
