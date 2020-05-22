namespace Xap.Infrastructure.Interfaces.Security {
    public interface IXapLob {
        int LobId { get; set; }
        string LobName { get; set; }
        string LobDescription { get; set; }
        bool AmosEnabled { get; set; }
        bool GmcEnabled { get; set; }
        string AmosName { get; set; }
        string GmcName { get; set; }
    }
}
