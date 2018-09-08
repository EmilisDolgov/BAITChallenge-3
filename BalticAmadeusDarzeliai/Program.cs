using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalticAmadeusDarzeliai
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxValue = new int();
            int maxValueIndex = new int();
            int minValue = new int();
            int minValueIndex = new int();
            var listKindergarten = new List<Kindergarten>();
            ReadFile(listKindergarten);
            FindMax(listKindergarten, ref maxValue, ref maxValueIndex);
            FindMin(listKindergarten, ref minValue, ref minValueIndex);
            Console.WriteLine("Max CHILDS_COUNT value " + maxValue);
            Console.WriteLine("Min CHILDS_COUNT value " + minValue);
            FullStringToFile(maxValue, listKindergarten);
            FullStringToFile(minValue, listKindergarten);
            MaxPercentageToFile(listKindergarten);
            GroupedListToFile(listKindergarten);
        }
        static void ReadFile(List<Kindergarten> ListKindergarten)
        {
            TextReader reader = File.OpenText(@"C:\Users\HP\Desktop\BalticAmadeusDarzeliai\BalticAmadeusDarzeliai\darzeliai.csv");
            CsvReader csv = new CsvReader(reader);
            csv.Configuration.Delimiter = ";";
            csv.Configuration.MissingFieldFound = null;
            while (csv.Read())
            {
                Kindergarten Record = csv.GetRecord<Kindergarten>();
                ListKindergarten.Add(Record);
            }

        }
        public static void FindMax(List<Kindergarten> listKindergarten, ref int maxValue, ref int maxValueIndex)
        {
            maxValueIndex = -1;
            maxValue = Int32.MinValue;
            for (int i = 0; i < listKindergarten.Count; i++)
            {
                var value = listKindergarten[i].CHILDS_COUNT;
                if (value > maxValue)
                {
                    maxValue = value;
                    maxValueIndex = i;
                }
            }
        }
        public static void FindMin(List<Kindergarten> listKindergarten, ref int minValue, ref int minValueIndex)
        {
            minValueIndex = -1;
            minValue = Int32.MaxValue;
            for (int i = 0; i < listKindergarten.Count; i++)
            {
                var value = listKindergarten[i].CHILDS_COUNT;
                if (value < minValue)
                {
                    minValue = value;
                    minValueIndex = i;
                }
            }
        }
        public static string FullString(Kindergarten kindergarten)
        {
            string label = kindergarten.TYPE_LABEL;
            StringBuilder builder = new StringBuilder(label);
            builder.Replace("Nuo ", "");
            builder.Replace(" iki ", "-");
            builder.Replace(" metų", "");
            label = builder.ToString();
            string schoolName = kindergarten.SCHOOL_NAME;
            schoolName = schoolName.Substring(0, 3);
            string lanLabel = kindergarten.LAN_LABEL;
            lanLabel = lanLabel.Substring(0, 4);
            return schoolName + "_" + label + "_" + lanLabel;

        }
        public static void FullStringToFile(int value, List<Kindergarten> kindergartens)
        {
            var list = kindergartens.Where(x => x.CHILDS_COUNT == value).ToList();
            TextWriter tw = new StreamWriter("FullString.txt");
            foreach (var i in list)
                tw.WriteLine(FullString(i));
            tw.Close();
                
        }
        public static void MaxPercentageToFile(List<Kindergarten> listKindergarten)
        {
            var languages = listKindergarten.Select(x => x.LAN_LABEL).Distinct();
            var languagesWithCount = new List<Tuple<string, string>>();
            foreach (var lang in languages)
            {
                var freeSpace = listKindergarten.Where(x => x.LAN_LABEL == lang).Sum(y => y.FREE_SPACE);
                var childsCount = listKindergarten.Where(x => x.LAN_LABEL == lang).Sum(y => y.CHILDS_COUNT);
                var percentage = ((double)freeSpace / childsCount).ToString("0.00%");
                languagesWithCount.Add(new Tuple<string, string>(lang, percentage));
            }
            var maxPercentage = languagesWithCount.Max(x => x.Item2);
            var indexMaxPercentage = languagesWithCount.FindIndex(x => x.Item2 == maxPercentage);
            var kindergartenName = languagesWithCount[indexMaxPercentage].Item1;
            TextWriter tw = new StreamWriter("MostFreeSpace.txt");
            tw.WriteLine(kindergartenName + " " + maxPercentage.ToString());
            tw.Close();
        }
        public static void GroupedListToFile(List<Kindergarten> listKindergarten)
        {
            var sortedList = listKindergarten.Where(x => x.FREE_SPACE >= 2 && x.FREE_SPACE <= 4).ToList();
            var groupedList = sortedList.GroupBy(x => x.SCHOOL_NAME).Select(group => new { SchoolName = group.Key, ChildCount = group.Sum(x => x.CHILDS_COUNT), FreeSpace = group.Sum(x => x.FREE_SPACE), Languages = group.Select(x => x.LAN_LABEL).Distinct().ToList() }).ToList();
            var groupedListAlphabetically = groupedList.OrderByDescending(x=>x.SchoolName).ToList();
            TextWriter tw = new StreamWriter("GroupedList.txt");
            foreach (var i in groupedListAlphabetically)
                tw.WriteLine(i.SchoolName + ", vaikų kiekis: " + i.ChildCount + ", tuščios vietos: " + i.FreeSpace);
            tw.Close();
        }
    }
}
