namespace RSDK.Client
{
    public partial class ProjectCreateProgressControl
    {
        private int ProgressPercent => Payload?.Percent ?? 0;
        private string ProgressBarClass => (Payload?.IsCompleted == true) ? "bg-success" : "bg-info";
    }
}