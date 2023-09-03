using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Services.ClassCancellationRequest
{
    public class ClassCancellationRequest : IClassCancellationRequest
    {
        private readonly DataContext _dbContext;

        public ClassCancellationRequest(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
    }   
}