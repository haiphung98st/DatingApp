namespace API.Extensions
{
    public static class DatetimeExtension
    {
        public static int CalculateAge(this DateTime dob)
        {
            var age = Convert.ToInt32(DateTime.Today.Year - dob.Year);
            if (dob.Date > DateTime.Today.AddDays(-age))
                age--;
            return age;
        }
    }
}
