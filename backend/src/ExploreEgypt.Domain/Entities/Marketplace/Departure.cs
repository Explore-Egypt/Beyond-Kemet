namespace ExploreEgypt.Domain.Entities.Marketplace;

public class Departure
{
    public Guid DepartureId { get; set; }

    public Guid TourId { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public int Capacity { get; set; }

    public DepartureStatus Status { get; set; } = DepartureStatus.Open;

    public Tour Tour { get; set; } = null!;

    public ICollection<global::ExploreEgypt.Domain.Entities.Booking.Booking> Bookings { get; set; }
        = new List<global::ExploreEgypt.Domain.Entities.Booking.Booking>();
}