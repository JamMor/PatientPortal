using System;
using System.Collections.Generic;
using Bogus;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ISeedService : IDisposable
    {
        //Staff
        Faker<Staff> SeedStaff();

        //Utilities for Patient
        List<int> SelectRandomStaffIds(int max = 1);
        List<int> LookupStaffLinkIds(List<int> staffIds);

        //Patient
        Faker<Patient> SeedPatient();
        Faker<Address> SeedAddress();
        List<PatientStaffConnection> AddStaffToMedicalTeam(DateTime earliestDate, List<int> chosenStaffIds);
        Faker<Visit> SeedVisit(DateTime earliestDate, List<int> staffIds);
        Faker<TestResult> SeedTestResult(DateTime earliestDate, List<int> staffIds);

        //HealthIssues
        Faker<HealthIssue> SeedHealthIssue(DateTime earliestDate, List<int> staffIds, int patientId);
        Faker<VisitHealthIssueAssociation> SeedIssueVisits(DateTime earliestDate, List<int> staffIds, int patientId);
        Faker<TestHealthIssueAssociation> SeedIssueTestResults(DateTime earliestDate, List<int> staffIds, int patientId);
        
        //Messaging
        Faker<Conversation> SeedConversation(DateTime earliestDate, List<int> staffLinkIds, int? patientLinkId);
        List<ConversationParticipant> SeedConversationParticipants(DateTime earliestDate, List<int> staffLinkIds, int? patientLinkId);
        List<Message> SeedThreadOfMessages(DateTime earliestDate, List<int> staffLinkIds, int? patientLinkId);
        Faker<Message> SeedMessage(DateTime earliestDate, int linkId);

        //System
        void AddRangeStaff(List<Staff> seedStaff);
        void AddRangePatients(List<Patient> seedPatients);
        void AddRangeHealthIssues(List<HealthIssue> seedHealthIssues);
        void AddRangeConversations(List<Conversation> seedConversations);
        void SaveSeed();
    }
}