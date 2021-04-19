using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code;

namespace BuildingTest
{
    public class ScorpionTower : Building
    {
        public static ResourceAmount BuildingCost 
        { 
            get
            {
                ResourceAmount amount = new ResourceAmount();

                amount.Set(FreeResourceType.Gold, 100);
                amount.Set(FreeResourceType.Stone, 50);
                amount.Set(FreeResourceType.Tree, 250);
                amount.Set(FreeResourceType.IronOre, 20);

                return amount;
            }
        }

        public static float AttackTime { get; } = 30f;
        public static float AttackDamage { get; } = 500f;









    }
}
