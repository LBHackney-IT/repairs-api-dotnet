namespace RepairsApi.V2.Email
{
    public class WorkApprovedEmail : EmailRequest
    {
        public WorkApprovedEmail(string address, object id)
            : base(address)
        {
            Set(EmailVariables.WorkOrderId, id);
        }
    }
}
