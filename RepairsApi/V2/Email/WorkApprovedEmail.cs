namespace RepairsApi.V2.Email
{
    internal class WorkApprovedEmail : EmailRequest
    {
        public WorkApprovedEmail(string address, object id)
            : base(address)
        {
            Set(EmailVariables.WorkOrderId, id);
        }
    }
}
