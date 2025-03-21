namespace GameNamespace.DataBase
{
    public class TowerData
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public float radius { get; set; }
        public int gold { get; set; }
        public float attackSpeed { get; set; }
        public string prefab { get; set; }
        public string projectileId { get; set; }
        public string attackModifier { get; set; }
        public int attackModCounter { get; set; }
        public string nextLevelId { get; set; }
    }

}
