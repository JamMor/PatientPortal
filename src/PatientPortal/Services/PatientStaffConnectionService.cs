using System.Linq;
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
    }
}