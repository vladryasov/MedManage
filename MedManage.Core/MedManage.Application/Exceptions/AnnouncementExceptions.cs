namespace MedManage.Application.Exceptions
{
    public class AnnouncementNotFoundException : Exception
    {
        public AnnouncementNotFoundException(string message) : base(message) { }
    }
}