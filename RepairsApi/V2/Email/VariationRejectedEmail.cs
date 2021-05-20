namespace RepairsApi.V2.Email
{
    internal class VariationRejectedEmail : EmailRequest
    {
        public VariationRejectedEmail(string address, int id)
            : base(address)
        {
            Set(EmailVariables.WorkOrderId, id);
        }
    }
}
