using System;
using System.Collections.Generic;

namespace Практика_Кошки___мышки
{
    public class Cell
    {
        private byte Life { get; set; } = 5;
        private byte Eat { get; set; } = 10;
        private byte TimeP { get; set; } = 5;
        private byte BornM { get; set; } = 5;
        private List<int> _stats = new List<int> {5};
        private List<bool> _pol = new List<bool> {false};
        private Random rand = new Random();
        private int lastn;
        private int collastn;
        private int[] OTEnemy = new int[3];
        private List<int> OTStats = new List<int>();
        private List<bool> OTPol = new List<bool>();

        public int[] Enemy = new int[3];
        public bool[] PastEnemy = new bool[3];
        public bool[] Prep = new bool[4];
        public int ColE() => Enemy[0] + Enemy[1] + Enemy[2];
        public int TimeColE() => OTEnemy[0] + OTEnemy[1] + OTEnemy[2];
        public void Redact(byte life, byte eat, byte timep, byte bornm)
        {
            Life = life;
            Eat = eat;
            TimeP = timep;
            BornM = bornm;
        }
        public void Copy(Cell cell)
        {
            _stats.Clear();
            _stats.AddRange(cell._stats);
            _pol.Clear();
            _pol.AddRange(cell._pol);
            Enemy[0] = cell.Enemy[0];
            Enemy[1] = cell.Enemy[1];
            Enemy[2] = cell.Enemy[2];
            Prep[0] = cell.Prep[0];
            Prep[1] = cell.Prep[1];
            Prep[2] = cell.Prep[2];
            Prep[3] = cell.Prep[3];
        }
        public void Clear()
        {
            Enemy[0] = 0;
            Enemy[1] = 0;
            Enemy[2] = 0;
            Prep[0] = false;
            Prep[1] = false;
            Prep[2] = false;
            Prep[3] = false;
            _pol.Clear();
            _pol.Add(false);
            _stats.Clear();
            _stats.Add(TimeP);
        }
        public int[] EnemyN (int k)
        {
            if (k < 0 || k >= ColE()) throw new Exception("Нет такого количества существ в клетке!");
            if (k >= Enemy[0] + Enemy[1]) { return new[] {2,0}; }
            var min = 1;
            var i = _stats.Count;
            if (lastn == 0) while (--i > 1) if (_stats[i] > 0 && _stats[i] < _stats[min]) min = i;
            else
            {
                var col = 0;
                while (--i > 1)
                    if (_stats[i] == lastn) if (col++ == collastn) { min = i; break; }
                    else if (_stats[i] > lastn && _stats[i] < _stats[min]) min = i;
            }
            return new[] { _pol[min]?1:0, _stats[min]};
        }
        public void Wait (int s)
        {
            if (s > lastn)
            {
                lastn = s;
                collastn = 1;
            }
            else collastn++;
        }
        public void Add (int e, int s)
        {
            if (e < 0 || e > 2) throw new Exception("Нет такого существа!");
            if (e == 2)
            {
                if (OTEnemy[1] + OTEnemy[0] == 0) OTEnemy[2]++;
                else OTStats[rand.Next(OTStats.Count*100)/100] += Eat;
                return;
            }
            if (Enemy[2] != 0)
            {
                Enemy[2]--;
                PastEnemy[2] = true;
                s += Eat;
            }
            else if (OTEnemy[2] != 0)
            {
                OTEnemy[2]--;
                s += Eat;
            }
            else if (e == 1)
            {
                if (Enemy[0] + OTEnemy[0] != 0 && Enemy[1] + OTEnemy[1] <= Enemy[0] + OTEnemy[0])
                {
                    var p = rand.Next(200) / 100;
                    OTEnemy[p]++;
                    OTStats.Add(Life);
                    OTPol.Add(p == 1);
                }
                else if (rand.Next(500) < 100)
                {
                    Prep[3] = true;
                    _stats[0] = TimeP;
                }
            }
            else if (Enemy[1] + OTEnemy[1] != 0 && Enemy[0] + OTEnemy[0] < Enemy[1] + OTEnemy[1])
            {
                var p = rand.Next(200) / 100;
                OTEnemy[p]++;
                OTStats.Add(Life);
                OTPol.Add(p == 1);
            }
            OTEnemy[e]++;
            OTStats.Add(s);
            OTPol.Add(e == 1);
        }
        public void Add (int e)
        {
            if (e < 0 || e > 2) throw new Exception("Нет такого существа!");
            Enemy[e]++;
            if (e == 2) return;
            _stats.Add(Life);
            _pol.Add(e == 1);
        }
        public void Delete (int e)
        {
            if (e < 0 || e > 2) throw new Exception("Нет такого существа!");
            Enemy[e]--;
            PastEnemy[e] = true;
            if (e == 2) return;
            var min = 1;
            var i = _pol.Count;
            if (lastn == 0) while (--i > 1) if (_stats[i] > 0 && _stats[i] < _stats[min]) min = i;
            else
            {
                var col = 0;
                while (--i > 1)
                    if (_stats[i] == lastn) if (col++ == collastn) { min = i; break; }
                    else if (_stats[i] > lastn && _stats[i] < _stats[min]) min = i;
             }
            _stats.RemoveAt(min);
            _pol.RemoveAt(min);
        }
        public void Del(int e)
        {
            if (e < 0 || e > 2) throw new Exception("Нет такого существа!");
            Enemy[e]--;
            if (e == 2) return;
            _stats.RemoveAt(1);
            _pol.RemoveAt(1);
        }
        public void Time()
        {
            PastEnemy[0] = false;
            PastEnemy[1] = false;
            PastEnemy[2] = false;
            if (OTEnemy[2] != 0)
            {
                Enemy[2] += OTEnemy[2];
                OTEnemy[2] = 0;
            }
            if (OTEnemy[0] + OTEnemy[1] != 0)
            {
                Enemy[0] += OTEnemy[0];
                Enemy[1] += OTEnemy[1];
                OTEnemy[0] = 0;
                OTEnemy[1] = 0;
                _stats.AddRange(OTStats);
                OTStats.Clear();
                _pol.AddRange(OTPol);
                OTPol.Clear();
            }
            int i;
            if (_pol.Count != 1)
            {
                i = _pol.Count;
                while (--i > 0)
                {
                    _stats[i]--;
                    if (_stats[i] > 0) continue;
                    Enemy[_pol[i] ? 1 : 0]--;
                    _stats.RemoveAt(i);
                    _pol.RemoveAt(i);
                }
            }
            else
            {
                i = Enemy[2];
                while (--i >= 0) if (rand.Next(BornM*100) < 100) Enemy[2]++;
            }
            lastn = 0;
            collastn = 0;
        }
        public void TimeP3()
        {
            if (!Prep[3]) return;
            _stats[0]--;
            if (_stats[0] == 0) Prep[3] = false;
        }
        public override string ToString()
        {
            var t = Prep[0]? "#": (Enemy[0] != 0 ? Enemy[0] + "КШ" : "") + (Enemy[1] != 0 ? Enemy[1] + "К" : "")
                + (Enemy[2] != 0 ? Enemy[2] + "М" : (Prep[3] ? "Э" : "") + (Prep[2] ? "#К" : "")) + (Prep[1] ? "#М" : "");
            return t.Length > 5? t.Insert(t.Length/2, "\n"): t;
        }
    }
}