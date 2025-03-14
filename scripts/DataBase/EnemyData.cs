namespace GameNamespace.DataBase
{
    public class EnemyData
    {
        public int id { get; set; }
        public string name { get; set; }
        public float health { get; set; }
        public int gold { get; set; }
        public float speed { get; set; }
        public string prefab { get; set; }

        /// <summary>
        /// This was a new concept for me.  In my dev window, I'm reading in this dataset and then changing it on the fly.
        /// I learned that I was permanently changing the EnemyData that GameDataBase.Instance.QueryEnemyData was returning.
        /// To get around this, you can make a clone of the object so your not touching the original data in memory.
        /// </summary>
        /// <returns></returns>
        public EnemyData Clone()
        {
            return new EnemyData
            {
                id = this.id,
                name = this.name,
                health = this.health,
                gold = this.gold,
                speed = this.speed,
                prefab = this.prefab
            };
        }
    }




}
