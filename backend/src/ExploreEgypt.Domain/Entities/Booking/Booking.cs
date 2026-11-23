namespace ExploreEgypt.Domain.Entities.Booking;

public class Booking
{
    public Guid BookingId { get; set; }

    public Guid DepartureId { get; set; }

    public Guid TravelerId { get; set; }

    public int ParticipantCount { get; set; }

    public decimal UnitPriceSnapshot { get; set; }

    public decimal TotalPriceSnapshot { get; set; }

    public string Currency { get; set; } = string.Empty;

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Marketplace.Departure Departure { get; set; } = null!;
}