using System.Linq;
using PatientPortal.DTOs;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class VisitService : IVisitService
    {
        private PatientPortalContext _context;
        public VisitService(PatientPortalContext context)
        {
            _context = context;
        }

        //COMMANDS
        public void CreateVisit(int patientId, int staffId, VisitDTO formData)
        {
            Visit newVisit = new Visit()
            {
                Comment = formData.Comment,
                DateOfVisit = formData.DateOfVisit,
                PatientId = patientId,
                StaffId = staffId,
                AssociatedHealthIssues = formData.HealthIssueIds
                    .Select(h => new VisitHealthIssueAssociation()
                    {
                        HealthIssueId = h
                    })
                    .ToList()
            };
            
            _context.Visits.Add(newVisit);
            _context.SaveChanges();
        }
        public void DeleteVisit(int visitId)
        {
            Visit? deletedVisit = _context.Visits
                .SingleOrDefault(visit => visit.VisitId == visitId);

            if(deletedVisit != null)
            {
                _context.Visits.Remove(deletedVisit);
                _context.SaveChanges();
            }
        }
    }
}