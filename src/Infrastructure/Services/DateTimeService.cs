using System;
using ContractorBackend.Application.Common.Interfaces;

namespace ContractorBackend.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
