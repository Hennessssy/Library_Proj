using System;
using Library_Proj.Data;
using Library_Proj.Models;

class Program
{
    static void Main(string[] args)
    {
        using (var db = new LibraryContext())
        {
            bool created = db.Database.EnsureCreated();

            if (created)
            {
                Console.WriteLine("база создана");
            }
            else
            {
                Console.WriteLine("база уже была создана");
            }
        }

        Console.ReadLine();
    }
}