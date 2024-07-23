using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Project_4.Misc
{
    public static class DBModel
    {
        public static string[] Scores = new string[5];

        public static void ReadLists()
        {

            if (!File.Exists("Score.txt")) // in case this is the first time running the program
                File.Create("Score.txt").Close(); // I use Close() to dispose.


            using (StreamReader sr = new StreamReader("Score.txt")) // reads file
            {

                string line;
                int lineNum = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    Scores[lineNum] = line;

                    lineNum++;

/*                    if (line[0] == '1')  // checks first character
                    {
                        if (toDoList.Count < 15) // maximum to-do list count is 15
                        {
                            string[] tokens = line.Split(';');
                            toDoList.Add(tokens);
                        }
                    }
                    else if (line[0] == '2') // checks first character
                    {
                        if (workingOnList.Count < 3) // maximum to-do list count is 15
                        {
                            string[] tokens = line.Split(';');
                            workingOnList.Add(tokens);
                        }


                    }
                    else if (line[0] == '3') //checks first character
                    {
                        string[] tokens = line.Split(';');
                        doneList.Add(tokens);
                    }*/


/*                    else
                    {
                        // MessageBox.Show("GoalActivity.txt might be corrupted");
                    }*/
                }
            }

        }

        public static void SaveList(int score)
        {
            DateTime dt = DateTime.Now.ToLocalTime();
            string.Format("{0:g}", dt);
            if (Scores.Length < 5)
            {
                while (5 - Scores.Length != 0)
                    Scores[Scores.Length + 1] = "0000 - 00/00/0000 00:00";
            }
            else
            {
                for (int i = 3; i >= 0; i--)
                { 
                    Scores[i+1] = Scores[i];
                }
            }
            Scores[0] = $"{score} - {dt}";

            using (StreamWriter sw = new StreamWriter("Score.txt")) // overwrites the old txt file to refill it with the new updated lists
            {
                //int i = File.ReadAllLines("Score.txt").Length;
                foreach (string s in Scores)
                    sw.WriteLine(s);
                //sw.WriteLine($"{score} - {dt}");


                /*                toDoList = toDoList.OrderBy(arr => arr[1]).ToList(); // to sort by priority, hence arr[1]
                                foreach (string[] s in toDoList)
                                    sw.WriteLine($"{s[0]};{s[1]};{s[2]};{s[3]};{s[4]}");

                                workingOnList = workingOnList.OrderBy(arr => arr[1]).ToList(); // to sort
                                foreach (string[] s in workingOnList)
                                    sw.WriteLine($"{s[0]};{s[1]};{s[2]};{s[3]};{s[4]}");

                                doneList = doneList.OrderBy(arr => arr[1]).ToList(); // to sort
                                foreach (string[] s in doneList)
                                    sw.WriteLine($"{s[0]};{s[1]};{s[2]};{s[3]};{s[4]}");
                            }*/
            }
        }
    }
}
