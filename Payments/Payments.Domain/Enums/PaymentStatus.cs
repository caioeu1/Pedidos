namespace Payments.Domain.Enums;

public enum PaymentStatus
{
    Pending,
    Authorized,
    Captured,
    Refunded,
    Failed,
    Cancelled
}
