
namespace dbRad.Classes
{
    public partial class ApplicationConnections
    {
        public string HostName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        private string UserPassword { get; set; } = "bv7gnE9AAL7g'kce8x)SHRf8!`Q&cd]3";

        public override string ToString()
        {
            return "Host = " + HostName +"; Database =  $DatabaseName$; Username = " + UserName + "; Password = " + UserPassword;
        }
    }
}
