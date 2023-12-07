using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StadiaJungleBoss
{
    internal static class MagmaWormChanges
    {
        public static bool enabled, useStatItem;

        public static ItemDef StatItem;

        private static BodyIndex MagmaWormIndex;

        public static void Initialize()
        {
            if (!enabled) return;
            InitItem();
            RoR2Application.onLoad += GetBodyIndex;

            On.RoR2.CharacterBody.GetDisplayName += ReplaceName;
            On.RoR2.CharacterBody.GetSubtitle += ReplaceSubtitle;
            IL.RoR2.Util.GetBestBodyName += SuppressMendingName;
        }

        private static void SuppressMendingName(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                     x => x.MatchCallvirt(typeof(CharacterBody), "HasBuff")
                    );
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<BuffIndex, CharacterBody, BuffIndex>>((buffIndex, body) =>
            {
                if (buffIndex == DLC1Content.Buffs.EliteEarth.buffIndex && CheckForWorm(body))
                {
                    return BuffIndex.None;
                }
                return buffIndex;
            });
        }

        private static string ReplaceSubtitle(On.RoR2.CharacterBody.orig_GetSubtitle orig, CharacterBody self)
        {
            if (CheckForWorm(self))
            {
                return Language.GetString("STADIAJUNGLEBOSS_BODY_SUBTITLE");
            }
            return orig(self);
        }

        private static string ReplaceName(On.RoR2.CharacterBody.orig_GetDisplayName orig, CharacterBody self)
        {
            if (CheckForWorm(self))
            {
                return Language.GetString("STADIAJUNGLEBOSS_BODY_NAME");
            }
            return orig(self);
        }

        private static void GetBodyIndex()
        {
            MagmaWormIndex = BodyCatalog.FindBodyIndex("MagmaWormBody");
        }

        private static void InitItem()
        {
            if (!useStatItem) return;
            StatItem = ScriptableObject.CreateInstance<ItemDef>();
            StatItem.name = "StadiaBossMagmaWormItem";
            StatItem.deprecatedTier = ItemTier.NoTier;
            StatItem.nameToken = "Stadia Worm";
            StatItem.pickupToken = "Changes the attacks of Magma Worms.";
            StatItem.descriptionToken = "Changes the attacks of Magma Worms.";
            StatItem.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotSteal
            };
            ItemDisplayRule[] idr = new ItemDisplayRule[0];
            //ContentAddition.AddItemDef(OriginBonusItem);
            ItemAPI.Add(new CustomItem(StatItem, idr));
        }

        private static bool CheckForWorm(CharacterBody body)
        {
            if (body.bodyIndex == MagmaWormIndex && body.inventory)
            {
                if (useStatItem)
                {
                    return body.inventory.GetItemCount(StatItem) > 0;
                }
                else
                {
                    if (body.inventory.GetItemCount(RoR2Content.Items.TeleportWhenOob) > 0
                        && body.inventory.GetEquipmentIndex() == StadiaJungleBossPlugin.EliteEarthEquipment.equipmentIndex
                        && SceneManager.GetActiveScene().name == "rootjungle")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
