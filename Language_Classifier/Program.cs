using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Language_Classifier
{
    class Program
    {
        static void Main(string[] args)
        {
            String InputFolder;
            if (args.Length == 0)
                InputFolder = "/opt/dropbox/15-16/473/project5/language-models";
            else
                InputFolder = args[0];

            Dictionary<String, double>[] DictList = new Dictionary<string, double>[15];
            String[] LanguageName = new String[15];
            double[] TotalWords = new double[15];
            CheckDir(InputFolder);
            int count = 0;
            double noOfWords = 0;
            double delta = 0.001;
            string line;
            //Create an array of dictionary one dictioanry for each language.
            foreach (string file in Directory.EnumerateFiles(InputFolder, "*"))
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    noOfWords = 0;
                    LanguageName[count] = Path.GetFileName(file).Substring(0,3);
                    Dictionary<String, double> tempDict = new Dictionary<string, double>();
                    while((line = reader.ReadLine()) !=null)
                    {
                        string[] words = line.Split('\t');
                        double value = Int32.Parse(words[1]) + delta;
                        tempDict[words[0]] = value;
                        noOfWords += value;
                    }
                    TotalWords[count] = noOfWords;
                    DictList[count++]=tempDict;
                }
            }

            String TestFolder ;
            if (args.Length > 1)
                TestFolder = args[1];
            else
                TestFolder = "/opt/dropbox/15-16/473/project5";

            CheckDir(TestFolder);
            string trainFile = TestFolder + "\\extra-train.txt";
            string Pattern = Regex.Escape(".,!¡¥$£?¿;:()\"/¹²³«»");
            Pattern = "[" + Pattern + @"\—\–\-]";
            Regex regex1 = new Regex(@Pattern);
            Regex regex2 = new Regex(@"[\[\]]");
            //Regex regex3 = new Regex(@"[\']");
            System.IO.StreamWriter outfile =
            new System.IO.StreamWriter(@"WriteLines2.txt");
            using (StreamReader train = new StreamReader(trainFile))
            {
                //int linenumber = 1;
                while((line = train.ReadLine()) !=null)
                {
                    outfile.WriteLine(line);
                    line = regex1.Replace(line, " ");
                    line = regex2.Replace(line, " ");
                    //line = regex3.Replace(line, " ");
                    double[] score = new double[15];
                    string label = line.Split('\t')[0];
                    line = line.Split('\t')[1];
                    string[] words = line.Split(' ');
                    for (int i = 0; i < 15; i++)
                    {
                        var LanDict = DictList[i];
                        double langCount = (double)TotalWords[i];
                        double LangProb = 0;
                        foreach (var item in words)
                        {
                            if (!String.IsNullOrWhiteSpace(item))
                            {
                                double prob = 0.0;
                                if (LanDict.ContainsKey(item))
                                    prob = LanDict[item] / langCount;
                                else
                                    prob = delta / langCount;
                                LangProb += Math.Log10(prob);
                            }
                                
                                
                        }
                        outfile.WriteLine(LanguageName[i] + "\t" + LangProb);
                        score[i] = LangProb;
                    }
                    double maxValue = score.Max();
                    int maxIndex = score.ToList().IndexOf(maxValue);
                    //int actualIndex = LanguageName.ToList().IndexOf(label);
                    //if (maxIndex == actualIndex)
                    //    Console.WriteLine(maxValue);
                    //else
                    //    Console.WriteLine("Linenumber : " +linenumber);
                    //linenumber++;

                    outfile.WriteLine("result" +"\t" + LanguageName[maxIndex]);
                    //Console.WriteLine();

                }
            }
            outfile.Close();
            Console.ReadLine();
        }

        //Ref: https://stackoverflow.com/questions/1073038/c-should-i-throw-an-argumentexception-or-a-directorynotfoundexception/1073349#1073349
        static void CheckDir(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Incorrect Path");
            }
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException();
            }
        }
    }
}
