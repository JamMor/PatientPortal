using System;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ITestResultViewService : IDisposable
    {
        TestResultFormView GetNewTestResultForm(int patientId);
    }
}