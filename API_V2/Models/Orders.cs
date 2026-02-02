namespace API_V2.Models
{
    public class Orders
    {

        public int OrderID { get; set; }
        public string? ShipName { get; set; }

        public int ShipVia { get; set; }

        public decimal Freight { get; set; }    

    }
}
