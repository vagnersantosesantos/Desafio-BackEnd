using Domain.Enum;

namespace MotorcycleRental.Application.DTOs
{
    public class DeliveryDriverDto
    {
        private LicenseType licenseType;
        private bool Image;

        public DeliveryDriverDto(Guid id, string name, string cNPJ, DateTime birthDate, string licenseNumber, LicenseType licenseType, bool image, DateTime createdAt)
        {
            Id = id;
            Name = name;
            CNPJ = cNPJ;
            BirthDate = birthDate;
            LicenseNumber = licenseNumber;
            this.licenseType = licenseType;
            this.Image = image;
            CreatedAt = createdAt;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public DateTime BirthDate { get; set; }
        public string LicenseNumber { get; set; }
        public bool HasLicenseImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
