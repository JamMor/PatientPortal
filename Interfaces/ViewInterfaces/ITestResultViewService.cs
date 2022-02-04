using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ITestResultViewService : IDisposable
    {
        TestResultFormView ReturnTestResultFormView(int patientId);
    }
}