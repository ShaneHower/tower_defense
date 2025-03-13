using System.Reflection;

namespace GameNamespace.DataBase
{
    public class ProjectileData
    {
        public int id { get; set; }
        public string name { get; set; }
        public float damage { get; set; }
        public float speed { get; set;}
        public string effect { get; set; }
        public float effectRate { get; set; }
        public float effectDuration { get; set; }
        public string prefab { get; set; }
    }

}
