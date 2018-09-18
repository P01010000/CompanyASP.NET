using System;
using System.ComponentModel.DataAnnotations;

namespace CompanyASP.NET.Models
{
    public class Employee : IModel
    {
        //[Key]
        public int? Id { get; set; }
        //[Editable(false)]
        public int? PersonId { get; set; }
        //[Required]
        public string LastName { get; set; }
        //[Required]
        public string FirstName { get; set; }
        //[Required]
        public DateTime? Birthday { get; set; }
        //[Required]
        public string Phone { get; set; }
        //[Required]
        public string Gender { get; set; }
        //[Required]
        public DateTime? EmployeeSince { get; set; }

        public bool Identity(int[] ids)
        {
            return Id == ids[0];
        }
    }
}
