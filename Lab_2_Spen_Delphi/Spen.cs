using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Lab_2_Spen_Delphi
{
    class Spen
    {
        private const string EmptyLine = "";
        private const string LiteralExpression = "(\'[^\']*\')";
        private const string SinglelineCommentExpression = "//.*$";
        private const string MultilineCommentOpeningExpression = "[{][^}]*$";
        private const string MultilineCommentClosureExpression = "^[^{]*[}]";
        private const string MultilineCommentSingleExpression = "[{].*[}]";
        private const string ForwardDescrptionExpression = "(;)(\\s)*(forward;)$";
        private const string MethodExpression = "^(((procedure)|(function))\\s)";
        private const string MethodNameExpression = "^[\\w]*(\\s|\\()";
        private const int    FieldWidth = 41;
        private struct VariablesElement
        {
            public string Name;
            public int SpenNumber;
            public string MethodName;
        }

        private List<string> ProgramText;
        private List<VariablesElement> VariablesToOutput = null;
        private int CurrentPosition;

        private void RemoveLiterals()
        {
            for (int i = 0; i < ProgramText.Count; i++)
            {
                Regex LiteralRegex = new Regex(LiteralExpression);
                ProgramText[i] = LiteralRegex.Replace(ProgramText[i], EmptyLine);
                ProgramText[i] = ProgramText[i].Trim();
            }
        }
        private void RemoveSinglelineComments()
        {
            for (int i = 0; i < ProgramText.Count; i++)
            {
                Regex CommentsRegex = new Regex(SinglelineCommentExpression);
                ProgramText[i] = CommentsRegex.Replace(ProgramText[i], EmptyLine);
                ProgramText[i] = ProgramText[i].Trim();
            }
        }
        private void RemoveMultilineComments()
        {
            bool IsOpen = false;
            for (int i = 0; i < ProgramText.Count; i++)
            {
                if (IsOpen)
                {
                    Regex CommentsRegex = new Regex(MultilineCommentClosureExpression);
                    if (CommentsRegex.IsMatch(ProgramText[i]))
                    {
                        ProgramText[i] = CommentsRegex.Replace(ProgramText[i], EmptyLine);
                        IsOpen = false;
                    }
                    else
                    {
                        ProgramText[i] = EmptyLine;
                    }
                }
                else
                {
                    Regex CommentsRegex = new Regex(MultilineCommentOpeningExpression);
                    if (CommentsRegex.IsMatch(ProgramText[i]))
                    {
                        ProgramText[i] = CommentsRegex.Replace(ProgramText[i], EmptyLine);
                        IsOpen = true;
                    }
                    else
                    {
                        CommentsRegex = new Regex(MultilineCommentSingleExpression);
                        if (CommentsRegex.IsMatch(ProgramText[i]))
                        {
                            ProgramText[i] = CommentsRegex.Replace(ProgramText[i], EmptyLine);
                        }
                    }
                    ProgramText[i] = ProgramText[i].Trim();
                }
            }
        }
        private void RemoveEmptyLines()
        {
            int i = 0;
            while (i < ProgramText.Count)
            {
                if (string.Compare(ProgramText[i], EmptyLine) == 0)
                {
                    ProgramText.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
        private void RemoveForwardDescription()
        {
            for (int i = 0; i < ProgramText.Count; i++)
            {
                Regex ForwardRegex = new Regex(ForwardDescrptionExpression, RegexOptions.IgnoreCase);
                if (ForwardRegex.IsMatch(ProgramText[i]))
                {
                    ProgramText[i] = EmptyLine;
                }
            }
        }
        private void TrimProgramText()
        {
            for (int i = 0; i < ProgramText.Count; i++)
            {
                ProgramText[i] = ProgramText[i].Trim();
            }
        }
        private string GetMethodName(string Line)
        {
            Regex MethodNameRegex = new Regex(MethodNameExpression);
            return MethodNameRegex.Match(Line).Groups[0].Value;
        }
        private bool CheckVarEnd(string Line)
        {
            Regex TempRegex = new Regex(MethodExpression);
            if ((String.Compare(Line, "type") == 0) 
                || (String.Compare(Line, "const") == 0)
                || (String.Compare(Line, "begin") == 0)
                || (TempRegex.IsMatch(Line)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool FindLocalAnalog(string Line, List<VariablesElement> Variables)
        {
            foreach (var Variable in Variables)
            {
                if ((Variable.Name == Line) && (Variable.MethodName != "Main"))
                {
                    return true;
                }
            }
            return false;
        }


        public  void InitializeText(List<string> ExternalProgramText)
        {
            this.ProgramText = new List<string>(ExternalProgramText);
            CurrentPosition = 0;
        }
        public  void ClearText()
        {
            this.TrimProgramText();
            this.RemoveLiterals();
            this.RemoveSinglelineComments();
            this.RemoveMultilineComments();
            this.RemoveForwardDescription();
            this.TrimProgramText();
            this.RemoveEmptyLines();
            for (int i = 0; i < ProgramText.Count; i++)
            {
                ProgramText[i] = ProgramText[i].ToLower();
            }
        }
        public  void CountSpen()
        {
            List<VariablesElement> Variables = new List<VariablesElement>();
            string CurrentMethodName = "Main";
            int BeginsCount = 0;

            while (CurrentPosition < ProgramText.Count)
            {
                Regex MethodRegex = new Regex(MethodExpression, RegexOptions.IgnoreCase);
                if (MethodRegex.IsMatch(ProgramText[CurrentPosition]))
                {
                    ProgramText[CurrentPosition] = MethodRegex.Replace(ProgramText[CurrentPosition], EmptyLine);
                    ProgramText[CurrentPosition] = ProgramText[CurrentPosition].Trim();
                    ProgramText[CurrentPosition] = ProgramText[CurrentPosition].Trim();
                    CurrentMethodName = GetMethodName(ProgramText[CurrentPosition]);
                    CurrentMethodName = CurrentMethodName.Remove(CurrentMethodName.Length - 1, 1);
                }

                MethodRegex = new Regex("^(" + CurrentMethodName + ")(\\s|\\()");
                if (MethodRegex.IsMatch(ProgramText[CurrentPosition]))
                {
                    MethodRegex = new Regex("[\\w]+[\\s]*(:|,)");
                    foreach (Match match in MethodRegex.Matches(ProgramText[CurrentPosition]))
                    {
                        string Temp = match.Groups[0].Value;
                        Temp = Temp.Remove(Temp.Length - 1, 1);
                        Temp = Temp.Trim();
                        Variables.Add(new VariablesElement { Name = Temp, SpenNumber = 0, MethodName = CurrentMethodName});
                    }
                    CurrentPosition++;
                    continue;
                }

                if (String.Compare(ProgramText[CurrentPosition], "var") == 0)
                {
                    while ((!CheckVarEnd(ProgramText[CurrentPosition])) && (CurrentPosition < ProgramText.Count))
                    {
                        MethodRegex = new Regex("[\\w]+[\\s]*(:|,)");

                        var TempMatches = MethodRegex.Matches(ProgramText[CurrentPosition]);
                        foreach (Match match in TempMatches)
                        {
                            string Temp = match.Groups[0].Value;
                            Temp = Temp.Remove(Temp.Length - 1, 1);
                            Temp = Temp.Trim();
                            Variables.Add(new VariablesElement { Name = Temp, SpenNumber = 0, MethodName = CurrentMethodName });
                        }
                        CurrentPosition++;
                    }
                    continue;
                }
                
                
                if ((String.Compare(ProgramText[CurrentPosition], "begin") == 0) && (BeginsCount == 0))
                {
                    do
                    {
                        if (String.Compare(ProgramText[CurrentPosition], "begin") == 0)
                        {
                            BeginsCount++;
                            CurrentPosition++;
                            continue;
                        }
                        else if ((String.Compare(ProgramText[CurrentPosition], "end") == 0) || (String.Compare(ProgramText[CurrentPosition], "end;") == 0))
                        {
                            BeginsCount--;
                            if (BeginsCount != 0)
                            {
                                CurrentPosition++;
                            }
                            continue;
                        }
                        else
                        {
                            MethodRegex = new Regex("(case\\s)", RegexOptions.IgnoreCase);
                            if (MethodRegex.IsMatch(ProgramText[CurrentPosition]))
                            {
                                BeginsCount++;
                                CurrentPosition++;
                                continue;
                            }
                        }
                        for (int i = 0; i < Variables.Count; i++)
                        {
                            if ((Variables[i].MethodName == CurrentMethodName) 
                                || ((CurrentMethodName != "Main") && (!FindLocalAnalog(Variables[i].Name, Variables) && (Variables[i].MethodName == "Main"))))
                            {
                                MethodRegex = new Regex("(" + Variables[i].Name + ")(\\s|\\+|-|/|\\*|=|\\^|\\)|:|,|\\]|\\[|;|^|\\.)");
                                while (MethodRegex.IsMatch(ProgramText[CurrentPosition]))
                                {
                                    var Temp = Variables[i];
                                    (Temp.SpenNumber)++;
                                    Variables[i] = Temp;
                                    ProgramText[CurrentPosition] = ProgramText[CurrentPosition].Remove(MethodRegex.Match(ProgramText[CurrentPosition]).Groups[0].Index, MethodRegex.Match(ProgramText[CurrentPosition]).Groups[0].Value.Length);
                                }
                            }
                        }
                         CurrentPosition++;
                    }
                    while ((BeginsCount != 0) && (CurrentPosition < ProgramText.Count));
                    CurrentMethodName = "Main";
                }
                CurrentPosition++;                                                                                                              //Check
            }

            this.VariablesToOutput = new List<VariablesElement>(Variables);
        }
        public void PrintResults()
        {
            if (VariablesToOutput != null)
            { 
                Console.WriteLine("|      METHOD     |     VARIABLE    | SPEN|");
                Console.Write(" ");
                for (int i = 0; i < FieldWidth; i++)
                {
                    Console.Write("-");
                }
                Console.WriteLine();
                foreach (var Variable in VariablesToOutput)
                {
                    Console.WriteLine("|{0,17}|{1, 17}|{2, 5}|", Variable.MethodName, Variable.Name, Variable.SpenNumber);
                    Console.Write(" ");
                    for (int i = 0; i < FieldWidth; i++)
                    {
                        Console.Write("-");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
