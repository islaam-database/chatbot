﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Islaam;
using Microsoft.EntityFrameworkCore;

namespace idb_dialog_flow
{
    public class WhoIsHandler : SinglePersonHandler
    {
        public WhoIsHandler(Database idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
            Person = idb
                .People
                .Include(p => p.PraisesReceived)
                    .ThenInclude(p => p.Praiser)
                        .ThenInclude(p => p.MainTitle)
                            .ThenInclude(p => p.Status)
                .Include(p => p.PraisesReceived)
                    .ThenInclude(p => p.Title)
                        .ThenInclude(p => p.Status)
                .Include(p => p.PraisesReceived)
                    .ThenInclude(p => p.Praiser)
                        .ThenInclude(p => p.PraisesReceived)
                            .ThenInclude(p => p.Praiser)
                                .ThenInclude(p => p.MainTitle)
                                    .ThenInclude(p => p.Status)
                .Include(p => p.Teachers)
                    .ThenInclude(ts => ts.Teacher)
                        .ThenInclude(p => p.MainTitle)
                .Include(p => p.Students)
                    .ThenInclude(ts => ts.Student)
                        .ThenInclude(p => p.MainTitle)
                .Include(p => p.MainTitle)
                    .ThenInclude(p => p.Status)
                .Where(p => p.Id == Person.Id)
                .FirstOrDefault();
        }

        public override string TextResponse
        {
            get
            {
                if (Person == null)
                    return PnfHandler.TextResponse;

                var bio = Person.Bio;

                if (bio.amountOfInfo <= 2)
                {
                    bio.text += " That's all the information I have at the moment.";
                }
                return bio.text;
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                if (Person == null)
                    return PnfHandler.QuickReplies;

                var teachers = Person.Teachers.ToList(); // slow?
                var students = Person.Students.ToList();
                var teachNames = teachers.Select(x => x.Teacher.FriendlyName).ToList();
                var studNames = students.Select(x => x.Student.FriendlyName).ToList();

                var suggestions = GetFivePeopleForSuggestions(teachNames, studNames, SearchResults)
                    .Select(Formula)
                    .ToList();

                return suggestions;
            }
        }

        protected override string Formula(string personName) => $"Who is {personName}?";

        public static List<string> GetFivePeopleForSuggestions(
            List<string> teachers,
            List<string> students,
            List<PersonSearchResult> searchResults
        )
        {
            var fivePeople = new List<string>();
            var minGroupLength = Math.Min(teachers.Count, students.Count);
            if (minGroupLength >= 2)
            {
                fivePeople = fivePeople.Concat(teachers.Take(2)).ToList(); // add 2 teachers
                fivePeople = fivePeople.Concat(students.Take(2)).ToList(); // add 2 students
            }
            else
            {
                var sum = teachers.Count + students.Count;
                if (sum >= 4)
                {
                    var minGroup = teachers.Count < students.Count ? teachers : students;
                    var maxGroup = teachers.Count < students.Count ? students : teachers;
                    fivePeople = fivePeople.Concat(minGroup).ToList(); // add small group
                    var amountLeft = 4 - fivePeople.Count;
                    fivePeople = fivePeople.Concat(maxGroup.Take(amountLeft)).ToList();
                }
                else
                {
                    fivePeople = fivePeople
                        .Concat(teachers)
                        .Concat(students)
                        .ToList();
                }
            }
            {
                var amountLeft = 5 - fivePeople.Count;
                var remainingPeople = searchResults
                    .Take(amountLeft + 1)
                    .Select(sr => sr.Person.FriendlyName);
                fivePeople = fivePeople.Concat(remainingPeople).ToList();
            }
            return fivePeople;
        }
    }
}
