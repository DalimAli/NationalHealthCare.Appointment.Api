namespace NationalHealthCare.Appointment.SharedKarnel.Domain.ValueObjects;

public sealed class PatientInfo
{
    public string PatientName { get; private set; }
    public string PatientId { get; private set; }
    public string ContactNumber { get; private set; }
    public string Email { get; private set; }

    private PatientInfo() { }

    public PatientInfo(string patientName, string patientId, string contactNumber = null, string email = null)
    {
        if (string.IsNullOrWhiteSpace(patientName))
            throw new ArgumentException("Patient name cannot be empty", nameof(patientName));

        if (string.IsNullOrWhiteSpace(patientId))
            throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));

        PatientName = patientName;
        PatientId = patientId;
        ContactNumber = contactNumber;
        Email = email;
    }

    public override bool Equals(object obj)
    {
        if (obj is not PatientInfo other)
            return false;

        return PatientId == other.PatientId && PatientName == other.PatientName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PatientName, PatientId);
    }
}
