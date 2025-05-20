using System;

namespace Entities.Dto
{
    public class RequestUserDto
    {
        public string RequestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; } // Talebin oluşturulma tarihi
        public DateTime? ModifiedDate { get; set; } // Talebin güncelleme tarihi
        public string Status { get; set; } // Talebin durumu
    }

}
