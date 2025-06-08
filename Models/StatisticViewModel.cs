using PBL3_MicayOnline.Models.DTOs;

namespace PBL3_MicayOnline.Models
{
    public class StatisticViewModel
    {
        public List<OrderDto> Orders { get; set; } = new(); 
        public DateTime Date { get; set; }
        public string Type { get; set; } = "month";
        public decimal TotalRevenue { get; set; }
    }
}
