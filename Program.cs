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

            //dictionary to keep track of project index and days left 
            Dictionary<int, int> activeProjects = new Dictionary<int, int>();

            //for loop that iterates over days
            for (int i = 0; i < totalDays; i++)
            {
                bool newProjectStarted = false;
                //update our active project dictionary
                for (int j = 0; j < totalProjectCount; j++)
                {
                    if (matrix[j, i])
                    {
                        //if not in our active project dictionary, add it and set the new project flag to true 
                        if (!activeProjects.ContainsKey(j))
                        {
                            int projectLength = (int)(set[j].end - set[j].start).TotalDays + 1;
                            activeProjects.Add(j, projectLength);
                            newProjectStarted = true;
                        }
                    }
                }

                string dayClassification;

                //if there are no active projects, there is no cost for the day 
                if (activeProjects.Count() == 0)
                    dayClassification = "none";
                //the first and last day of the entire project sequ ence is always a travel day 
                else if (i == 0 || i == (totalDays - 1))
                    dayClassification = "travel";
                else
                {
                    if (activeProjects.Count() == 1)
                    {
                        //if the project is "pushed-up" against another project, its a full day 
                        if (AnotherProjectBefore(i, matrix, activeProjects, set) || AnotherProjectAfter(i, matrix, activeProjects, set, totalDays))
                        {
                            dayClassification = "full";
                        }
                        //if this is the beginning or end of a project, its a travel day 
                        else if (newProjectStarted || activeProjects.First().Value == 1)
                        {
                            dayClassification = "travel";
                        }
                        //if theres one active project and it doesnt meet the other criteria, it is ongoing and a full day 
                        else
                            dayClassification = "full";
                    }

                    //if there is greater than one active project, the "overlap" rule determines it will be a full day
                    else
                        dayClassification = "full";

                }
                // get cost for the day and add to our total 
                totalCost += getCostForDay(dayClassification, activeProjects, set);

                //decrement days in our active project dictionary, remove if zero days 
                foreach (var key in activeProjects.Keys.ToList())
                {
                    activeProjects[key] = activeProjects[key] - 1;
                    if (activeProjects[key] == 0) activeProjects.Remove(key);
                }
            }

            return totalCost;
        }

        /*
            return true if theres a project the next day that is not currently in our active project dictionary, 
            aka "pushed up" against another project
         */
        private static bool AnotherProjectAfter(int i, bool[,] matrix, Dictionary<int, int> activeProjects, List<Project> set, int totalDays)
        {
            for (int k = 0; k < set.Count(); k++)
            {
                // if its not in our active projects dictionary, its a new project starting
                if (matrix[k, (i + 1)] && !activeProjects.ContainsKey(k))
                    return true;
            }
            return false;
        }
        /*
            returns true if theres another project ending the previous day 
            aka "pushed up" against another project
         */
        private static bool AnotherProjectBefore(int i, bool[,] matrix, Dictionary<int, int> activeProjects, List<Project> set)
        {

            for (int k = 0; k < set.Count(); k++)
            {
                //if theres a project the day before thats not in our active project dictionary, it has ended
                if (matrix[k, (i - 1)] && !activeProjects.ContainsKey(k))
                    return true;
            }
            return false;

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

        /*
            returns the cost for each day based on the city cost and travel/full/none classification
         */
        private static int getCostForDay(string dayClassification, Dictionary<int, int> activeProjects, List<Project> set)
        {
            string cityCost;
            if (dayClassification == "none")
                return 0;

            //if we have more than one project on the same day, the high cost project takes precedant     
            if (activeProjects.Count() > 1)
            {
                cityCost = activeProjects.ContainsKey(set.FindIndex(x => x.cost == "high")) ? "high" : "low";
            }
            //else just take the only active project we have 
            else
            {
                cityCost = set[activeProjects.First().Key].cost;
            }
            if (cityCost == "high")
            {
                return dayClassification == "full" ? 85 : 55;
            }
            // else low cost city
            else
                return dayClassification == "full" ? 75 : 45;
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

    }


}
