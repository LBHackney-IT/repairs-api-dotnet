namespace RepairsApi.V2.Boundary.Response
{
    public class HubUserModel
    {
        public string Sub { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public double? VaryLimit { get; set; }
        public double? RaiseLimit { get; set; }
    }
}
