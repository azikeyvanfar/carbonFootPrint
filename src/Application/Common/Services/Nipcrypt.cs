using System;
using System.Collections.Generic;

namespace ContractorBackend.Application.Common.Services
{
    public class Nipcrypt
    {
        // Fields
        public string e1;
        public string e2;
        public List<string> ea = new List<string>();
        public List<string> eo = new List<string>();
        public int ei;
        public string nipenRes;
        public string d1;
        public string d2;
        public int di;
        public List<string> da = new List<string>();
        public List<string> dO = new List<string>();
        public string nipdeRes;
        public List<string> chs;
        public List<string> rndcde;
        public List<string> rndcde2;
        public string dumch;
        public string dumch2;
        public int ri;

        // Methods
        public Nipcrypt()
        {
            List<string> list = new List<string> {
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h",
            "i",
            "j",
            "k",
            "l",
            "m",
            "n",
            "o",
            "p",
            "q",
            "r",
            "s",
            "t",
            "u",
            "v",
            "w",
            "x",
            "y",
            "z",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"
        };
            this.chs = list;
            this.rndcde = new List<string>();
            this.rndcde2 = new List<string>();
        }

        public void nipde(string sek)
        {
            this.d1 = sek.Substring(sek.Length - 10, 10);
            this.d2 = sek.Substring(6, sek.Length - 0x10);
            this.da.Clear();
            this.dO.Clear();
            this.di = 0;
            while (true)
            {
                bool flag = this.di <= (this.d1.Length - 1);
                if (!flag)
                {
                    this.di = 0;
                    while (true)
                    {
                        flag = this.di <= (this.d2.Length - 1);
                        if (!flag)
                        {
                            this.nipdeRes = string.Join("", this.dO.ToArray());
                            return;
                        }
                        this.dO.Add(Convert.ToString(this.da.IndexOf(this.d2.Substring(this.di, 1))));
                        this.di++;
                    }
                }
                this.da.Add(this.d1.Substring(this.di, 1));
                this.di++;
            }
        }

        public void nipen(string ind)
        {
            this.niprnd();
            this.e1 = ind;
            this.e2 = string.Join("", this.rndcde.ToArray());
            this.ea.Clear();
            this.eo.Clear();
            this.ei = 0;
            while (true)
            {
                bool flag = this.ei <= (this.e2.Length - 1);
                if (!flag)
                {
                    this.ei = 0;
                    while (true)
                    {
                        flag = this.ei <= (this.e1.Length - 1);
                        if (!flag)
                        {
                            this.nipenRes = string.Join("", this.rndcde2.ToArray()) + string.Join("", this.eo.ToArray()) + this.e2;
                            return;
                        }
                        this.eo.Add(this.ea[Convert.ToInt32(this.e1.Substring(this.ei, 1))]);
                        this.ei++;
                    }
                }
                this.ea.Add(this.e2.Substring(this.ei, 1));
                this.ei++;
            }
        }

        public void niprnd()
        {
            this.dumch = "";
            this.dumch2 = "";
            Random random = new Random();
            while (true)
            {
                bool flag = this.rndcde.ToArray().Length < 10;
                if (!flag)
                {
                    while (true)
                    {
                        flag = this.rndcde2.ToArray().Length < 6;
                        if (!flag)
                        {
                            return;
                        }
                        this.dumch2 = this.chs[random.Next(0x3e)];
                        if (!this.rndcde2.Contains(this.dumch2))
                        {
                            this.rndcde2.Add(this.dumch2);
                        }
                    }
                }
                this.dumch = this.chs[random.Next(0x3e)];
                if (!this.rndcde.Contains(this.dumch))
                {
                    this.rndcde.Add(this.dumch);
                }
            }
        }
    }





}
