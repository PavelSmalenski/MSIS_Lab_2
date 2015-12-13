using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2_Spen_Delphi
{
    class Program
    {
        const string FilePath = "d:\\Мои документы\\МСиСвИнфТ\\Lab_2_Spen_Delphi\\Lab_2_Spen_Delphi\\bin\\Release\\Program_To_Test_On.txt";

        static List<string> ReadFromFile(string FilePath)
        {
            List<string> TempList = new List<string>();
            System.IO.StreamReader ProgramFile = new System.IO.StreamReader(FilePath);
            string TempLine;

            while ((TempLine = ProgramFile.ReadLine()) != null)
            {
                TempList.Add(TempLine);
            }
            ProgramFile.Close();

            return TempList;
        }

        static void Main(string[] args)
        {
            List<string> ProgramList;
            ProgramList = ReadFromFile(FilePath);

            Spen CurrentSpen = new Spen();
            CurrentSpen.InitializeText(ProgramList);
            CurrentSpen.ClearText();
            CurrentSpen.CountSpen();
            CurrentSpen.PrintResults();

            Console.ReadLine();
        }
    }
}
