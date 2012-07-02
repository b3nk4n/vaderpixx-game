using System;
using System.IO;

namespace VaderpiXX
{
    class WaveInfo
    {
        public int SpawnsCount;
        public EnemyType Type;

        public WaveInfo(int spawns, EnemyType type)
        {
            this.SpawnsCount = spawns;
            this.Type = type;
        }

        public WaveInfo()
        {

        }

        public void DecrementSpawns()
        {
            this.SpawnsCount -= 1;
        }

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.SpawnsCount = Int32.Parse(reader.ReadLine());
            this.Type = (EnemyType)Enum.Parse(Type.GetType(), reader.ReadLine(), false);
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(SpawnsCount);
            writer.WriteLine(Type);
        }

        #endregion
    }
}
