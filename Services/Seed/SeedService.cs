using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class SeedService : ISeedService
    {
        private PatientPortalContext _context;

        public SeedService(PatientPortalContext context)
        {
            _context = context;
        }

    //Form Methods (Counts)
        public int GetStaffCount()
        {
            return _context.Staff.Count();
        }
        public int GetPatientCount()
        {
            return _context.Patients.Count();
        }
    //Staff
        public Faker<Staff> SeedStaff()
        {
            var staffFaker = new Faker<Staff>()
                .StrictMode(false)
                .Rules((f, s) =>
                {
                    s.IsAdmin = false;
                    s.FirstName = f.Name.FirstName();
                    s.LastName = f.Name.LastName();
                    s.Role = f.PickRandomParam("MD", "RN");
                    s.StaffUsername = f.Internet.UserName(s.FirstName, s.LastName) + "00$";
                    if (s.StaffUsername.Length < 10)
                    {
                        s.StaffUsername += f.Random.String2(10 - s.StaffUsername.Length, "0123456789");
                    };
                    PasswordHasher<Staff> hasher = new PasswordHasher<Staff>();
                    s.Password = hasher.HashPassword(s, "Password0$");
                    s.MessagingLink = new MessagingLink();
                    DateTime staffStartDate = f.Date.Between(DateTime.Today.AddYears(-25), DateTime.Today.AddYears(-1));
                    s.CreatedAt = staffStartDate;
                    s.UpdatedAt = f.Date.Between(staffStartDate, DateTime.Today);
                });

            return staffFaker;
        }

    //Utilities for Patient
        public List<int> SelectRandomStaffIds(int max = 1)
        {
            List<int> allStaffIds = _context.Staff
                .Select(s => s.StaffId)
                .ToList();

            Randomizer randomizer = new Randomizer();
            int numOfStaff = randomizer.Number(1, max);
            var shuffledList = randomizer.Shuffle(allStaffIds);
            List<int> selected = randomizer.ListItems<int>(allStaffIds, numOfStaff).ToList();

            return selected;
        }
        public List<int> LookupStaffLinkIds(List<int> staffIds)
        {
            return _context.Staff
                .Include(s => s.MessagingLink)
                .Where(s => staffIds.Contains(s.StaffId))
                .Select(s => s.MessagingLink.MessagingLinkId)
                .ToList();
        }

    //Patient
        public Faker<Patient> SeedPatient()
        {
            List<int> staffIds = SelectRandomStaffIds(5);

            var patientFaker = new Faker<Patient>()
                .StrictMode(false)
                .Rules((f, p) =>
                {
                    p.FirstName = f.Name.FirstName();
                    p.LastName = f.Name.LastName();
                    p.DOB = f.Date.Between(DateTime.Today.AddYears(-70), DateTime.Today.AddYears(-19));
                    p.Last4SSN = f.Random.String2(4, "0123456789");
                    p.Email = f.Internet.Email(p.FirstName, p.LastName);
                    DateTime patientSince = f.Date.Between(p.DOB.AddYears(18), DateTime.Today.AddMonths(-1));
                    p.CreatedAt = patientSince;
                    p.UpdatedAt = f.Date.Between(patientSince, DateTime.Today);
                    if (0 == f.Random.Number(3))
                    {
                        p.Address = SeedAddress().Generate();
                    }
                    p.MessagingLink = new MessagingLink()
                    {
                        CreatedAt = patientSince,
                        UpdatedAt = patientSince,
                    };
                    p.MedicalTeam = AddStaffToMedicalTeam(patientSince, staffIds);
                    // p.HealthIssues = SeedHealthIssue(patientSince, staffIds).Generate(f.Random.Number(1, 4));
                    p.Tests = SeedTestResult(patientSince, staffIds).Generate(f.Random.Number(1, 3));
                    p.Visits = SeedVisit(patientSince, staffIds).Generate(f.Random.Number(1, 5));

                });

            return patientFaker;
        }

        public Faker<Address> SeedAddress()
        {
            return new Faker<Address>()
                .StrictMode(false)
                .Rules((f, a) =>
                {
                    a.StreetAddress = f.Address.StreetAddress();
                    a.City = f.Address.City();
                    a.State = f.Address.State();
                    a.ZipCode = f.Address.ZipCode();
                });
        }

        public List<PatientStaffConnection> AddStaffToMedicalTeam(DateTime earliestDate, List<int> chosenStaffIds)
        {
            return chosenStaffIds.Select(c => new PatientStaffConnection()
                {
                    StaffId = c,
                    CreatedAt = earliestDate,
                    UpdatedAt = earliestDate
                })
                .ToList();
        }

        public Faker<Visit> SeedVisit(DateTime earliestDate, List<int> staffIds)
        {
            return new Faker<Visit>()
                .StrictMode(false)
                .Rules((f, v) =>
                {
                    v.Comment = f.Lorem.Paragraph(2);
                    v.StaffId = f.Random.ListItem(staffIds);
                    DateTime visitDate = f.Date.Between(earliestDate, DateTime.Today);
                    v.DateOfVisit = visitDate;
                    v.CreatedAt = visitDate;
                    v.UpdatedAt = visitDate;
                });
        }

        public Faker<TestResult> SeedTestResult(DateTime earliestDate, List<int> staffIds)
        {
            return new Faker<TestResult>()
                .StrictMode(false)
                .Rules((f, t) =>
                {
                    t.Type = f.PickRandomParam("Vitals", "Pathology", "Imaging", "Labwork");
                    t.Comment = f.Lorem.Paragraph(2);
                    t.StaffId = f.Random.ListItem(staffIds);
                    DateTime testDate = f.Date.Between(earliestDate, DateTime.Today);
                    t.CreatedAt = testDate;
                    t.UpdatedAt = testDate;
                });
        }

    //Health Issue
        public Faker<HealthIssue> SeedHealthIssue(DateTime earliestDate, List<int> staffIds, int patientId)
        {
            return new Faker<HealthIssue>()
                .StrictMode(false)
                .Rules((f, h) =>
                {
                    h.PatientId = patientId;
                    string shortDesc = f.Lorem.Sentence(2, 5);
                    if(shortDesc.Length > 29){shortDesc = shortDesc.Substring(0,29);}
                    h.ShortDescription = shortDesc;
                    h.LongDescription = f.Lorem.Paragraph(2);
                    DateTime issueDate = f.Date.Between(earliestDate, DateTime.Today.AddDays(-5));
                    h.CreatedAt = issueDate;
                    h.UpdatedAt = issueDate;
                    h.AssociatedVisits = SeedIssueVisits(issueDate, staffIds, patientId).Generate(f.Random.Number(2, 4));
                    h.AssociatedTestResults = SeedIssueTestResults(issueDate, staffIds, patientId).Generate(f.Random.Number(1, 3));
                });
        }

        public Faker<VisitHealthIssueAssociation> SeedIssueVisits(DateTime earliestDate, List<int> staffIds, int patientId)
        {
            return new Faker<VisitHealthIssueAssociation>()
                .StrictMode(false)
                .Rules((f, a) =>
                {
                    a.Visit = SeedVisit(earliestDate, staffIds, patientId).Generate();
                });
        }

        public Faker<TestHealthIssueAssociation> SeedIssueTestResults(DateTime earliestDate, List<int> staffIds, int patientId)
        {
            return new Faker<TestHealthIssueAssociation>()
                .StrictMode(false)
                .Rules((f, a) =>
                {
                    a.TestResult = SeedTestResult(earliestDate, staffIds, patientId).Generate();
                });
        }
        
        public Faker<Visit> SeedVisit(DateTime earliestDate, List<int> staffIds, int patientId)
        {
            return new Faker<Visit>()
                .StrictMode(false)
                .Rules((f, v) =>
                {
                    v.PatientId = patientId;
                    v.Comment = f.Lorem.Paragraph(2);
                    v.StaffId = f.Random.ListItem(staffIds);
                    DateTime visitDate = f.Date.Between(earliestDate, DateTime.Today);
                    v.DateOfVisit = visitDate;
                    v.CreatedAt = visitDate;
                    v.UpdatedAt = visitDate;
                });
        }

        public Faker<TestResult> SeedTestResult(DateTime earliestDate, List<int> staffIds, int patientId)
        {
            return new Faker<TestResult>()
                .StrictMode(false)
                .Rules((f, t) =>
                {
                    t.PatientId = patientId;
                    t.Type = f.PickRandomParam("Vitals", "Pathology", "Imaging", "Labwork");
                    t.Comment = f.Lorem.Paragraph(2);
                    t.StaffId = f.Random.ListItem(staffIds);
                    DateTime testDate = f.Date.Between(earliestDate, DateTime.Today);
                    t.CreatedAt = testDate;
                    t.UpdatedAt = testDate;
                });
        }

    //Messaging
        public Faker<Conversation> SeedConversation(DateTime earliestDate, List<int> staffLinkIds, int? patientLinkId)
        {
            //No more than 3 staff assigned to a conversation
            int maxStaff = staffLinkIds.Count;
            if (maxStaff > 3){ maxStaff = 3;}
            //No digital conversations beginning more than 10 years ago
            DateTime oldestPossible = DateTime.Today.AddYears(-10);
            oldestPossible = earliestDate > oldestPossible ? earliestDate : oldestPossible;

            return new Faker<Conversation>()
                .StrictMode(false)
                .Rules((f, m) =>
                {
                    m.WithPatient = patientLinkId != null;
                    m.Subject = f.Lorem.Sentence();
                    DateTime convoStart = f.Date.Between(oldestPossible, DateTime.Today.AddDays(-5));
                    m.CreatedAt = convoStart;
                    m.UpdatedAt = convoStart;
                    List<int> selectedStaff = f.Random.ListItems(staffLinkIds, f.Random.Number(1, maxStaff)).ToList();
                    m.ConversationParticipants = SeedConversationParticipants(convoStart, selectedStaff, patientLinkId);
                    m.Messages = SeedThreadOfMessages(convoStart, selectedStaff, patientLinkId);
                });
        }

        public List<ConversationParticipant> SeedConversationParticipants(DateTime earliestDate, List<int> staffLinkIds, int? patientLinkId)
        {
            List<ConversationParticipant> convoPartics = staffLinkIds
                .Select(linkId => new ConversationParticipant()
                {
                    MessagingLinkId = linkId,
                    CreatedAt = earliestDate,
                    UpdatedAt = earliestDate
                })
                .ToList();
            
            if(patientLinkId != null)
            {
                convoPartics.Add(new ConversationParticipant()
                {
                    MessagingLinkId = (int)patientLinkId,
                    CreatedAt = earliestDate,
                    UpdatedAt = earliestDate
                });
            }

            return convoPartics;
        }

        public List<Message> SeedThreadOfMessages(DateTime earliestDate, List<int> staffLinkIds, int? patientLinkId)
        {
            Randomizer randomizer = new Randomizer();
            int numStaffMessages = randomizer.Number(3,12);
            int numPatientMessages = 0;
            
            if(patientLinkId != null)
            {
                numStaffMessages = numStaffMessages/2;
                numPatientMessages = numStaffMessages + 1;
            }
            
            List<Message> messageThread = SeedMessage(earliestDate, randomizer.ListItem(staffLinkIds)).Generate(numStaffMessages);
            List<Message> patientMessages = SeedMessage(earliestDate, (int)patientLinkId).Generate(numPatientMessages);
            messageThread.AddRange(patientMessages);
            
            return messageThread;
        }

        public Faker<Message> SeedMessage(DateTime earliestDate, int linkId)
        {
            // Messages take place within 10 days as long as that isn't in the future
            DateTime maxDate = earliestDate.AddDays(10);
            maxDate = maxDate < DateTime.Today ? maxDate : DateTime.Today;

            return new Faker<Message>()
                .StrictMode(false)
                .Rules((f, m) =>
                {
                    m.MessagingLinkId = linkId;
                    m.MessageText = f.Lorem.Paragraph(1);
                    DateTime messageDate = f.Date.Between(earliestDate, maxDate);
                    m.CreatedAt = messageDate;
                    m.UpdatedAt = messageDate;
                });
        }

    //System
        public void AddRangeStaff(List<Staff> seedStaff)
        {
            _context.Staff.AddRange(seedStaff);
        }

        public void AddRangePatients(List<Patient> seedPatients)
        {
            _context.Patients.AddRange(seedPatients);
        }
        
        public void AddRangeHealthIssues(List<HealthIssue> seedHealthIssues)
        {
            _context.HealthIssues.AddRange(seedHealthIssues);
        }

        public void AddRangeConversations(List<Conversation> seedConversations)
        {
            _context.Conversations.AddRange(seedConversations);
        }

        public void SaveSeed()
        {
            _context.SaveChanges();
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PatientService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}