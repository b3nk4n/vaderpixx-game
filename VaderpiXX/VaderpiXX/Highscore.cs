using System;
using System.Text;

namespace VaderpiXX
{
    class Highscore
    {
        private string name;
        private long score;
        private int level;
        public const int MaxNameLength = 12;

        public Highscore()
            : this("UNKNOWN", 0, 1)
        {
        }

        public Highscore(string csvText)
        {
            if (csvText == null)
            {
                this.name = "UNKNOWN";
                this.score = 0;
                this.level = 1;
                return;
            }

            string[] s = csvText.Split(',');

            if (s.Length != 3)
            {
                this.name = "UNKNOWN";
                this.score = 0;
                this.level = 1;
                return;
            }

            this.Name = s[0];
            this.score = Int64.Parse(s[1]);
            this.level = Int32.Parse(s[2]);
        }

        public Highscore(string name, long score, int level)
        {
            this.Name = name;
            this.score = score;
            this.level = level;
        }

        public static string CheckedName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            
            string tmp = name;

            for (int i = 0; i < tmp.Length; i++)
            {
                if (!((tmp[i] >= 48 && tmp[i] <= 57) || (tmp[i] >= 65 && tmp[i] <= 90) || (tmp[i] >= 97 && tmp[i] <= 122)))
                {
                    tmp = tmp.Replace(tmp[i], ' '); ;
                }
            }
            tmp = tmp.Trim();

            return tmp.Substring(0, Math.Min(tmp.Length, MaxNameLength));
        }

        public string Name
        {
            private set
            {
                this.name = Highscore.CheckedName(value);
            }
            get
            {
                return this.name;
            }
        }

        public long Score
        {
            get
            {
                return this.score;
            }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
        }

        public override string ToString()
        {
            return new StringBuilder().Append(this.name)
                                      .Append(',')
                                      .Append(this.score)
                                      .Append(',')
                                      .Append(this.level)
                                      .ToString();
        }

        public string ScoreText
        {
            get
            {
                return string.Format("{0:000000}", score);
            }
        }

        public string LevelText
        {
            get
            {
                return string.Format("{0:00}", level);
            }
        }
    }
}
