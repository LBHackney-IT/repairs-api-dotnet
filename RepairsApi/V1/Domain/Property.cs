namespace RepairsApi.V1.Domain
{
    public class PropertyModel // Model suffix to avoid usage of keyword
    {
        public string PropertyReference { get; set; }
        public Address Address { get; set; }
        public HierarchyType HierarchyType { get; set; }
    }
}
