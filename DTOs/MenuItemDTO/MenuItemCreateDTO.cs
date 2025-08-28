namespace WeddingInvite.Api.DTOs.MenuItemDTO
{
    public class MenuItemCreateDTO
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool IsPopular { get; set; }
        //public string? ImageUrl { get; set; }
    }
}
