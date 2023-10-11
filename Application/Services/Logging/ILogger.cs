using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Logging
{
  public interface ILogger
  {
    Task WriteSuccessLog(ZonePersonCountDTO zone, string ExperimentName, string FileName);
    Task WriteErrorLog(string message, string zone, string fileName, DateTime timeStamp);
  }
}