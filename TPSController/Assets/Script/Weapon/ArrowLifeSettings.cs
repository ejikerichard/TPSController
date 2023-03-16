using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/ New BulletLifeSettings")]
public class ArrowLifeSettings : ScriptableObject
{
    public List<vBulletLostLife> bulletLostLifeList;
    private bool seedGenerated;


    [System.Serializable]
    public class vBulletLostLife
    {
        public LayerMask layers = 1 << 0;
        public List<string> tags = new List<string>() { "Untagged" };

        public vBulletLostLife()
        {
            layers = 1 << 0;
            tags = new List<string>() { "Untagged" };
        }
    }
}
