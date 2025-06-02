using domain.Model.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.ModelDto.Appointment
{
    public class AppointmentDto
    {
        public int UserTypeId { get; set; }
        public int PatientId { get; set; }
        public int ProviderId { get; set; }
        public int SpecialisationId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string ChiefComplaint { get; set; }
        public float Fee { get; set; }
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
    }
}
