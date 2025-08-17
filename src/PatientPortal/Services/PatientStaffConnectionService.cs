using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class PatientStaffConnectionService : IPatientStaffConnectionService
    {
        private PatientPortalContext _context;
        public PatientStaffConnectionService(PatientPortalContext context)
        {
            _context = context;
        }

        //COMMANDS
        public void AddStaffToPatientTeam(int patientId, int staffId)
        {
            PatientStaffConnection? oldLink = _context.PatientStaffConnections
                .FirstOrDefault(link => link.PatientId == patientId && link.StaffId == staffId);

            if (oldLink == null)
            {
                PatientStaffConnection newLink = new PatientStaffConnection()
                {
                    PatientId = patientId,
                    StaffId = staffId
                };
                _context.PatientStaffConnections.Add(newLink);
                _context.SaveChanges();
            }
        }

        public void RemoveStaffFromPatientTeam(int patientId, int staffId)
        {
            PatientStaffConnection? oldLink = _context.PatientStaffConnections
                .FirstOrDefault(link => link.PatientId == patientId && link.StaffId == staffId);

            if (oldLink != null)
            {
                _context.PatientStaffConnections.Remove(oldLink);
                _context.SaveChanges();
            }
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