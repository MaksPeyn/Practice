using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using Практика_Кошки___мышки.Annotations;

namespace Практика_Кошки___мышки
{
    /*Кнопки: Слева можно задать некоторые постоянные
              Добавить! добавляет выбранное количество живых существ на поле в случайном порядке.
              Пошагово! рассчитывает экосистему в реальном времени.
              Поехали! рассчитывает до гибели всех живых существ или бесконечно.
              Один шаг! рассчитывает один ход системы.
              Сохранить! сохраняет поле.
              Загрузить! загружает последнее сохранённое поле.
              Очистить! очищает поле. */
    public class Pole: INotifyPropertyChanged
    {
        private RelayCommand _sizeCommand;
        private RelayCommand _goShagCommand;
        private RelayCommand _goCommand;
        private RelayCommand _genCommand;
        private RelayCommand _saveCommand;
        private RelayCommand _loadCommand;
        private RelayCommand _iterCommand;
        private RelayCommand _clearCommand;
        private RelayCommand _modifyCommand;
        private static Cell[,] _p;
        private static Cell[,] _tp;
        private static Random Och = new Random();
        private int[] Col = new int[3];
        private static List<int> EnemyP = new List<int>();
        private bool _vis;
        private bool _bvis;
        private bool _shag;

        public short CatW { get; set; }
        public short Cat { get; set; }
        public short Mouse { get; set; }
        public byte Life { get; set; } = 5;
        public byte Eat { get; set; } = 10;
        public byte TimeP { get; set; } = 5;
        public byte BornM { get; set; } = 5;
        public short N { get; set; }
        public short M { get; set; }
        public RelayCommand SizeCommand => _sizeCommand ?? (_sizeCommand = new RelayCommand(Size));
        public RelayCommand GenCommand => _genCommand ?? (_genCommand = new RelayCommand(Gener));
        public RelayCommand IterCommand => _iterCommand ?? (_iterCommand = new RelayCommand(OneIter));
        public RelayCommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));
        public RelayCommand LoadCommand => _loadCommand ?? (_loadCommand = new RelayCommand(Load));
        public RelayCommand GoShagCommand => _goShagCommand ?? (_goShagCommand = new RelayCommand(GoShag));
        public RelayCommand GoCommand => _goCommand ?? (_goCommand = new RelayCommand(Go));
        public RelayCommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(Clear));
        public RelayCommand ModifyCommand => _modifyCommand ?? (_modifyCommand = new RelayCommand(Modify));
        public ArrayDataView Adv { get; set; }
        public string Button => (_bvis ? "Пошагово!": "Пру!") + " (" + Col[0] + "КШ, " + Col[1] + "К, " + Col[2] + "М)";
        public bool Vis
        {
            get { return _vis; }
            set
            {
                _vis = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NVis));
            }
        }
        public bool NVis => !_vis;
        public bool BVis
        {
            get { return _bvis; }
            set
            {
                _bvis = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Button));
            }
        }

        private int Mesto (Cell cell)
        {
            return cell.Prep[0] ? 2 : (cell.Enemy[2] != 0? 160 : cell.PastEnemy[2]? 80 : 0) + (cell.Prep[2] || cell.Prep[3] ? 1 : cell.Prep[1] ? 0 : 3) + (cell.Enemy[0] != 0 ? 70 - (cell.Enemy[1] == 1? 10: cell.Enemy[1] > 1? 20: 0) : cell.Enemy[1] != 0? 40 - (cell.PastEnemy[0]? 10: 0): cell.PastEnemy[0]? 20: cell.PastEnemy[1]? 10: 0);
        }

        private void Size(object o)
        {
            if (N < 2) N = 2;
            if (M < 2) M = 2;
            var i = N;
            _p = new Cell[N, M];
            _tp = new Cell[N, M];
            while (--i >= 0)
            {
                var j = M;
                while (--j >= 0)
                {
                    _p[i,j] = new Cell();
                    _tp[i,j] = new Cell();
                }
            }
            Adv = new ArrayDataView(_p);
            OnPropertyChanged(nameof(Adv));
            Vis = true;
            BVis = true;
            _shag = true;
        }

        private void Gener(object o)
        {
            Col[0] += CatW;
            Col[1] += Cat;
            Col[2] += Mouse;
            int[] tp = {CatW, Cat, Mouse};
            while (tp[0] + tp[1] + tp[2] != 0)
            {
                var e = Och.Next(300)/100;
                if (tp[e] == 0) continue;
                var i = Och.Next(N*M*100)/100;
                var j = i%M;
                i /= M;
                if (_p[i, j].Prep[0] || (e == 2 && (_p[i, j].Enemy[0] + _p[i, j].Enemy[1] != 0 || _p[i, j].Prep[2] || _p[i, j].Prep[3])) || (e != 2 && (_p[i, j].Enemy[2] != 0 || _p[i, j].Prep[1]))) continue;
                _p[i, j].Add(e);
                tp[e]--;
            }
            Adv.Reset();
            OnPropertyChanged(nameof(Button));
        }

        private void Clear(object o)
        {
            foreach (var cell in _p)
            {
                cell.Clear();
            }
            EnemyP.Clear();
            Col[0] = 0;
            Col[1] = 0;
            Col[2] = 0;
            OnPropertyChanged(nameof(Button));
            Adv.Reset();
        }

        private void Modify(object o)
        {
            foreach (var cell in _p)
            {
                cell.Redact(Life, Eat, TimeP, BornM);
            }
            foreach (var cell in _tp)
            {
                cell.Redact(Life, Eat, TimeP, BornM);
            }
        }

        private void Save(object o)
        {
            var i = N;
            while (--i >= 0)
            {
                var j = N;
                while (--j >= 0) _tp[i, j].Copy(_p[i, j]);
            }
        }

        private void Load(object o)
        {
            Col[0] = 0;
            Col[1] = 0;
            Col[2] = 0;
            var i = N;
            while (--i >= 0)
            {
                var j = N;
                while (--j >= 0)
                {
                    _p[i, j].Copy(_tp[i, j]);
                    if (_p[i, j].ColE() == 0) continue;
                    Col[0] += _p[i, j].Enemy[0];
                    Col[1] += _p[i, j].Enemy[1];
                    Col[2] += _p[i, j].Enemy[2];
                }
            }
            Adv.Reset();
            OnPropertyChanged(nameof(Button));
        }

        private void Go(object o)
        {
            _shag = false;
            Temp();
        }

        private void GoShag(object o)
        {
            BVis = !BVis;
            Temp();
        }

        private void OneIter(object o)
        {
            if (EnemyP.Count == 0)
            {
                var i = N;
                while (--i >= 0)
                {
                    var j = M;
                    while (--j >= 0) if (_p[i, j].ColE() != 0) EnemyP.Add(i * 100 + j);
                }
                if (EnemyP.Count == 0)
                {
                    BVis = true;
                    Iter.Change(-1, 0);
                    return;
                }
            }
            Game(null);
        }

        private void Temp()
        {
            if (EnemyP.Count == 0)
            {
                var i = N;
                while (--i >= 0)
                {
                    var j = M;
                    while (--j >= 0) if (_p[i, j].ColE() != 0) EnemyP.Add(i * 100 + j);
                }
                if (EnemyP.Count == 0)
                {
                    BVis = true;
                    Iter.Change(-1, 0);
                    return;
                }
            }
            if (!_bvis && _shag) Iter.Change(1000, 3000);
            else
            {
                Iter.Change(-1, 0);
                if (_shag) return;
                var k = 0;
                while (EnemyP.Count != 0) { Game(null); k++; }
                BVis = true;
                _shag = true;
            }
        }

        private void Game(object o)
        {
            var m = new int[9];
            var s = new int[9];
            var n = new bool[9];
            int i1 = EnemyP.Count, i = i1, j;
            while (--i1 >= 0) EnemyP[i1] += Och.Next(i*100)*10000;
            EnemyP.Sort();
            foreach (var ep in EnemyP.ToArray())
            {
                i = ep%10000/100;
                j = ep%100;
                var k = _p[i, j].ColE();
                if (k == 0) continue;
                m[8] = _p[i, j].Prep[1] ? 0 : 3;
                if (i > 0 && j > 0 && i < N - 1 && j < M - 1)
                {
                    m[0] = Mesto(_p[i - 1, j - 1]);
                    m[1] = Mesto(_p[i - 1, j]);
                    m[2] = Mesto(_p[i - 1, j + 1]);
                    m[3] = Mesto(_p[i, j + 1]);
                    m[4] = Mesto(_p[i + 1, j + 1]);
                    m[5] = Mesto(_p[i + 1, j]);
                    m[6] = Mesto(_p[i + 1, j - 1]);
                    m[7] = Mesto(_p[i, j - 1]);
                }
                else if (i > 0 && j > 0)
                {
                    m[0] = Mesto(_p[i - 1, j - 1]);
                    m[1] = Mesto(_p[i - 1, j]);
                    m[7] = Mesto(_p[i, j - 1]);
                    m[4] = 2;
                    if (i < N - 1)
                    {
                        m[5] = Mesto(_p[i + 1, j]);
                        m[6] = Mesto(_p[i + 1, j - 1]);
                        m[2] = m[3] = 2;
                    }
                    else if (j < M - 1)
                    {
                        m[2] = Mesto(_p[i - 1, j + 1]);
                        m[3] = Mesto(_p[i, j + 1]);
                        m[5] = m[6] = 2;
                    }
                    else m[2] = m[3] = m[5] = m[6] = 2;
                }
                else if (i < N - 1 && j < M - 1)
                {
                    m[3] = Mesto(_p[i, j + 1]);
                    m[4] = Mesto(_p[i + 1, j + 1]);
                    m[5] = Mesto(_p[i + 1, j]);
                    m[0] = 2;
                    if (i > 0)
                    {
                        m[1] = Mesto(_p[i - 1, j]);
                        m[2] = Mesto(_p[i - 1, j + 1]);
                        m[6] = m[7] = 2;
                    }
                    else if (j > 0)
                    {
                        m[6] = Mesto(_p[i + 1, j - 1]);
                        m[7] = Mesto(_p[i, j - 1]);
                        m[1] = m[2] = 2;
                    }
                    else m[1] = m[2] = m[6] = m[7] = 2;
                }
                else if (i > 0 && j < M - 1)
                {
                    m[1] = Mesto(_p[i - 1, j]);
                    m[2] = Mesto(_p[i - 1, j + 1]);
                    m[3] = Mesto(_p[i, j + 1]);
                    m[0] = m[4] = m[5] = m[6] = m[7] = 2;
                }
                else
                {
                    m[5] = Mesto(_p[i + 1, j]);
                    m[6] = Mesto(_p[i + 1, j - 1]);
                    m[7] = Mesto(_p[i, j - 1]);
                    m[0] = m[1] = m[2] = m[3] = m[4] = 2;
                }
                while (k-- > 0)
                {
                    var h = 9;
                    short ch;
                    i1 = 9;
                    while (--i1 >= 0) { n[i1] = true; s[i1] = 0; }
                    var e = _p[i, j].EnemyN(k);
                    int f;
                    if (e[0] == 2)
                    {
                        i1 = 8;
                        while (--i1 >= 0)
                        {
                            f = m[i1] % 80;
                            if (f > 8 && f < 24)
                            {
                                if (s[i1] < 3) s[i1] = 3;
                                f = i1 == 0 ? 7 : i1 - 1;
                                if (s[f] < 2 && m[f] % 10 != 0) s[f] = 2;
                                f = i1 == 7 ? 0 : i1 + 1;
                                if (s[f] < 2 && m[f] % 10 != 0) s[f] = 2;
                                switch (i1)
                                {
                                    case 0:
                                    case 4:
                                        if (s[2] == 0 && m[2]%10 != 0) s[2] = 1;
                                        if (s[6] == 0 && m[6]%10 != 0) s[6] = 1;
                                        break;
                                    case 2:
                                    case 6:
                                        if (s[0] == 0 && m[0]%10 != 0) s[0] = 1;
                                        if (s[4] == 0 && m[4]%10 != 0) s[4] = 1;
                                        break;
                                }
                            }
                            else if (f > 28)
                            {
                                s[i1] = 5;
                                f = i1 == 0 ? 7 : i1 - 1;
                                if (s[f] < 4 && m[f] % 10 != 0) s[f] = 4;
                                f = i1 == 7 ? 0 : i1 + 1;
                                if (s[f] < 4 && m[f] % 10 != 0) s[f] = 4;
                                if (s[8] < 4 && m[8] != 0) s[8] = 4;
                                switch (i1)
                                {
                                    case 1:
                                    case 5:
                                        if (s[3] < 4 && m[3] % 10 != 0) s[3] = 4;
                                        if (s[7] < 4 && m[7] % 10 != 0) s[7] = 4;
                                        break;
                                    case 3:
                                    case 7:
                                        if (s[1] < 4 && m[1] % 10 != 0) s[1] = 4;
                                        if (s[5] < 4 && m[5] % 10 != 0) s[5] = 4;
                                        break;
                                }
                            }
                            else if (m[i1] % 10 == 1 || m[i1] == 2) s[i1] = 5;
                        }
                        ch = -1;
                        do
                        {
                            f = 0;
                            if (ch++ == 4) break;
                            i1 = 9;
                            while (--i1 >= 0) if (s[i1] > ch) f++;
                        } while (h - f == 0);
                        if (ch == 5) continue;
                        h -= f;
                        i1 = 9;
                        while (--i1 >= 0) if (s[i1] > ch) n[i1] = false;
                    }
                    else
                    {
                        i1 = 8;
                        while (--i1 >= 0)
                        {
                            if (m[i1] >= 160) s[i1] = 6;
                            else if (m[i1] >= 80) s[i1] = 5;
                            else if (m[i1] > 64) s[i1] = 4;
                            else if (m[i1] > 56) s[i1] = 3;
                            else if (m[i1] > 48) s[i1] = 2;
                            else if (m[i1] > 16 && m[i1] < 36) s[i1] = 1;
                            else if (m[i1]%10 == 0 || m[i1] == 2) { n[i1] = false; h--; }
                        }
                        ch = 7;
                        do
                        {
                            f = 0;
                            if (ch-- == 1) break;
                            i1 = 8;
                            while (--i1 >= 0) if (s[i1] == ch) f++;
                        } while (f == 0);
                        if (ch != 0 && (e[0] == 1 || ch > 4))
                        {
                            i1 = 8;
                            h = f;
                            while (--i1 >= 0) if (s[i1] < ch) n[i1] = false;
                        }
                    }
                    h = Och.Next(h*100)/100;
                    while (true) if (n[++i1]) if (h-- == 0) { h = i1; break; }
                    if (h == 8) { _p[i, j].Wait(e[1]); continue; }
                    int ia = i, ja = j;
                    switch (h)
                    {
                        case 0: { ia--; ja--; break; }
                        case 1: { ia--; break; }
                        case 2: { ia--; ja++; break; }
                        case 3: { ja++; break; }
                        case 4: { ia++; ja++; break; }
                        case 5: { ia++; break; }
                        case 6: { ia++; ja--; break; }
                        case 7: { ja--; break; }
                    }
                    _p[ia, ja].Add(e[0], e[1]);
                    _p[i, j].Delete(e[0]);
                }
            }
            Col[0] = 0;
            Col[1] = 0;
            Col[2] = 0;
            foreach (var ep in EnemyP.ToArray())
            {
                i = ep%10000/100;
                j = ep%100;
                _p[i, j].Time();
                if (_p[i, j].ColE() == 0) { EnemyP.Remove(ep); continue; }
                Col[0] += _p[i, j].Enemy[0];
                Col[1] += _p[i, j].Enemy[1];
                Col[2] += _p[i, j].Enemy[2];
            }
            i1 = EnemyP.Count;
            while (--i1 >= 0) EnemyP[i1] %= 10000;
            i = N;
            while (--i >= 0)
            {
                j = M;
                while (--j >= 0)
                    if (_p[i, j].TimeColE() != 0)
                    {
                        _p[i, j].Time();
                        _p[i, j].TimeP3();
                        if (_p[i, j].ColE() == 0) continue;
                        Col[0] += _p[i, j].Enemy[0];
                        Col[1] += _p[i, j].Enemy[1];
                        Col[2] += _p[i, j].Enemy[2];
                        EnemyP.Add(i*100 + j);
                    }
                    else _p[i,j].TimeP3();
            }
            Adv = new ArrayDataView(_p);
            OnPropertyChanged(nameof(Adv));
            OnPropertyChanged(nameof(Button));
            if (EnemyP.Count != 0) return;
            BVis = true;
            Iter.Change(-1, 0);
        }

        private Timer _iter;

        private Timer Iter => _iter ?? (_iter = new Timer(Game, null, -1, 0));

        public void Click(object sender, MouseButtonEventArgs e)
        {
            if (EnemyP.Count != 0) return;
            var celldg = ((DataGrid) sender).CurrentCell;
            var arv = celldg.Item as ArrayRowView;
            if (arv == null) return;
            var cell = (Cell) arv.GetColumn(celldg.Column.DisplayIndex);
            var enemies = cell.Enemy;
            var preps = cell.Prep;
            if (enemies[0] == 1)
            {
                cell.Del(0);
                Col[0]--;
                cell.Add(1);
                Col[1]++;
                OnPropertyChanged(nameof(Button));
            }
            else if (enemies[1] == 1)
            {
                cell.Del(1);
                Col[1]--;
                cell.Add(2);
                Col[2]++;
                OnPropertyChanged(nameof(Button));
            }
            else if (enemies[2] == 1)
            {
                cell.Del(2);
                Col[2]--;
                preps[0] = true;
                OnPropertyChanged(nameof(Button));
            }
            else if (preps[0])
            {
                preps[0] = false;
                preps[1] = true;
            }
            else if (preps[1])
            {
                preps[1] = false;
                preps[2] = true;
            }
            else if (preps[2])
            {
                preps[2] = false;
                preps[3] = true;
            }
            else if (preps[3]) preps[3] = false;
            else
            {
                cell.Add(0);
                Col[0]++;
                OnPropertyChanged(nameof(Button));
            }
            Adv.Reset();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}