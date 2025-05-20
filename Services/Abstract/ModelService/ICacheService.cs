using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract.ModelService
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiry = null);
        void Remove(string key);
        bool Exists(string key);
    }
}
