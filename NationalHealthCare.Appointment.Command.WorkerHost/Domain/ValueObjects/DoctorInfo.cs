namespace NationalHealthCare.Appointment.Command.WorkerHost.Domain.ValueObjects;

public sealed class DoctorInfo
{
    public string DoctorName { get; private set; }
    public string DoctorId { get; private set; }
    public string Specialization { get; private set; }
    public string Department { get; private set; }

    private DoctorInfo() { }

    public DoctorInfo(string doctorName, string doctorId, string specialization = null, string department = null)
    {
        if (string.IsNullOrWhiteSpace(doctorName))
            throw new ArgumentException("Doctor name cannot be empty", nameof(doctorName));

        if (string.IsNullOrWhiteSpace(doctorId))
            throw new ArgumentException("Doctor ID cannot be empty", nameof(doctorId));

        DoctorName = doctorName;
        DoctorId = doctorId;
        Specialization = specialization;
        Department = department;
    }

    public override bool Equals(object obj)
    {
        if (obj is not DoctorInfo other)
            return false;

        return DoctorId == other.DoctorId && DoctorName == other.DoctorName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DoctorName, DoctorId);
    }
}
