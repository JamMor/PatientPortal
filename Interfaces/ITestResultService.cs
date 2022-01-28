using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ITestResultService : IDisposable
    {

        // TestResult GetTestResultbyId(int testResultId);
        void CreateTestResult(int patientId, int staffId, TestResultFormView formData);
        // void UpdateTestResult(TestResultFormView testResultInfo);
        void DeleteTestResult(int testResultId);
    }
}