namespace RepairsApi.V2.Email
{
    internal class VariationApprovedEmail : EmailRequest
    {
        public VariationApprovedEmail(string address, int id)
            : base(address)
        {
            Set(EmailVariables.WorkOrderId, id);
        }
    }
}
