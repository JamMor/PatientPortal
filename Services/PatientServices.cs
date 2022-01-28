using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class PatientService : IPatientService
    {
        private PatientPortalContext _context;
        public PatientService(PatientPortalContext context)
        {
            _context = context;
        }

        // Comparable Service Structure ==== REMOVE LATER
        // private readonly IBasketQueryService _basketQueryService;
        // private readonly IRepository<CatalogItem> _itemRepository;

        // public BasketViewModelService(
        //     IRepository<CatalogItem> itemRepository,
        //     IBasketQueryService basketQueryService)
        // {
        //     _basketQueryService = basketQueryService;
        //     _itemRepository = itemRepository;
        // }
        // {
        //     public async Task<BasketViewModel> CreatePatient(string userName)
        //     {
        //         var basketSpec = new BasketWithItemsSpecification(userName);
        //         var basket = (await _basketRepository.GetBySpecAsync(basketSpec));

        //         if (basket == null)
        //         {
        //             return await CreateBasketForUser(userName);
        //         }
        //         var viewModel = await Map(basket);
        //         return viewModel;
        //     }

        //     private async Task<BasketViewModel> CreateBasketForUser(string userId)
        //     {
        //         var basket = new Basket(userId);
        //         await _basketRepository.AddAsync(basket);

        //         return new BasketViewModel()
        //         {
        //             BuyerId = basket.BuyerId,
        //             Id = basket.Id,
        //         };
        //     }
        // }

        //COMMANDS
        public bool DoesPatientExist(PatientFormView patientInfo)
        {
            return _context.Patients.Any(patient =>
                    patient.Last4SSN == patientInfo.Last4SSN
                    && patient.DOB == patientInfo.DOB
                    && patient.FirstName == patientInfo.FirstName
                    && patient.LastName == patientInfo.LastName);
        }

        public int CreatePatient(PatientFormView patientInfo)
        {
            Patient newPatient = new Patient()
            {
                FirstName = patientInfo.FirstName,
                LastName = patientInfo.LastName,
                DOB = patientInfo.DOB,
                Last4SSN = patientInfo.Last4SSN,
                PhoneNumber = patientInfo.PhoneNumber,
                Email = patientInfo.Email,
                MessagingLink = new MessagingLink()
            };
            
            if(patientInfo.Address != null)
            {
                newPatient.Address = new Address()
                {
                    StreetAddress = patientInfo.Address.StreetAddress,
                    City = patientInfo.Address.City,
                    State = patientInfo.Address.State,
                    ZipCode = patientInfo.Address.ZipCode
                };
            }

            _context.Patients.Add(newPatient);
            _context.SaveChanges();

            return newPatient.PatientId;
        }

        public void UpdatePatient(PatientFormView patientInfo)
        {}
        public void DeletePatient(int patientId)
        {
            Patient deletedPatient = _context.Patients
                .Include(p => p.MessagingLink)
                .SingleOrDefault(patient => patient.PatientId == patientId);
            if (deletedPatient != null)
            {
                _context.Patients.Remove(deletedPatient);
                _context.SaveChanges();
            }
        }

        // QUERIES
        public Patient GetPatientbyId(int patientId)
        {
            Patient patient = _context.Patients
                .Include(patient => patient.HealthIssues
                    .OrderByDescending(h => h.UpdatedAt))
                .ThenInclude(issue => issue.AssociatedTestResults)
                .Include(patient => patient.HealthIssues)
                .ThenInclude(issue => issue.AssociatedVisits)
                .Include(patient => patient.Visits
                    .OrderByDescending(v => v.CreatedAt))
                .ThenInclude(team => team.Staff)
                .Include(patient => patient.Tests
                    .OrderByDescending(t => t.CreatedAt))
                .ThenInclude(team => team.Staff)
                .Include(patient => patient.MedicalTeam)
                .ThenInclude(team => team.Staff)
                .Include(p => p.MessagingLink)
                .FirstOrDefault(patient => patient.PatientId == patientId);

            return patient;
        }
        public PatientManagerView GetPatientbyQuery(PatientSearch SearchBar, ListResultAttributes DisplayProperties)
        
        {            
            var patientQuery = _context.Patients
            .Where(patient => SearchBar.SearchPatientId == null || patient.PatientId == SearchBar.SearchPatientId)
            .Where(patient => string.IsNullOrEmpty(SearchBar.SearchFirstName) || patient.FirstName.StartsWith(SearchBar.SearchFirstName))
            .Where(patient => string.IsNullOrEmpty(SearchBar.SearchLastName) || patient.LastName.StartsWith(SearchBar.SearchLastName))
            .Where(patient => string.IsNullOrEmpty(SearchBar.SearchSSN) || patient.Last4SSN == SearchBar.SearchSSN)
            .Where(patient => SearchBar.SearchBirthdate == null || patient.DOB == SearchBar.SearchBirthdate);

            switch (DisplayProperties.SortOrder)
            {
                case "PatientId_desc":
                    patientQuery = patientQuery.OrderByDescending(p => p.PatientId);
                    break;
                case "PatientId_asc":
                    patientQuery = patientQuery.OrderBy(p => p.PatientId);
                    break;
                case "LastName_desc":
                    patientQuery = patientQuery.OrderByDescending(p => p.LastName);
                    break;
                case "LastName_asc":
                    patientQuery = patientQuery.OrderBy(p => p.LastName);
                    break;
                case "DOB_desc":
                    patientQuery = patientQuery.OrderByDescending(p => p.DOB);
                    break;
                case "DOB_asc":
                    patientQuery = patientQuery.OrderBy(p => p.DOB);
                    break;
                default:
                    patientQuery = patientQuery.OrderBy(s => s.LastName);
                    break;
            }

            DisplayProperties.ResultsCount = patientQuery.Count();
            
            List<Patient> queryResults = patientQuery
                .Skip(DisplayProperties.ResultsPerPage*(DisplayProperties.CurrentPage-1))
                .Take(DisplayProperties.ResultsPerPage)
                .ToList();

            PatientManagerView ViewModel = new PatientManagerView
            {
                SearchBar = SearchBar,
                SearchResults = queryResults,
                DisplayProperties = DisplayProperties
            };

            return ViewModel;
        }

        //Patient Staff Connection
        public void AddStaffToPatientTeam(int patientId, int staffId)
        {}
        public void RemoveStaffFromPatientTeam(int patientId, int staffId)
        {}

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