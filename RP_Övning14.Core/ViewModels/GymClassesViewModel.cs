

namespace RP_Övning14.Core.ViewModels
{
    public class GymClassesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        //public DateTime EndTime { get { return StartTime + Duration; } }
        public string Description { get; set; }
        public bool isUserAttending { get; set; }
    }
}
