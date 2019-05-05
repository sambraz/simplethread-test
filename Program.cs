using System;
using System.Collections.Generic;
using System.Linq;

namespace simpletest
{

    class Program
    {
        static void Main(string[] args)
        {
            //set 1 
            int set1Total = CalculateTravelCosts(dataset.set1);
            Console.WriteLine(String.Format("Total cost for set 1: ${0}", set1Total));

            //set 2 
            int set2Total = CalculateTravelCosts(dataset.set2);
            Console.WriteLine(String.Format("Total cost for set 2: ${0}", set2Total));

            //set 3 
            int set3Total = CalculateTravelCosts(dataset.set3);
            Console.WriteLine(String.Format("Total cost for set 3: ${0}", set3Total));

            //set 4 
            int set4Total = CalculateTravelCosts(dataset.set4);
            Console.WriteLine(String.Format("Total cost for set 4: ${0}", set4Total));

        }


        public static int CalculateTravelCosts(List<Project> set)
        {
            int totalCost = 0;

            int min = set.Min(x => x.start.DayOfYear);
            int max = set.Max(x => x.end.DayOfYear);
            //total amount of days for the entire set of projects
            int totalDays = (int)(max - min) + 1;

            //total amount of projects in the set 
            int totalProjectCount = set.Count();

            //get our matrix of bools 
            bool[,] matrix = GetDaysMatrix(set, min, totalDays);
            
            
            return totalCost;
        }

        /*
        create a 2d matrix of bools, projects x days 
        the first day is at index 0
        note: this using the assumption that projects are the same calendar year
        additional code would need to be added to adjust for projects over multiple years 
         */
        public static bool[,] GetDaysMatrix(List<Project> set, int min, int totalDays)
        {
            
            bool[,] matrix = new bool[set.Count(), totalDays];
            for (int i = 0; i < set.Count(); i++)
            {
                //flip bools to true for days the project is active 
                for (int j = (set[i].start.DayOfYear - min); j <= (set[i].end.DayOfYear - min); j++)
                {
                    matrix[i, j] = true;
                }
            }
            return matrix;
        }

    }



    public class Project
    {
        public int project;
        public DateTime start;
        public DateTime end;
        public string cost;
    }

    public class dataset
    {
        public static List<Project> set1 = new List<Project>
        {
            new Project {
                project = 1,
                start = new DateTime(2015, 9, 1),
                end = new DateTime(2015, 9, 3),
                cost = "low"
        }};

        public static List<Project> set2 = new List<Project>
        {
            new Project {
                project = 1,
                start = new DateTime(2015, 9, 1),
                end = new DateTime(2015, 9, 1),
                cost = "low"
        },

            new Project {
                project = 2,
                start = new DateTime(2015, 9, 2),
                end = new DateTime(2015, 9, 6),
                cost = "high"
        },

            new Project {
                project = 3,
                start = new DateTime(2015, 9, 6),
                end = new DateTime(2015, 9, 8),
                cost = "low"
        }};

        public static List<Project> set3 = new List<Project>
        {
            new Project {
                project = 1,
                start = new DateTime(2015, 9, 1),
                end = new DateTime(2015, 9, 3),
                cost = "low"
        },

            new Project {
                project = 2,
                start = new DateTime(2015, 9, 5),
                end = new DateTime(2015, 9, 7),
                cost = "high"
        },

            new Project {
                project = 3,
                start = new DateTime(2015, 9, 8),
                end = new DateTime(2015, 9, 8),
                cost = "high"
        }
        };

        public static List<Project> set4 = new List<Project>
        {
            new Project {
                project = 1,
                start = new DateTime(2015, 9, 1),
                end = new DateTime(2015, 9, 1),
                cost = "low"
        },

            new Project {
                project = 2,
                start = new DateTime(2015, 9, 1),
                end = new DateTime(2015, 9, 1),
                cost = "low"
        },

            new Project {
                project = 3,
                start = new DateTime(2015, 9, 2),
                end = new DateTime(2015, 9, 2),
                cost = "high"
        },

        new Project {
                project = 4,
                start = new DateTime(2015, 9, 2),
                end = new DateTime(2015, 9, 3),
                cost = "high"
        }
        };

        public static List<List<Project>> dataSet = new List<List<Project>> { set4 };

    }


}
