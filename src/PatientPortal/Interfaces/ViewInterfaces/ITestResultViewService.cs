using System;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ITestResultViewService : IDisposable
    {
        TestResultForm? ReturnTestResultForm(int patientId);
    }
}