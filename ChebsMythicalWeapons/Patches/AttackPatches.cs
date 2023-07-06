using ChebsMythicalWeapons.Items;
using HarmonyLib;

namespace ChebsMythicalWeapons.Patches
{
    [HarmonyPatch(typeof(Attack))]
    public class AttackPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Attack.FireProjectileBurst))]
        static void FireProjectileBurst(Attack __instance)
        {
            // eliminate projectile gravity if configured to do so
            var lastProjectile = __instance.m_weapon.m_lastProjectile;
            if (lastProjectile == null) return;

            if (!ApolloBowItem.RemoveProjectileGravity.Value) return;
            
            if (__instance.m_weapon.m_dropPrefab.name.Equals(ChebsMythicalWeapons.ApolloBow.ItemName))
            {
                lastProjectile.GetComponent<Projectile>().m_gravity = 0;
            }
        }
    }
}