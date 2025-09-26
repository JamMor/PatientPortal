using System;
using PatientPortal.DTOs;

namespace PatientPortal.Interfaces
{
    public interface ITestResultService : IDisposable
    {
        void CreateTestResult(int patientId, int staffId, TestResultDTO formData);
        void DeleteTestResult(int testResultId);
    }
}