namespace InventoryService.Domain.Enums
{
    public enum ReservationStatus
    {
        Reserved = 1,   // Đã giữ hàng
        Released = 2,   // Đã giải phóng
        Completed = 3,  // Đã hoàn tất (Order confirmed / paid)
        Expired = 4     // Hết hạn tự động (Quartz/Background job xử lý)
    }
}
