using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Model.Appointment
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int ProviderId { get; set; }
        public int SpecialisationId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string ChiefComplaint { get; set; }
        public string AppointmentStatus { get; set; }
        public float Fee { get; set; }

    }
    public enum AppointmentStatusEnum
    {
        Scheduled,
        Completed,
        Cancelled
    }
}
