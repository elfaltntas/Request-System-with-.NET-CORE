using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract.ModelService
{
    public interface IRequestProcess
    {
        void CreateRequestProcess(RequestProcess requestProcess);
    }

}
