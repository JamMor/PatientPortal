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
    public class SeedViewService : ISeedViewService
    {
        private ISeedService _seedService;

        public SeedViewService(ISeedService seedService)
        {
            _seedService = seedService;
        }

        public SeedFormView ReturnSeedFormView()
        {
            return new SeedFormView()
                {
                    CurrentPatients = _seedService.GetPatientCount(),
                    CurrentStaff = _seedService.GetStaffCount()
                };
        }
        public void SeedNStaff(int staffAmount)
        {
            if(staffAmount <= 0){return;}

            List<Staff> seededStaff = _seedService.SeedStaff().Generate(staffAmount);
            
            _seedService.AddRangeStaff(seededStaff);
            _seedService.SaveSeed();
        }
        public void SeedNPatients(int patientAmount)
        {
            if(patientAmount <= 0){return;}

            List<Patient> seededPatients = _seedService.SeedPatient().Generate(patientAmount);

            _seedService.AddRangePatients(seededPatients);
            _seedService.SaveSeed();

            //Populate Patient Conversations and Health Issues
            Randomizer randomizer = new Randomizer();
            List<Conversation> seededConversations = new List<Conversation>();
            List<HealthIssue> seededHealthIssues = new List<HealthIssue>();

            foreach(Patient p in seededPatients)
            {
                
                DateTime startDate = p.CreatedAt;
                List<int> staffIds = p.MedicalTeam.Select(m => m.StaffId).ToList();
                List<int> staffLinkIds = _seedService.LookupStaffLinkIds(staffIds);
                int patientId = p.PatientId;
                int patientLinkId = p.MessagingLink.MessagingLinkId;
                
                seededConversations.AddRange(_seedService
                    .SeedConversation(startDate, staffLinkIds, patientLinkId)
                    .Generate(randomizer.Number(2,5))
                    );
                
                seededHealthIssues.AddRange(_seedService
                    .SeedHealthIssue(startDate, staffIds, patientId)
                    .Generate(randomizer.Number(1,3))
                    );

            }

            _seedService.AddRangeConversations(seededConversations);
            _seedService.AddRangeHealthIssues(seededHealthIssues);
            _seedService.SaveSeed();
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _seedService.Dispose();
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