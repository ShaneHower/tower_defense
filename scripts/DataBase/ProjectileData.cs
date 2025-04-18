
namespace GameNamespace.DataBase
{
    public class ProjectileData
    {
        public string id { get; set; }
        public string name { get; set; }
        public float damage { get; set; }
        public float speed { get; set;}
        public string effect { get; set; }
        public float effectRate { get; set; }
        public float effectDuration { get; set; }
        public bool aoe { get; set; }
        public float aoeRange { get; set; }
        public float aoeDamagePerc { get; set; }
        public string prefab { get; set; }
        public string sfx { get; set; }
    }

}
