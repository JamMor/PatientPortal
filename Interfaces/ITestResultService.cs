using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ITestResultService : IDisposable
    {
        void CreateTestResult(int patientId, int staffId, TestResultFormView formData);
        void DeleteTestResult(int testResultId);
    }
}