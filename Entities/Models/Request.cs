using Entities.EntityType.Abstract;
using Entities.EntityType.Concrete;
using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public class Request
    {

        public string RequestId { get; set; }  // benzersiz

        public string Title { get; set; }  // Talebin başlığı

        public string Description { get; set; }  // Talep açıklaması

        public DateTime CreatedDate { get; set; }  // Talebin oluşturulma tarihi

        public DateTime ModifiedDate { get; set; }  // Talebin güncellenme tarihi

        public string? PersonelId { get; set; }  // Personel ID (bir talep bir personele aittir)

        public Personel Personel { get; set; }  // Talebin ait olduğu personel (Many-to-One ilişkisi)

        public RequestStatus RequestStatus { get; set; }  // Talebin durumu (Pending, InProgress, Completed, vb.)

        public List<RequestProcess> RequestProcess { get; set; } = new List<RequestProcess>();  // Talep süreci ile ilgili bilgiler
        
    }
}
