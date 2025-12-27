namespace NationalHealthCare.Appointment.SharedKarnel.Domain.ValueObjects;

public sealed class TimeSlot
{
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public int DurationInMinutes { get; private set; }

    private TimeSlot() { }

    public TimeSlot(DateTime startTime, int durationInMinutes)
    {
        if (durationInMinutes <= 0)
            throw new ArgumentException("Duration must be positive", nameof(durationInMinutes));

        if (startTime < DateTime.UtcNow)
            throw new ArgumentException("Start time cannot be in the past", nameof(startTime));

        StartTime = startTime;
        DurationInMinutes = durationInMinutes;
        EndTime = startTime.AddMinutes(durationInMinutes);
    }

    public bool OverlapsWith(TimeSlot other)
    {
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }

    public bool IsWithinBusinessHours(TimeSpan businessStart, TimeSpan businessEnd)
    {
        var startTimeOfDay = StartTime.TimeOfDay;
        var endTimeOfDay = EndTime.TimeOfDay;

        return startTimeOfDay >= businessStart && endTimeOfDay <= businessEnd;
    }

    public override bool Equals(object obj)
    {
        if (obj is not TimeSlot other)
            return false;

        return StartTime == other.StartTime && EndTime == other.EndTime;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartTime, EndTime);
    }
}
