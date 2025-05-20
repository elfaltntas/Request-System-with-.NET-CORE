using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models 
{
    public enum RequestStatus
    {
        [Description("Beklemede")]
        Pending = 0,     // Beklemede

        [Description("İşleniyor")]
        InProgress = 1,  // İşleniyor

        [Description("Tamamlandı")]
        Completed = 2,   // Tamamlandı

        [Description("İptal Edildi")]
        Cancelled = 3    // İptal Edildi
    }


}
