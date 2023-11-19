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
            var lastProjectile = __instance?.m_weapon?.m_lastProjectile;
            if (lastProjectile == null) return;

            var dropPrefab = __instance.m_weapon?.m_dropPrefab;
            if (dropPrefab != null && dropPrefab.name.Equals(ChebsMythicalWeapons.ApolloBow.ItemName))
            {
                lastProjectile.GetComponent<Projectile>().m_gravity = ApolloBowItem.ProjectileGravity.Value;
            }
        }
    }
}