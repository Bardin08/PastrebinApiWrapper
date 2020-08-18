namespace PastebinApiWrapper.Models.Enums
{
    /// <summary>
    /// Paste expire date
    /// </summary>
    public enum PasteExpireDate
    {
        Never = 0,
        TenMinutes = 1,
        OneHour = 2,
        OneDay = 3,
        OneWeek = 4,
        TwoWeeks = 5,
        OneMonth = 6,
        SixMonths = 7,
        OneYear = 8
    }
}
