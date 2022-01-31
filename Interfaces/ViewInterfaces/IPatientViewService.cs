using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IPatientViewService : IDisposable
    {
        PatientManagerView ReturnPatientManagerView(PatientSearch searchQuery, ListResultAttributes displayProperties);
    }
}